using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TUFF
{
    public class ChangeMenuAccessAction : EventAction
    {
        public bool enable = false;
        public ChangeMenuAccessAction()
        {
            eventName = "Change Menu Access";
            eventColor = EventGUIColors.system;
        }
        public override void Invoke()
        {
            PlayerData.instance.charProperties.disableMenuAccess = !enable;
            isFinished = true;
        }
    }

}
