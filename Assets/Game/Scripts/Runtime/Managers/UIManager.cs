using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BucketsGame
{
    public class UIManager : MonoBehaviour
    {
        public Image focusFill;
        private void Update()
        {
            UpdateBar();
        }
        private void UpdateBar()
        {
            if (focusFill)
            {
                focusFill.fillAmount = GameManager.instance.FocusFill();
            }
        }
    }
}
