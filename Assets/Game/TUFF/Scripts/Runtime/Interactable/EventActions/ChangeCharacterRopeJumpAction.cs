using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TUFF
{
    public class ChangeCharacterRopeJumpAction : EventAction
    {
        public bool enable = false;
        public ChangeCharacterRopeJumpAction()
        {
            eventName = "Change Character Rope Jump";
            eventColor = EventGUIColors.character;
        }
        public override void Invoke()
        {
            PlayerData.instance.charProperties.disableRopeJump = !enable;
            isFinished = true;
        }
    }
}

