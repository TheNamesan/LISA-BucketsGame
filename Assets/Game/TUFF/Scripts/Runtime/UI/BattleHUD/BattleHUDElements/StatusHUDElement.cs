using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace TUFF
{
    public class StatusHUDElement : MonoBehaviour
    {
        [Header("References")]
        public TMP_Text nameText;
        public TMP_Text descText;
        public ActiveStatesHUDElement activeStatesHUDElement;
        public UIButton uiButton;
        [System.NonSerialized] public ActiveState activeState = null;

        public void AssignData(ActiveState activeState)
        {
            this.activeState = activeState;
            if (this.activeState == null) return;
            nameText.text = activeState.state.GetName();
            descText.text = activeState.state.GetDescription();
            activeStatesHUDElement?.UpdateStateInfo(activeState);
        }
    }
}

