using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TUFF
{
    [System.Serializable]
    public class ChangeMagazinesAction : EventAction
    {
        public AddSetOperationType operation = AddSetOperationType.Add;
        public NumberOperand operand = new();

        public ChangeMagazinesAction()
        {
            eventName = "Change Magazines Action";
            eventColor = EventGUIColors.party;
        }
        public override void Invoke()
        {
            int value = (int)operand.GetNumber();
            if (operation == AddSetOperationType.Add) PlayerData.instance.AddMags(value);
            else if (operation == AddSetOperationType.Set) PlayerData.instance.SetMags(value);
            isFinished = true;
        }
    }
}
