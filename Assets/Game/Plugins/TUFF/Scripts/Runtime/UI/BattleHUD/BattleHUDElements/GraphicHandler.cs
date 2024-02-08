using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

namespace TUFF
{
    public class GraphicHandler : MonoBehaviour
    {
        [Header("References")]
        [Tooltip("Reference to the Targetable's Image component.")]
        public Image userImage;
        [Tooltip("Reference to the Image component for effects.")]
        public Image imageVFX;
        [Tooltip("Reference to the HighlightAnimator component for effects.")]
        public HighlightAnimator highlightAnimator;
        [Tooltip("Reference to the FlashImageHandler component for effects.")]
        public FlashImageHandler vfxFlashHandler;
        [Tooltip("Reference to the outline Shadow component.")]
        public Shadow outline;
        [Tooltip("Reference to the outline Image component.")]
        public Image outlineImage;
        [Tooltip("Reference to the Vulnerability Info parent")]
        public RectTransform vulnerabilityInfoParent;
        [Tooltip("Reference to the Vulnerability Info image")]
        public Image vulnerabilityInfoImage;
        [Tooltip("Reference to the Vulnerability Info text")]
        public TMP_Text vulnerabilityInfoText;
        [Tooltip("Reference to the parent GameObject to all state visuals.")]
        public RectTransform stateVisualsParent;
        public Canvas canvasRoot;

        [HideInInspector] public bool isHighlighted = false;

        [HideInInspector] public RectTransform rect;
        [HideInInspector] public Color originalUserColor = new Color();
        [HideInInspector] public Color originalVFXColor = new Color();
        protected Tween colorTween;
        protected Tween twitchTween;
        protected Vector2 orgPosition = Vector2.zero;
        protected TUFFMotion motionHandler = new TUFFMotion();
        public List<StateVisual> stateVisuals = new List<StateVisual>();

        public void Awake()
        {
            rect = GetComponent<RectTransform>();
            canvasRoot = LISAUtility.GetCanvasRoot(transform);
            if(userImage != null)
            {
                originalUserColor = userImage.color;
            }
            if (imageVFX != null)
            {
                originalVFXColor = imageVFX.color;
            }
            ActivateOutline(false);
        }
        public Vector2 GetOverlayPosition() => LISAUtility.GetCanvasOverlayPosition(this);
        public Vector2 GetCameraPosition() => LISAUtility.GetCanvasCameraPosition(this);
        public void LateUpdate()
        {
            if (outlineImage != null)
            {
                outlineImage.sprite = userImage.sprite;
            }
        }
        public void ChangeGraphic(Sprite sprite)
        {
            userImage.sprite = sprite;
        }
        public void ActivateOutline(bool active)
        {
            if (outlineImage == null) return;
            outlineImage.gameObject.SetActive(active);
        }
        public void ActivateOutline(int elementIndex, VulnerabilityType vulType, bool activateIfNoVulnerability = false)
        {
            if (outlineImage == null) return;
            if (vulType != VulnerabilityType.Normal) { ActivateOutline(true); }
            else if (vulType == VulnerabilityType.Normal) ActivateOutline(activateIfNoVulnerability);
            
            outline.effectColor = BattleHUD.GetVulnerabilityColor(vulType);
            var elements = TUFFSettings.elements;
            bool active = (vulType != VulnerabilityType.Normal);
            var icon = (elementIndex > 0 ? elements[elementIndex].icon : null);
            var color = BattleHUD.GetVulnerabilityColor(vulType);
            string text = BattleHUD.GetVulnerabilityText(vulType);
            SetVulnerabilityInfo(active, icon, color, text);
        }
        public void Highlight(bool highlight)
        {
            if (imageVFX == null) return;
            KillTween();
            outlineImage.color = Color.white;
            if (vulnerabilityInfoParent != null)
                vulnerabilityInfoParent.gameObject.SetActive(false);
            if (highlight)
            {
                ActivateOutline(true);
            }
            else
            {
                ActivateOutline(false);
            }
            if (highlightAnimator != null)
                highlightAnimator.Highlight(highlight);
            else imageVFX.enabled = highlight;

            isHighlighted = highlight;
        }
        public void Highlight(bool highlight, int elementIndex, VulnerabilityType vulType)
        {
            Highlight(highlight);
            ActivateOutline(elementIndex, vulType, true);
        }
        public void SetVulnerabilityInfo(bool active, Sprite icon, Color color, string text)
        {
            if (vulnerabilityInfoParent != null)
                vulnerabilityInfoParent.gameObject.SetActive(active);
            
            if (vulnerabilityInfoImage != null)
            {
                vulnerabilityInfoImage.gameObject.SetActive(icon != null);
                vulnerabilityInfoImage.sprite = icon;
            }
            if (vulnerabilityInfoText != null)
            {
                vulnerabilityInfoText.text = text;
                vulnerabilityInfoText.color = color;
            }
        }
        public void Highlight(bool highlight, Color outlineColor)
        {
            Highlight(highlight);
            outline.effectColor = outlineColor;
        }
        public void PlayMotion(MotionOneTimeType motionType)
        {
            motionHandler?.PlayOneTimeMotion(userImage, motionType);
        }
        public void AddStateVisual(ActiveState activeState)
        {
            if (stateVisualsParent == null) return;
            StateVisual stateVisual = activeState.state.visual;
            if (stateVisual == null) return;
            GameObject visualGOInstance = Instantiate(stateVisual.gameObject, stateVisualsParent);
            var visual = visualGOInstance.GetComponent<StateVisual>();
            visual.InitializeVisual(activeState);
            stateVisuals.Add(visual);
            ApplyGraphicColor();
        }
        public void RemoveStateVisual(ActiveState activeState)
        {
            for(int i = 0; i < stateVisuals.Count; i++)
            { 
                if(stateVisuals[i].activeState == activeState)
                {
                    Debug.Log("Remove");
                    Destroy(stateVisuals[i].gameObject);
                    stateVisuals.RemoveAt(i);
                    ApplyGraphicColor();
                    return;
                }
            }
        }
        
        public void ApplyGraphicColor()
        {
            Color color = originalUserColor;
            for (int i = 0; i < stateVisuals.Count; i++)
            {
                color *= stateVisuals[i].graphicColoration;
            }
            userImage.color = color;
        }
        public void Flash(Color color, float duration)
        {
            KillTween();
            vfxFlashHandler?.Flash(color, duration);
        }
        public void Twitch(float strength)
        {
            if (twitchTween != null) { userImage.transform.position = orgPosition; KillTwitchTween(); }
            orgPosition = userImage.transform.position;
            twitchTween = userImage.transform.DOShakePosition(0.15f, new Vector2(strength, 0), 30, 90, false, false)
                .OnComplete(() => { userImage.transform.position = orgPosition; });
        }
        protected void KillTwitchTween()
        {
            twitchTween?.Kill(true); twitchTween = null;
        }
        protected void KillTween()
        {
            colorTween?.Complete();
            colorTween?.Kill();
            colorTween = null;
        }
    }
}
