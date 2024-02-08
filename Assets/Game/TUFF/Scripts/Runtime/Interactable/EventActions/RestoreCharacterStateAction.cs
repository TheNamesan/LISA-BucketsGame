using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TUFF
{
    [System.Serializable]
    public class RestoreCharacterStateAction : EventAction
    {
        public FieldOriginType originType;

        //public CharacterAnimationHandler targetAnimationHandler;
        public RestoreCharacterStateAction()
        {
            eventName = "Restore Character State";
            eventColor = EventGUIColors.character;
        }
        public override void Invoke()
        {
            if (originType == FieldOriginType.FromPersistentInstance)
            {
                FollowerInstance.instance?.RestoreState();
            }
            else
            {
                //if (targetAnimationHandler != null) // Change this to be Scene Character
                //    targetAnimationHandler.RestoreState();
            }
            isFinished = true;
        }
    }
}
