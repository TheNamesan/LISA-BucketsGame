using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Localization.Settings;

namespace TUFF.TUFFEditor
{
    [CustomEditor(typeof(ShowDialogueEvent)), CanEditMultipleObjects]
    public class ShowDialogueEventEditor : EventCommandEditor
    {
        public override void InspectorGUIContent()
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("dialogue"));
        }

        public override void OnEditorInstantiate()
        {
            var eventCommand = target as ShowDialogueEvent;
            eventCommand.dialogue.baseVoicebank = TUFFSettings.defaultVoicebank;
        }
        public override void SummaryGUI(Rect position)
        {
            EditorGUI.LabelField(position, GetSummaryText());
        }
        private string GetSummaryText()
        {
            var eventCommand = target as ShowDialogueEvent;
            if (eventCommand.dialogue == null) return "Empty Dialogue";
            if (eventCommand.dialogue.sentences == null) return "Empty Dialogue";
            if (eventCommand.dialogue.sentences.Length <= 0) return "Empty Dialogue";
            string origin = "";
            string sentence = "";
            if (eventCommand.dialogue.textboxType == TextboxType.Fixed) origin = $"Fixed ({(eventCommand.dialogue.fixedTextboxPosition)}):";
            else if (eventCommand.dialogue.originType == FieldOriginType.FromScene)
            {
                origin = $"({eventCommand.dialogue.sentences.Length}){(eventCommand.dialogue.origin ? eventCommand.dialogue.origin.name : "Origin not assigned") }:";
            }
            else origin = $"({eventCommand.dialogue.sentences.Length}){ eventCommand.dialogue.persistentOrigin }:";
            
            if (eventCommand.dialogue.sentences[0].sentenceTextType == SentenceTextType.Localized)
            {
                LISAUtility.CheckLocaleIsNotNull();
                sentence = LISAUtility.GetLocalizedDialogueText(eventCommand.dialogue.sentences[0].key);
            }
            else sentence = eventCommand.dialogue.sentences[0].text;
            return $"{origin} { TUFFTextParser.ParseText(sentence) }";
        }
    }
}
