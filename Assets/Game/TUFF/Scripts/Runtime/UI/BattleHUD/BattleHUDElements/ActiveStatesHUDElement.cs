using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace TUFF
{
    public class ActiveStatesHUDElement : MonoBehaviour
    {
        [Header("References")]
        public Image image;
        public TMP_Text countText;
        [HideInInspector] public ActiveState activeState;
        
        public void UpdateStateInfo(ActiveState activeState)
        {
            this.activeState = activeState;
            image.sprite = activeState.state.icon;
            countText.text = LISAUtility.IntToString(activeState.remainingTurns);
                countText.gameObject.SetActive(activeState.ShouldDisplayCount());
        }
    }
}

