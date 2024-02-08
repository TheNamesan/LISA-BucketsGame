using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TUFF
{
    public class HitDisplayGroup : MonoBehaviour
    {
        public GameObject hitDisplayPrefab;
        public Targetable target;
        public BattleHUD hudCallback;
        [SerializeField] private List<HitDisplay> hitDisplays = new List<HitDisplay>();
        public void CreateGroup(Targetable target, BattleHUD hudCallback)
        {
            this.target = target;
            this.hudCallback = hudCallback;
            hitDisplays = new List<HitDisplay>();
            var rect = GetComponent<RectTransform>();
            var pos = target.imageReference.GetOverlayPosition();
            Debug.Log(pos);
            rect.position = pos;
            rect.anchoredPosition += Vector2.up * 250f;
        }
        public void AddHitDisplay(BattleAnimationEvent hitInfo, int value, bool isCrit = false)
        {
            var hitDisplay = InstantiateNewDisplay();
            hitDisplay.InitializeHitDisplay(this, hitInfo);
            hitDisplay.DisplayHitValue(value, isCrit, hitInfo.ElementIndex);
        }
        public void AddRegenDisplay(int value, DamageType damageType)
        {
            var hitDisplay = InstantiateNewDisplay();
            hitDisplay.InitializeHitDisplay(this);
            hitDisplay.DisplayRegen(value, damageType);
        }
        public void AddMissDisplay()
        {
            var hitDisplay = InstantiateNewDisplay();
            hitDisplay.InitializeHitDisplay(this);
            hitDisplay.DisplayMiss();
        }
        public void AddStateDisplay(State state, bool removal = false)
        {
            var hitDisplay = InstantiateNewDisplay();
            hitDisplay.InitializeHitDisplay(this);
            hitDisplay.DisplayState(state, removal);
        }
        public void AddVulnerabilityDisplay(int elementIndex, VulnerabilityType vulType)
        {
            var hitDisplay = InstantiateNewDisplay();
            hitDisplay.InitializeHitDisplay(this);
            hitDisplay.DisplayVulnerability(elementIndex, vulType);
        }
        protected virtual HitDisplay InstantiateNewDisplay()
        {
            var hitDisplayGO = Instantiate(hitDisplayPrefab, transform);
            var hitDisplay = hitDisplayGO.GetComponent<HitDisplay>();
            hitDisplays.Add(hitDisplay);
            return hitDisplay;
        }
        public void RemoveFirst()
        {
            if (hitDisplays.Count <= 0) return;
            hitDisplays.RemoveAt(0);
            if(hitDisplays.Count <= 0)
            {
                hudCallback.RemoveHitDisplayGroup(this);
                Destroy(gameObject);
            }
        }
    }
}

