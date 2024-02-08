using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TUFF
{
    [System.Serializable]
    public class ActiveState
    {
        public State state;
        public Targetable user;
        public int remainingTurns = 0;
        public int startingTurns = 0;
        public float remainingSeconds = 0f;
        public bool isAutoState = false;
        
        public ActiveState(State state, Targetable user, bool isAutoState = false)
        {
            this.state = state;
            this.user = user;
            this.isAutoState = isAutoState;
            SetTurns(state);
        }

        protected void SetTurns(State state)
        {
            if (state.autoRemovalTiming != AutoRemovalTiming.None && !isAutoState)
            {
                int stateTurns = Random.Range(Mathf.Abs(state.durationInTurns.x), Mathf.Abs(state.durationInTurns.y + 1));
                if(stateTurns > remainingTurns)
                {
                    startingTurns = stateTurns;
                    remainingTurns = startingTurns;
                }
            }
            if (state.removeByWalking)
            {
                if (state.autoRemovalTiming != AutoRemovalTiming.None) remainingSeconds = GetRemainingSecondsFromTurns();
                else remainingSeconds = state.removeByWalkingSeconds;
            }
        }

        public void ResetState(bool isAutoState = false)
        {
            if (isAutoState) this.isAutoState = true;
            SetTurns(state);
        }

        /// <summary>
        /// Returns true if state was removed.
        /// </summary>
        /// <param name="turns">Number of turns to reduce from duration.</param>
        public bool ReduceStateTurnDuration(int turns)
        {
            if (isAutoState) return false;
            remainingTurns -= 1 * turns;
            if(remainingTurns <= 0)
            {
                ExpireState();
                return true;
            }
            if (state.removeByWalking && remainingSeconds > 0)
            {
                if (state.removeByWalkingSeconds <= 0) { remainingSeconds = 0; return true; } 
                else remainingSeconds = GetRemainingSecondsFromTurns();
            }
            return false;
        }

        public void ExpireState()
        {
            State progressiveState = state.progressiveState;
            int chance = state.progressiveStateTriggerChance;
            if (progressiveState != null) BattleManager.RollForStateApply(progressiveState, null, user, chance);
            user.RemoveState(this);
        }

        protected virtual float GetRemainingSecondsFromTurns()
        {
            return Mathf.Lerp(0, state.removeByWalkingSeconds, Mathf.InverseLerp(0, Mathf.Abs(state.durationInTurns.y), remainingTurns));
        }

        public bool ShouldDisplayCount()
        {
            if (state.autoRemovalTiming == AutoRemovalTiming.None || remainingTurns <= 0 || isAutoState || !user.CanShowStatus())
                return false;
            return true;
        }
    }
}
