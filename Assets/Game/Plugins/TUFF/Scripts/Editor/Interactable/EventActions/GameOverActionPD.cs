using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TUFF.TUFFEditor
{
    [CustomPropertyDrawer(typeof(GameOverAction))]
    public class GameOverActionPD : EventActionPD
    {
        public override void SummaryGUI(Rect position)
        {
            EditorGUI.LabelField(position, "Set Game Over");
        }
    }
}

