using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace TUFF
{
    public class ComboDialInfoElement : MonoBehaviour
    {
        public TMP_Text text;
        public Image image;
        [HideInInspector] public Skill skillRef;

        public void UpdateInfo(Skill skill)
        {
            skillRef = skill;
            text.text = skill.GetName();
            image.sprite = skillRef.icon;
        }
    }
}

