using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TUFF
{
    [System.Serializable]
    public class ChangeAnimationPackAction : EventAction
    {
        public FieldOriginType originType;
        public PersistentType persistentOrigin = PersistentType.AvatarController;
        
        public CharacterAnimationHandler targetAnimationHandler;
        public AnimationPack pack;
        public ChangeAnimationPackAction()
        {
            eventName = "Change Animation Pack";
            eventColor = EventGUIColors.character;
        }
        public override void Invoke()
        {
            if (originType == FieldOriginType.FromPersistentInstance)
            {
                if (persistentOrigin != PersistentType.None)
                {
                    FollowerInstance.instance?.ChangeAnimationPack(pack);
                }
            }
            else
            {
                if (targetAnimationHandler != null)
                    targetAnimationHandler.LoadAnimationPack(pack);
            }    
            isFinished = true;
        }
    }
}
