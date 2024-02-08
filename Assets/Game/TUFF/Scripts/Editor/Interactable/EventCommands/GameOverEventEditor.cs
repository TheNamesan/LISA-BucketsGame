using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TUFF.TUFFEditor
{
    [CustomEditor(typeof(GameOverEvent)), CanEditMultipleObjects]
    public class GameOverEventEditor : EventCommandEditor
    {
        public override void SummaryGUI(Rect position)
        {
            EditorGUI.LabelField(position, "Set Game Over");
        }
    }
}
