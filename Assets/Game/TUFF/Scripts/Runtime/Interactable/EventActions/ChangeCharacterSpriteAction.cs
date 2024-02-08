using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TUFF
{
    [System.Serializable]
    public class ChangeCharacterSpriteAction : EventAction
    {
        public FieldOriginType originType;
        public PersistentType persistentOrigin = PersistentType.AvatarController;

        public CharacterAnimationHandler targetAnimationHandler;
        public Sprite sprite;
        public ChangeCharacterSpriteAction()
        {
            eventName = "Change Character Sprite";
            eventColor = EventGUIColors.character;
        }
        public override void Invoke()
        {
            if (originType == FieldOriginType.FromPersistentInstance)
            {
                FollowerInstance.instance?.ChangeSprite(sprite);
            }
            else
            {
                if (targetAnimationHandler != null)
                    targetAnimationHandler.ChangeSprite(sprite);
            }
            isFinished = true;
        }
    }
}