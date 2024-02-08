using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TUFF.TUFFEditor
{
    [CustomPropertyDrawer(typeof(ShowDialogueAction))]
    public class ShowDialogueActionPD : EventActionPD
    {
        public override void InspectorGUIContent()
        {
            EditorGUILayout.PropertyField(targetProperty.FindPropertyRelative("dialogue"));
        }
        public override void SummaryGUI(Rect position)
        {
            EditorGUI.LabelField(position, GetSummaryText());
        }
        private string GetSummaryText()
        {
            var action = targetObject as ShowDialogueAction;
            if (action.dialogue == null) return "Empty Dialogue";
            if (action.dialogue.sentences == null) return "Empty Dialogue";
            if (action.dialogue.sentences.Length <= 0) return "Empty Dialogue";
            string origin = "";
            string sentence = "";
            if (action.dialogue.textboxType == TextboxType.Fixed) origin = $"Fixed ({(action.dialogue.fixedTextboxPosition)}):";
            else if (action.dialogue.originType == FieldOriginType.FromScene)
            {
                origin = $"({action.dialogue.sentences.Length}){(action.dialogue.origin ? action.dialogue.origin.name : "Origin not assigned") }:";
            }
            else origin = $"({action.dialogue.sentences.Length}){ action.dialogue.persistentOrigin }:";

            if (action.dialogue.sentences[0].sentenceTextType == SentenceTextType.Localized)
            {
                LISAUtility.CheckLocaleIsNotNull();
                sentence = LISAUtility.GetLocalizedDialogueText(action.dialogue.sentences[0].key);
            }
            else sentence = action.dialogue.sentences[0].text;
            return $"{origin} { TUFFTextParser.ParseText(sentence) }";
        }
    }
}
