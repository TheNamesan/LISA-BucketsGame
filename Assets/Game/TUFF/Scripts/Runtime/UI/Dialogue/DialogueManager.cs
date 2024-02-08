using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using TMPro;

namespace TUFF
{
    public class DialogueManager : MonoBehaviour
    {
        [Header("Text")]
        public Dialogue dialogue;
        public int currentSentence = 0;
        public bool textboxInitiated = false;
        public bool inUse = false;

        [Header("References")]
        public TextMeshProUGUI text;
        public TMP_Typewriter typewriter;
        public BoxTransitionHandler transition;
        public AdjustToPreferredTextSize textSizeAdjuster;
        public AdjustToOtherRect adjustToOtherRect;
        public RectTransform continuePrompt;
        
        private LayoutGroup rootLayout;
        private RectTransform parentRT;
        private UIMenu uiMenu;
        [System.NonSerialized]
        public EventAction actionCallback = null;
        private static bool m_continuedDialogue = false; // Change this to be a brief period of buffer time before a dialogue is called? (For things like SFXs)


        [Header("Input")]
        public float skipTimeBuffer = 0.03f;

        List<string> sentences;
        Camera cam;

        [SerializeField] public float skipTime = 0;
        public bool autoContinue = false;
        private float m_autoEndDelay = 0.5f;
        private IEnumerator autoContinueCoroutine;

        Vector3 bottomLeftCorner;
        Vector3 upperLeftCorner;
        Vector3 upperRightCorner;
        Vector3 bottomRightCorner;

        Vector3 canvasBottomLeftCorner;
        Vector3 canvasUpperLeftCorner;
        Vector3 canvasUpperRightCorner;
        Vector3 canvasBottomRightCorner;

        public RectTransform rect { get => transform as RectTransform; }
        private RectTransform parentRect { get => transform.parent as RectTransform; }

        public static List<DialogueManager> openBoxes = new(); // tmp // Change this to be a brief period of buffer time before a dialogue is called? (For things like SFXs)
        private static readonly Vector2 DEFAULT_OFFSET = new Vector2(0, 1f);

        private void Awake()
        {
            if (transition == null) transition = GetComponent<BoxTransitionHandler>();
        }
        private void OnEnable()
        {
            DisplayContinuePrompt(false);
        }
        void InitialValues()
        {
            inUse = true;
            StartCoroutine(InitialValuesCoroutine());
        }
        private IEnumerator InitialValuesCoroutine()
        {
            if (uiMenu == null)
                uiMenu = GetComponent<UIMenu>();
            if (rootLayout == null)
                rootLayout = GetComponent<LayoutGroup>();

            BattleManager.instance.hud.ShowWindowsDynamic(false);
            UIController.instance.SetMenu(uiMenu);
            LISAUtility.SetPivot(rect, new Vector2(rect.pivot.x, 0f));
            cam = Camera.main;
            parentRT = transform.parent as RectTransform;
            currentSentence = 0;
            text.enabled = true;
            skipTime = 0;
            sentences = new List<string>();
            foreach (var sentence in dialogue.sentences)
            {
                string text = "";
                if (sentence.sentenceTextType == SentenceTextType.Localized)
                {
                    text = LISAUtility.GetLocalizedDialogueText(sentence.key);
                }
                else if (sentence.sentenceTextType == SentenceTextType.Simple)
                    text = sentence.text;
                sentences.Add(text);
            }
            if (!m_continuedDialogue) transition?.Appear();
            SetPosition();
            SetVoicebank();
            openBoxes.Add(this);
            yield break;
        }
        private void SetPosition()
        {
            Vector3 originPos = Vector3.zero;
            if (dialogue.origin != null) originPos = dialogue.origin.position;
            if (dialogue.textboxType == TextboxType.Normal)
            {
                LISAUtility.SetPivot(rect, new Vector2(rect.pivot.x, 0f));
                Vector2 uiOffset = new Vector2(parentRT.rect.width * 0.5f, parentRT.rect.height * 0.5f);
                Vector2 viewportPosition = cam.WorldToViewportPoint(originPos + (Vector3)dialogue.positionOffset + (Vector3)DEFAULT_OFFSET);
                Vector2 proportionalPosition = new Vector2(viewportPosition.x * parentRT.rect.width, viewportPosition.y * parentRT.rect.height);
                rect.localPosition = proportionalPosition - uiOffset;
                if (dialogue.origin == null) rect.localPosition = dialogue.positionOffset * 100f;
                //AdjustOutOfBounds(); // needs fixing lol
                LISAUtility.SetPivot(rect, new Vector2(rect.pivot.x, 0.5f));
            }
            else if (dialogue.textboxType == TextboxType.Fixed)
            {
                if (dialogue.fixedTextboxPosition == FixedTextboxPosition.Bottom)
                {
                    rect.pivot = new Vector2(0.5f, 0);
                    rect.anchorMin = new Vector2(0.5f, 0);
                    rect.anchorMax = new Vector2(0.5f, 0);
                    rect.localPosition = new Vector2(0, -parentRT.rect.height * 0.5f);
                }
                else if (dialogue.fixedTextboxPosition == FixedTextboxPosition.Top)
                {
                    rect.pivot = new Vector2(0.5f, 1);
                    rect.anchorMin = new Vector2(0.5f, 1);
                    rect.anchorMax = new Vector2(0.5f, 1);
                    rect.localPosition = new Vector2(0, parentRT.rect.height * 0.5f);
                }
                //ForceRebuild();
            }
        }

        private void AdjustOutOfBounds()
        {
            float preferredWidth = LayoutUtility.GetPreferredWidth(rect);
            float preferredHeight = LayoutUtility.GetPreferredHeight(rect);

            Vector3[] rectCorners = new Vector3[4];
            rect.GetWorldCorners(rectCorners);
            bottomLeftCorner = rectCorners[0];
            upperLeftCorner = rectCorners[1];
            upperRightCorner = rectCorners[2];
            bottomRightCorner = rectCorners[3];

            Vector3[] canvasCorners = new Vector3[4];
            parentRT.GetWorldCorners(canvasCorners);
            canvasBottomLeftCorner = canvasCorners[0];
            canvasUpperLeftCorner = canvasCorners[1];
            canvasUpperRightCorner = canvasCorners[2];
            canvasBottomRightCorner = canvasCorners[3];

            //Debug.DrawLine(bottomLeftCorner, upperRightCorner, Color.cyan, 5);
            //Debug.DrawLine(upperLeftCorner, bottomRightCorner, Color.red, 5);
            //Debug.DrawLine(canvasBottomLeftCorner, canvasUpperRightCorner, Color.white, 5);
            //Debug.DrawLine(canvasUpperLeftCorner, canvasBottomRightCorner, Color.white, 5);

            if (bottomRightCorner.x > canvasBottomRightCorner.x)
            {
                rect.position += Vector3.left * Mathf.Abs(canvasBottomRightCorner.x - bottomRightCorner.x);
                //ForceRebuild();
            }
            if (bottomLeftCorner.x < canvasBottomLeftCorner.x)
            {
                rect.position += Vector3.right * Mathf.Abs(canvasBottomLeftCorner.x - bottomLeftCorner.x);
                //ForceRebuild();
            }
            if (upperLeftCorner.y > canvasUpperLeftCorner.y)
            {
                rect.position += Vector3.down * Mathf.Abs(canvasUpperLeftCorner.y - upperLeftCorner.y);
                //ForceRebuild();
            }
            if (bottomLeftCorner.y < canvasBottomLeftCorner.y)
            {
                rect.position += Vector3.up * Mathf.Abs(canvasBottomLeftCorner.y - bottomLeftCorner.y);
                //ForceRebuild();
            }
        }

        void SetVoicebank()
        {
            if (dialogue.baseVoicebank != null)
            {
                typewriter.clip = dialogue.baseVoicebank.clip;
                typewriter.pitch = dialogue.baseVoicebank.pitch;
                typewriter.pitchVariation = dialogue.baseVoicebank.pitchVariation;
            }
            if (dialogue.sentences.Length <= 0) return;
            if (dialogue.sentences[currentSentence].voicebank != null)
            {
                typewriter.clip = dialogue.sentences[currentSentence].voicebank.clip;
                typewriter.pitch = dialogue.sentences[currentSentence].voicebank.pitch;
                typewriter.pitchVariation = dialogue.sentences[currentSentence].voicebank.pitchVariation;
            }
        }

        float SetTextSpeed()
        {
            if (dialogue.sentences[currentSentence].textSpeed > 0) return dialogue.sentences[currentSentence].textSpeed;
            else if (dialogue.baseTextSpeed > 0) return dialogue.baseTextSpeed;
            else return Dialogue.defaultBaseTextSpeed;
        }
        private void AdjustText()
        {
            textSizeAdjuster?.Adjust();
            adjustToOtherRect?.Adjust();
        }

        //private void ForceRebuild()
        //{
        //    StartCoroutine(RebuildCoroutine());
        //    //LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
        //    //LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform); //called two times on purpose
        //} 

        private IEnumerator RebuildCoroutine()
        {
            if (rootLayout == null) yield break;
            //LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
            //rootLayout.enabled = false;
            yield return new WaitForEndOfFrame();
            //rootLayout.enabled = true;
        }

        private void Update()
        {
            SkipTimeTimer();

            if (textboxInitiated)
            {
                if (UIController.instance.skipButtonHold)
                {
                    if (skipTime == 0)
                    {
                        AdvanceText();
                        skipTime = skipTimeBuffer;
                    }
                }
                if (UIController.instance.actionButtonDown)
                {
                    if (!UIController.instance.skipButtonHold)
                        AdvanceText();
                }
                if (!IsTextPlaying() && autoContinue)
                {
                    AdvanceText();
                }
            }
        }

        private void SkipTimeTimer()
        {
            if (skipTime != 0)
            {
                skipTime -= Time.deltaTime;
                if (skipTime <= 0)
                {
                    skipTime = 0;
                }
            }
        }
        private bool IsTextPlaying()
        {
            return text.maxVisibleCharacters < text.GetParsedText().Length;
        }
        private void AdvanceText()
        {
            if (IsTextPlaying())
            {
                typewriter.Skip();
            }
            else
            { 
                if (autoContinue)
                {
                    if (autoContinueCoroutine == null)
                    {
                        autoContinueCoroutine = AdvanceTextCoroutine();
                        StartCoroutine(autoContinueCoroutine);
                    }
                }
                else DisplayNextSentence(); 
            }
        }
        private IEnumerator AdvanceTextCoroutine()
        {
            yield return new WaitForSeconds(m_autoEndDelay);
            DisplayNextSentence();
            autoContinueCoroutine = null;
        }

        public void StartDialogue()
        {
            InitialValues();
            textboxInitiated = true;
            DisplayNextSentence();
        }

        public void DisplayNextSentence()
        {
            if (currentSentence >= sentences.Count)
            {
                if (textboxInitiated)
                {
                    if (!ChoicesIsNext()) CloseTextbox(); // Change this to buffer
                    else {
                        if (actionCallback != null) actionCallback.isFinished = true;
                        textboxInitiated = false;
                        Debug.Log("Choices is next!"); 
                    }
                }
                return;
            }
            DisplayContinuePrompt(false);
            SetVoicebank();
            autoContinue = false;
            var savedTags = new List<TUFFTextParser.TagData>();
            var text = TUFFTextParser.ParseText(sentences[currentSentence], savedTags);
            if (savedTags.Exists(e => e.type == TUFFTextParser.TextTagType.AutoContinue) || (currentSentence >= sentences.Count - 1 && ChoicesIsNext()))
            {
                autoContinue = true;
                Debug.Log("AUTOCONTINUE: " + autoContinue);
            }
            typewriter.Play(text, SetTextSpeed(), () => { DisplayContinuePrompt(true); });
            currentSentence++;
            AdjustText();
            SetPosition();
        }

        private void DisplayContinuePrompt(bool display)
        {
            if (continuePrompt != null)
            {
                if (display && autoContinue) return;
                continuePrompt.gameObject.SetActive(display);
            }
        }
        public void CloseTextbox()
        {
            StartCoroutine(EndDialogue());
        }
        protected IEnumerator EndDialogue()
        {
            textboxInitiated = false;
            m_continuedDialogue = false;
            dialogue.onDialogueEnd?.Invoke();
            if (uiMenu != null) UIController.instance.SetMenu(null);
            BattleManager.instance.hud.ShowWindowsDynamic(true);
            if (actionCallback != null) actionCallback.isFinished = true;
            if (transition != null)
            {
                if (DialogueIsNext()) // Change this to buffer
                {
                    m_continuedDialogue = true;
                }
                else
                {
                    text.enabled = false;
                    DisplayContinuePrompt(false);
                    transition?.Dissapear();
                    while (transition.state == BoxTransitionState.Dissapearing)
                    {
                        yield return null;
                    }
                }
            }
            openBoxes.RemoveAt(openBoxes.IndexOf(this));
            inUse = false;
            gameObject.SetActive(false);
            //Destroy(gameObject);
        }
        private bool DialogueIsNext() // Change this to buffer
        {
            if (actionCallback == null) return false;
            int index = actionCallback.parent.GetActionIndex(actionCallback);
            if (index < 0 || index >= actionCallback.parent.content.Count - 1) return false;
            return actionCallback.parent.content[index + 1] is ShowDialogueAction;
        }
        private bool ChoicesIsNext() // Change this to buffer
        {
            if (actionCallback == null) return false;
            int index = actionCallback.parent.GetActionIndex(actionCallback);
            if (index < 0 || index >= actionCallback.parent.content.Count - 1) return false;
            return actionCallback.parent.content[index + 1] is ShowChoicesAction;
        }
    }
}
