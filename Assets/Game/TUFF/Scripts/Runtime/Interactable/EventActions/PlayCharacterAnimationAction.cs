using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TUFF
{
    [System.Serializable]
    public class PlayCharacterAnimationAction : EventAction
    {
        public FieldOriginType originType;
        public PersistentType persistentOrigin = PersistentType.AvatarController;

        public CharacterAnimationHandler targetAnimationHandler;
        public AnimationClip clip;
        public PlayCharacterAnimationAction()
        {
            eventName = "Play Character Animation";
            eventColor = EventGUIColors.character;
        }
        public override void Invoke()
        {
            if (originType == FieldOriginType.FromPersistentInstance)
            {
                FollowerInstance.instance?.PlayAnimation(clip);
            }
            else
            {
                if (targetAnimationHandler != null)
                    targetAnimationHandler.PlayAnimation(clip);
            }
            isFinished = true;
        }
    }
}
