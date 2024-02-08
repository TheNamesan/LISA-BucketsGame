using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TUFF
{
    public class ToggleCharacterLightSourceAction : EventAction
    {
        public FieldOriginType originType;
        public SceneCharacter sceneCharacter;
        public bool enable = false;
        public ToggleCharacterLightSourceAction()
        {
            eventName = "Toggle Character Light Source";
            eventColor = EventGUIColors.character;
        }
        public override void Invoke()
        {
            if (originType == FieldOriginType.FromPersistentInstance)
            {
                FollowerInstance.instance?.EnableLightSource(enable);
            }
            else
            {
                sceneCharacter?.EnableLightSource(enable);
            }
            isFinished = true;
        }
    }
}
