using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TUFF
{
    public class ChangeCharacterRunAction : EventAction
    {
        public bool enable = false;
        public ChangeCharacterRunAction()
        {
            eventName = "Change Character Run";
            eventColor = EventGUIColors.character;
        }
        public override void Invoke()
        {
            PlayerData.instance.charProperties.disableRun = !enable;
            isFinished = true;
        }
    }
}

