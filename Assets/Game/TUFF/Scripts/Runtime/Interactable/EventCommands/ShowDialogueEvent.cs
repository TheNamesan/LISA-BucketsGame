using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TUFF
{
    [CreateAssetMenu(fileName = "EVTShowDialogue", menuName = "TUFF/Events/Show Dialogue Event")]
    public class ShowDialogueEvent : EventCommand
    {
        public Dialogue dialogue = null;

        public override void Invoke()
        {
            //Dialogue.InvokeTextbox(dialogue, this);
        }

        public override void OnInstantiate()
        {
            eventName = "Show Dialogue";
        }
    }
}
