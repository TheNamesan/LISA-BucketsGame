using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TUFF
{
    [System.Serializable]
    public class ChangeSwitchAction : EventAction
    {
        [Tooltip("Reference to the Interactable Object.")]
        public InteractableObject target;
        [Tooltip("Switch to change to.")]
        public int newSwitch = 0;
        public ChangeSwitchAction()
        {
            eventName = "Change Switch";
            eventColor = new Color(1f, 0.5f, 0.5f, 1f);
        }
        public override void Invoke()
        {
            if (target != null) target.currentSwitch = newSwitch;
            isFinished = true;
        }
        
    }
}

