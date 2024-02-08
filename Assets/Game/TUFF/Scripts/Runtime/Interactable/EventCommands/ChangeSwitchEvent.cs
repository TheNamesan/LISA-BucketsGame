using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TUFF
{
    [CreateAssetMenu(fileName = "EVTChangeSwitch", menuName = "TUFF/Events/Change Switch Event")]
    public class ChangeSwitchEvent : EventCommand
    {
        [Tooltip("Reference to the Interactable Object.")]
        public InteractableObject target;
        [Tooltip("Switch to change to.")]
        public int newSwitch = 0;
        public override void Invoke()
        {
            if(target != null) target.currentSwitch = newSwitch;
            isFinished = true;
        }
        public override void OnInstantiate()
        {
            eventName = "Change Switch";
            eventColor = new Color(1f, 0.5f, 0.5f, 1f);
        }
    }
}
