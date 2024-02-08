using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TUFF
{
    [System.Serializable]
    public class ShowDialogueAction : EventAction
    {
        public Dialogue dialogue = new Dialogue();
        public ShowDialogueAction()
        {
            eventName = "Show Dialogue";
        }
        public override void OnInstantiate()
        {
            dialogue.baseVoicebank = TUFFSettings.defaultVoicebank;
        }
        public override void Invoke()
        {
            Dialogue.InvokeTextbox(dialogue, this);
        }
    }
}

