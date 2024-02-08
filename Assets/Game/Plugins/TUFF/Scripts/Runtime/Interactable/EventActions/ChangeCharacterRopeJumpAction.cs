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
            eventColor = new Color(0.7f, 1f, 0.6f, 1f);
        }
        public override void Invoke()
        {
            PlayerData.instance.charProperties.disableRopeJump = !enable;
            isFinished = true;
        }
    }
}

