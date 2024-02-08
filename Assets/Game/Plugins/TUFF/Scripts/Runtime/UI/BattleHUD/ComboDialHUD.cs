using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TUFF
{
    public class ComboDialHUD : MonoBehaviour
    {
        public GameObject dialInfoElementPrefab;
        public GameObject dialInputPrefab;
        public ComboDialInfoElement dialSuccessPrefab;

        public Transform mask;
        public Transform inputsInfo;
        public Transform dialInput;

        [HideInInspector] public ComboDial comboDial;

        public void InitializeComboDialHUD(ComboDial comboDial)
        {
            ResetInputsInfo();
            ResetDialInput();
            this.comboDial = comboDial;
            SetInputsInfo();
        }
        public void ResetInputsInfo()
        {
            foreach (Transform child in inputsInfo)
            {
                Destroy(child.gameObject);
            }
            ForceRebuild();
        }
        public void ResetDialInput()
        {
            foreach (Transform child in dialInput)
            {
                Destroy(child.gameObject);
            }
        }
        public virtual void SetInputsInfo()
        {
            if (comboDial.QSkill != null) InstantiateInfoElement(comboDial.QSkill, inputsInfo);
            if (comboDial.WSkill != null) InstantiateInfoElement(comboDial.WSkill, inputsInfo);
            if (comboDial.ASkill != null) InstantiateInfoElement(comboDial.ASkill, inputsInfo);
            if (comboDial.SSkill != null) InstantiateInfoElement(comboDial.SSkill, inputsInfo);
            if (comboDial.DSkill != null) InstantiateInfoElement(comboDial.DSkill, inputsInfo);
            Debug.Log(inputsInfo.childCount);
            ForceRebuild();
        }
        public void AddInputIcon(Sprite sprite)
        {
            var inputGO = Instantiate(dialInputPrefab, dialInput);
            var input = inputGO.GetComponent<Image>();
            if (input == null) inputGO.AddComponent<Image>();
            input.sprite = sprite;
        }
        public void AddSuccessInput(Skill skill)
        {
            var successGO = Instantiate(dialSuccessPrefab.gameObject, dialInput);
            var infoElement = successGO.GetComponent<ComboDialInfoElement>();
            if (infoElement == null) successGO.AddComponent<ComboDialInfoElement>();
            infoElement.UpdateInfo(skill);
        }
        protected void InstantiateInfoElement(Skill skill, Transform parent)
        {
            var elementGO = Instantiate(dialInfoElementPrefab, parent);
            var infoElement = elementGO.GetComponent<ComboDialInfoElement>();
            if (infoElement == null) return;
            infoElement.UpdateInfo(skill);
        }
        private void ForceRebuild()
        {
            RectTransform rectTransform = mask.GetComponent<RectTransform>();
            LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
            LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
        }
    }

}
