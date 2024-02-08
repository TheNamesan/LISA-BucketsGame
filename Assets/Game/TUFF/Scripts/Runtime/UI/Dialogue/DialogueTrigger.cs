using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace TUFF
{
    public class DialogueTrigger : MonoBehaviour
    {
        public List<Dialogue> dialogues = new List<Dialogue>();

        [Header("References")]
        [SerializeField] GameObject textboxPrefab;
        [SerializeField] GameObject systemTBPrefab;
        [SerializeField] GameObject canvas;

        void Start()
        {
            canvas = UIController.instance.uiContent;
        }

        public void InvokeTextbox(int index)
        {
            GameObject textbox;
            if(dialogues[index].textboxType == TextboxType.Fixed)
            {
                textbox = Instantiate(systemTBPrefab);
            }
            else
            {
                textbox = Instantiate(textboxPrefab);
            }
        
            RectTransform textboxRect = textbox.GetComponent<RectTransform>();
            textboxRect.SetParent(canvas.transform, false);
            DialogueManager textboxManager = textbox.GetComponent<DialogueManager>();
            Dialogue textboxDialogue = textboxManager.dialogue;
            textboxDialogue.textboxType = dialogues[index].textboxType;
            if (textboxDialogue.textboxType == TextboxType.Fixed)
            {
                textboxRect.position = Vector3.zero;
            }
            else
            {
                textboxDialogue.origin = dialogues[index].origin;
                textboxDialogue.positionOffset = dialogues[index].positionOffset;
            }
        
            textboxDialogue.baseVoicebank = dialogues[index].baseVoicebank;
            textboxDialogue.baseTextSpeed = dialogues[index].baseTextSpeed;
            textboxDialogue.sentences = new DialogueSentence[dialogues[index].sentences.Length];
            for (int i = 0; i < dialogues[index].sentences.Length; i++)
            {
                textboxDialogue.sentences[i] = dialogues[index].sentences[i];
            }
            textboxDialogue.onDialogueEnd = dialogues[index].onDialogueEnd;

            UIMenu uiMenu = textbox.GetComponent<UIMenu>();
            if (uiMenu != null) UIController.instance.SetMenu(uiMenu);
            textboxManager.StartDialogue();
        }
    }
}
