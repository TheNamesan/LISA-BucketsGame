using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace TUFF
{
    public class HitDisplay : MonoBehaviour
    { 
        [Header("References")]
        [SerializeField] protected Transform elementsContainer;
        [SerializeField] protected TMP_Text text;
        [SerializeField] protected Image image;
        [SerializeField] protected TMP_FontAsset critFont;
        [HideInInspector] public Targetable target;

        public static Color colorTint = Color.white;

        protected BattleAnimationEvent hitInfo;
        [HideInInspector] public HitDisplayGroup group;

        protected const float critSizeMult = 2f;

        public void InitializeHitDisplay(HitDisplayGroup group, BattleAnimationEvent hitInfo = null)
        {
            this.hitInfo = hitInfo;
            this.group = group;
        }
        public virtual void DisplayRegen(int value, DamageType damageType)
        {
            image.gameObject.SetActive(false);
            SetHitText(value, damageType);
            SetRegenColor(damageType);
        }
        public virtual void DisplayState(State state, bool removal = false)
        {
            image.gameObject.SetActive(true);
            var label = "+";
            if (removal) label = "-";
            text.text = $"{label}{state.GetName()}";
            image.sprite = state.icon;
            image.color = colorTint;
            SetStateColor(state, removal);
        }
        public virtual void DisplayVulnerability(int elementIndex, VulnerabilityType vulType)
        {
            var element = TUFFSettings.elements[elementIndex];
            Sprite sprite = element.icon;
            if (sprite != null)
            {
                image.gameObject.SetActive(true);
                image.sprite = sprite;
                image.color = colorTint;
            }
            SetVulnerabilityText(vulType);
            SetVulnerabilityColor(vulType);
        }
        public virtual void DisplayMiss()
        {
            image.gameObject.SetActive(false);
            text.text = TUFFSettings.missText;
            SetDefaultColor();
        }
        public virtual void DisplayHitValue(int value, bool isCrit, int elementIndex = -1)
        {
            image.gameObject.SetActive(false);
            if (elementIndex > 0)
            {
                var trueElement = TUFFSettings.elements[elementIndex];
                Sprite sprite = trueElement.icon;
                if (sprite != null)
                {
                    image.gameObject.SetActive(true);
                    image.sprite = sprite; 
                }
            }
            if (isCrit)
            {
                text.font = critFont;
                transform.localScale = new Vector3(transform.localScale.x * critSizeMult, transform.localScale.y * critSizeMult, transform.localScale.z);
            } 
            SetHitTextColor();
            SetHitText(value, hitInfo.damageType);
        }
        protected virtual void SetDefaultColor()
        {
            text.color = TUFFSettings.HPDamageTextColor * colorTint;
        }
        protected virtual void SetHitText(int value, DamageType damageType)
        {
            string label = "-";
            if (damageType == DamageType.HPRecover || damageType == DamageType.SPRecover ||
                damageType == DamageType.TPRecover) label = "+";
            if (value == 0) label = "";
            string type = "";
            if (damageType == DamageType.SPRecover || damageType == DamageType.SPDamage) type = " " + TUFFSettings.SPShortText;
            else if (damageType == DamageType.TPRecover) type = " " + TUFFSettings.TPShortText;
            text.text = $"{label}{LISAUtility.IntToString(value)}{type}";
        }
        protected virtual void SetVulnerabilityText(VulnerabilityType vulType)
        {
            text.text = BattleHUD.GetVulnerabilityText(vulType);
        }
        protected virtual void SetHitTextColor()
        {
            Color color;
            switch(hitInfo.damageType)
            {
                case DamageType.HPDamage:
                    if (hitInfo.hitType == HitType.PhysicalAttack)
                        color = TUFFSettings.HPDamageTextColor;
                    else if (hitInfo.hitType == HitType.SpecialAttack)
                        color = TUFFSettings.HPDamageSpecialTextColor;
                    else color = TUFFSettings.HPDamageTextColor;
                    break;
                case DamageType.SPDamage:
                    color = TUFFSettings.SPDamageTextColor;
                    break;
                case DamageType.HPRecover:
                    color = TUFFSettings.HPRecoverTextColor;
                    break;
                case DamageType.SPRecover:
                    color = TUFFSettings.SPRecoverTextColor;
                    break;
                case DamageType.TPRecover:
                    color = TUFFSettings.TPRecoverTextColor;
                    break;
                default:
                    color = TUFFSettings.HPDamageTextColor;
                    break;
            }
            color *= colorTint;
            text.color = color;
        }
        protected virtual void SetRegenColor(DamageType damageType)
        {
            Color color;
            switch (damageType)
            {
                case DamageType.SPDamage:
                    color = TUFFSettings.SPDamageTextColor;
                    break;
                case DamageType.HPRecover:
                    color = TUFFSettings.HPRecoverTextColor;
                    break;
                case DamageType.SPRecover:
                    color = TUFFSettings.SPRecoverTextColor;
                    break;
                case DamageType.TPRecover:
                    color = TUFFSettings.TPRecoverTextColor;
                    break;
                default:
                    color = TUFFSettings.HPDamageTextColor;
                    break;
            }
            color *= colorTint;
            text.color = color;
        }
        protected virtual void SetStateColor(State state, bool removal)
        {
            Color color;
            if(removal) color = TUFFSettings.stateRemovalTextColor;
            else
            {
                color = (state.stateType == StateType.Permanent ? TUFFSettings.permanentStateApplyTextColor : TUFFSettings.stateApplyTextColor);
            }
            color *= colorTint;
            text.color = color;
        }
        protected virtual void SetVulnerabilityColor(VulnerabilityType vulType)
        {
            text.color = BattleHUD.GetVulnerabilityColor(vulType) * colorTint;
        }
        public virtual void DestroyDisplay()
        {
            group.RemoveFirst();
            Destroy(gameObject);
        }
    }

}
