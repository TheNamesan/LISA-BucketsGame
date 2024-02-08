using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace TUFF
{
    public class ChangeSpriteEvent : EventCommand
    {

        public FieldOriginType originType;
        public PersistentType persistentOrigin = PersistentType.AvatarController;

        [Tooltip("Reference to the Sprite Renderer.")]
        public SpriteRenderer spriteRenderer;

        [Header("Enabled")]
        public bool keepEnabled = true;
        public bool enabled = true;
        
        [Header("Sprite")]
        [Tooltip("If true, the Renderer's sprite will stay the same.")]
        public bool keepSprite = false;
        [Tooltip("Sprite to change to.")]
        public Sprite sprite;

        [Header("Order In Layer")]
        [Tooltip("If true, the sprite's order in layer will stay the same.")]
        public bool keepOrderInLayer = true;
        [Tooltip("Rendering order within the sorting layer.")]
        public int orderInLayer = 0;
        
        [Header("Color")]
        [Tooltip("If true, the sprite's color will stay the same.")]
        public bool keepColor = true;
        [Tooltip("Sprite's color.")]
        public Color color = Color.white;
        [Tooltip("If true, only the sprite color's alpha value will be changed.")]
        public bool changeOnlyTransparency = false;
        [Tooltip("Fades the sprite into the color in seconds. Set to 0 or lower to change color instantly.")]
        public float colorFadeDuration = 0;
        [Tooltip("If true, the event will be paused until the fade ends.")]
        public bool waitForFade = false;

        [Header("Material")]
        [Tooltip("If true, the sprite's material will stay the same.")]
        public bool keepMaterial = true;
        [Tooltip("Sprite's material.")]
        public Material material = null;

        private Tween tween;


        private SpriteRenderer GetSpriteRendererFromInstance()
        {
            var instance = LISAUtility.GetPersistentInstance(persistentOrigin);
            if (instance == null) { return null; }
            if (persistentOrigin == PersistentType.AvatarController)
            {
                var avatarController = instance as OverworldCharacterController;
                var spriteRenderer = avatarController.sprite;
                return spriteRenderer;
            }
            return null;
        }
        public override void Invoke()
        {
            var spriteRenderer = this.spriteRenderer;
            if (originType == FieldOriginType.FromPersistentInstance)
            {
                if (persistentOrigin != PersistentType.None)
                {
                    spriteRenderer = GetSpriteRendererFromInstance();
                }
                else
                {
                    isFinished = true;
                    return;
                }
            }

            if (spriteRenderer != null)
            {
                if (!keepEnabled) spriteRenderer.enabled = enabled;
                if (!keepSprite) spriteRenderer.sprite = sprite;
                if (!keepOrderInLayer) spriteRenderer.sortingOrder = orderInLayer;
                if (!keepMaterial) spriteRenderer.material = material;
                if (keepColor)
                {
                    isFinished = true;
                    return;
                }
                if (colorFadeDuration <= 0)
                {
                    if (!changeOnlyTransparency) spriteRenderer.color = color;
                    else spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, color.a);
                    isFinished = true;
                }
                else
                {
                    KillTween();
                    if(!changeOnlyTransparency) tween = spriteRenderer.DOColor(color, colorFadeDuration).OnComplete(() => { if (waitForFade) isFinished = true; });
                    else tween = spriteRenderer.DOFade(color.a, colorFadeDuration).OnComplete(() => { if (waitForFade) isFinished = true; });
                    if (!waitForFade) isFinished = true;
                }
                
            }
            else isFinished = true;
        }
        public override void OnInstantiate()
        {
            eventName = "Change Sprite";
            eventColor = new Color(0.7f, 1f, 0.6f, 1f);
            color = Color.white;
        }
        private void OnDestroy()
        {
            KillTween();
        }
        private void KillTween()
        {
            tween?.Kill();
            tween = null;
        }
    }
}

