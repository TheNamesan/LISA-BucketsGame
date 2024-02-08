using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace TUFF
{
    public enum TextboxType
    {
        Normal = 0,
        Fixed = 1,
    }
    public enum FixedTextboxPosition
    {
        Bottom = 0,
        Top = 1
    }
    [System.Serializable]
    public class Dialogue
    {
        [Tooltip("The textbox prefab to use. Normal shows the standard square textbox above the character speaking. Fixed shows a transparent box that stretches across all the screen.")]
        public TextboxType textboxType;
        [Tooltip("FromScene: Spawns the textbox above an object on the scene.\nFromPersistentInstance: Spawns textboxes above GameObjects that are marked as 'DontDestroyOnLoad'.")]
        public FieldOriginType originType;
        [Tooltip("Reference to the GameObject who's speaking. The textbox appears above the GameObject.")]
        public Transform origin;
        [Tooltip("The offset position to add to the textbox's world position.")]
        public Vector2 positionOffset;
        [Tooltip("The persistent GameObject who’s speaking.")]
        public PersistentType persistentOrigin = PersistentType.AvatarController;
        [Tooltip("The position of the Fixed Textbox.")]
        public FixedTextboxPosition fixedTextboxPosition;
        [Tooltip("The base Voicebank to use for sentences.")]
        public Voicebank baseVoicebank;
        public const float defaultBaseTextSpeed = 40f;
        [Tooltip("The base Text Speed to use to display in sentences. If 0 or less, will use the default instead. Default: 40.")]
        public float baseTextSpeed = 0f;
        [Tooltip("If true, will override the Textbox prefab with a custom one.")]
        public bool overrideTextbox;
        [Tooltip("The custom textbox prefab to use.")]
        public GameObject customTextbox;
        public DialogueSentence[] sentences = new DialogueSentence[0];
        [Tooltip("Unity Event to call when this Dialogue ends.")]
        public UnityEvent onDialogueEnd;

        public static void InvokeTextbox(Dialogue dialogue, EventAction commandCallback = null)
        {
            GameManager.instance.StartCoroutine(InvokeTextboxCoroutine(dialogue, commandCallback));
            //GameManager.instance.StartCoroutine(StartDialogueDelay(textboxManager));
        }
        public static IEnumerator InvokeTextboxCoroutine(Dialogue dialogue, EventAction commandCallback = null)
        {
            var canvas = UIController.instance.uiContent;
            if (canvas == null) yield break;
            while (UIController.instance.textbox.inUse || UIController.instance.dimTextbox.inUse)
            {
                yield return null;
            }
            GameObject textbox;
            if (dialogue.overrideTextbox && dialogue.customTextbox != null)
            {
                textbox = Object.Instantiate(dialogue.customTextbox);
            }
            else if (dialogue.textboxType == TextboxType.Fixed)
            {
                textbox = Object.Instantiate(TUFFSettings.fixedTextbox);
                //textbox = UIController.instance.dimTextbox.gameObject;
            }
            else
            {
                textbox = Object.Instantiate(TUFFSettings.defaultTextbox);
                //textbox = UIController.instance.textbox.gameObject; // POSITION OFFSET BREAKS WITH THIS! FIX!!
            }
            RectTransform textboxRect = textbox.GetComponent<RectTransform>();
            textboxRect.SetParent(canvas.transform, false);
            DialogueManager textboxManager = textbox.GetComponent<DialogueManager>();
            Dialogue textboxDialogue = textboxManager.dialogue;
            textboxDialogue.textboxType = dialogue.textboxType;
            textboxDialogue.originType = dialogue.originType;
            textboxDialogue.persistentOrigin = dialogue.persistentOrigin;
            if (textboxDialogue.textboxType == TextboxType.Fixed)
            {
                textboxRect.position = Vector3.zero;
                textboxDialogue.fixedTextboxPosition = dialogue.fixedTextboxPosition;
            }
            else
            {
                if (textboxDialogue.originType == FieldOriginType.FromScene)
                {
                    textboxDialogue.origin = dialogue.origin;
                }
                else
                {
                    textboxDialogue.origin = LISAUtility.GetPersistentOrigin(textboxDialogue.persistentOrigin);
                }
                textboxDialogue.positionOffset = dialogue.positionOffset;
            }
            textboxDialogue.baseVoicebank = dialogue.baseVoicebank;
            textboxDialogue.baseTextSpeed = dialogue.baseTextSpeed;
            textboxDialogue.sentences = new DialogueSentence[dialogue.sentences.Length];
            for (int i = 0; i < dialogue.sentences.Length; i++)
            {
                textboxDialogue.sentences[i] = dialogue.sentences[i];
            }
            textboxDialogue.onDialogueEnd = dialogue.onDialogueEnd;

            if (commandCallback != null)
            {
                textboxManager.actionCallback = commandCallback;
            }
            //yield return new WaitForEndOfFrame();
            //yield return new WaitForEndOfFrame();
            //yield return new WaitForEndOfFrame();
            //yield return new WaitForEndOfFrame();
            //textboxManager.gameObject.SetActive(true);
            textboxManager.StartDialogue();
        }

        public static IEnumerator PreloadTextboxes() // Make this a pool
        {
            yield break;
            //var fixedTextbox = Object.Instantiate(TUFFSettings.fixedTextbox);
            //var textbox = Object.Instantiate(TUFFSettings.defaultTextbox);
            //yield return new WaitForEndOfFrame();
            //Object.Destroy(fixedTextbox);
            //Object.Destroy(textbox);
        }
        public void CopyFrom(Dialogue source)
        {
            Dialogue.CopyTo(source, this);
        }
        public static Dialogue CopyTo(Dialogue original, Dialogue clone)
        {
            if (original == null) return null;
            if (clone == null) clone = new Dialogue();
            clone.textboxType = original.textboxType;
            clone.originType = original.originType;
            clone.origin = original.origin;
            clone.positionOffset = original.positionOffset;
            clone.persistentOrigin = original.persistentOrigin;
            clone.fixedTextboxPosition = original.fixedTextboxPosition;
            clone.baseVoicebank = original.baseVoicebank;
            clone.baseTextSpeed = original.baseTextSpeed;
            clone.overrideTextbox = original.overrideTextbox;
            clone.customTextbox = original.customTextbox;
            clone.sentences = new DialogueSentence[original.sentences.Length];
            System.Array.Copy(original.sentences, clone.sentences, original.sentences.Length);
            clone.onDialogueEnd = original.onDialogueEnd;
            return clone;
        }
    }
}
