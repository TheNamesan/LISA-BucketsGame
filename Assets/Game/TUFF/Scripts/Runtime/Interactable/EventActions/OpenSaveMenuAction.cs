using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TUFF
{
    public class OpenSaveMenuAction : EventAction
    {
        public OpenSaveMenuAction()
        {
            eventName = "Open Save Menu";
            eventColor = new Color(0.5f, 1f, 0.95f, 1f);
        }
        public override void Invoke()
        {
            UIController.instance.OpenFileSelectMenu(FileSelectMenuMode.SaveFile, this);
        }
    }
}

