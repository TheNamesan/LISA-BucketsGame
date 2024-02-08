using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Localization.Settings;
using UnityEditor.Localization.UI;

namespace TUFF.TUFFEditor
{
    [CustomPropertyDrawer(typeof(DialogueSentence))]
    public class DialogueSentencePD : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float lines = 0f;
            float additionalHeight = 0;
            if (property.isExpanded)
            {
                lines = 4f;
                var sentenceTextType = property.FindPropertyRelative("sentenceTextType");
                if ((SentenceTextType)sentenceTextType.enumValueIndex == SentenceTextType.Localized)
                {
                    lines += 2f;
                    additionalHeight = 50f;
                    additionalHeight += 6f;
                }
                else if ((SentenceTextType)sentenceTextType.enumValueIndex == SentenceTextType.Simple)
                {
                    additionalHeight = (EditorGUI.GetPropertyHeight(property.FindPropertyRelative("text")) + EditorGUIUtility.standardVerticalSpacing) * 2f;
                    additionalHeight += 6f;
                    lines += 2f;
                }
                if(property.FindPropertyRelative("voicebank").isExpanded)
                {
                    lines += 2;
                }
            }
            return (EditorGUIUtility.singleLineHeight - EditorGUIUtility.standardVerticalSpacing) +
                (EditorGUIUtility.singleLineHeight * lines) +
                (EditorGUIUtility.standardVerticalSpacing) + additionalHeight;
        }
        public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label)
        {
            position.height = 20f;
            label = EditorGUI.BeginProperty(position, label, prop);
            prop.isExpanded = EditorGUI.Foldout(position, prop.isExpanded, label);
            if (prop.isExpanded)
            {
                Rect rect = EditorGUI.IndentedRect(position);
                AddLine(ref rect);
                var sentenceTextType = prop.FindPropertyRelative("sentenceTextType");
                EditorGUI.PropertyField(rect, sentenceTextType);
                if ((SentenceTextType)sentenceTextType.enumValueIndex == SentenceTextType.Localized)
                {
                    AddLine(ref rect);
                    var key = prop.FindPropertyRelative("key");
                    key.stringValue = EditorGUI.TextField(rect, "Key", key.stringValue);
                    AddLine(ref rect);
                    var locale = LocalizationSettings.SelectedLocale;
                    EditorGUI.LabelField(rect, $"Preview Localization [{(locale != null ? locale.LocaleName : "No Active Locale")}]");
                    float layoutX = rect.x + rect.width * 0.6f;
                    float layoutWidth = rect.width * 0.4f;
                    string[] options = new string[LocalizationSettings.AvailableLocales.Locales.Count];
                    int[] values = new int[LocalizationSettings.AvailableLocales.Locales.Count];
                    for (int i = 0; i < options.Length; i++)
                    {
                        options[i] = LocalizationSettings.AvailableLocales.Locales[i].LocaleName;
                        values[i] = i;
                    }
                    LISAUtility.CheckLocaleIsNotNull();
                    if (LISAUtility.GetSelectedLocaleIndex() >= 0)
                        LISAUtility.SelectLocale(EditorGUI.IntPopup(new Rect(layoutX, rect.y, layoutWidth * 0.5f, 20), LISAUtility.GetSelectedLocaleIndex(), options, values));
                    if (GUI.Button(new Rect(layoutX + layoutWidth * 0.5f, rect.y, layoutWidth * 0.5f, 20), "Open Tables", EditorStyles.miniButton))
                    {
                        LocalizationTablesWindow.ShowWindow();
                    }
                    var localizedText = LISAUtility.GetLocalizedDialogueText(key.stringValue);
                    AddLine(ref rect);
                    rect.height = 50f;
                    GUI.enabled = false;
                    EditorGUI.TextArea(rect, localizedText);
                    GUI.enabled = true;
                    rect.y += 50f;
                    rect.height = 20f;
                }
                else if ((SentenceTextType)sentenceTextType.enumValueIndex == SentenceTextType.Simple)
                {
                    AddLine(ref rect);
                    var text = prop.FindPropertyRelative("text");
                    float textHeight = EditorGUI.GetPropertyHeight(text) + EditorGUIUtility.standardVerticalSpacing;
                    rect.height = textHeight;
                    EditorGUI.PropertyField(rect, text);

                    // Preview
                    rect.y += textHeight;
                    rect.height = 20f;
                    var locale = LocalizationSettings.SelectedLocale;
                    EditorGUI.LabelField(rect, $"Preview [{(locale != null ? locale.LocaleName : "No Active Locale")}]");
                    float layoutX = rect.x + rect.width * 0.6f;
                    float layoutWidth = rect.width * 0.4f;
                    string[] options = new string[LocalizationSettings.AvailableLocales.Locales.Count];
                    int[] values = new int[LocalizationSettings.AvailableLocales.Locales.Count];
                    for (int i = 0; i < options.Length; i++)
                    {
                        options[i] = LocalizationSettings.AvailableLocales.Locales[i].LocaleName;
                        values[i] = i;
                    }
                    
                    if (LISAUtility.CheckLocaleIsNotNull())
                        LISAUtility.SelectLocale(EditorGUI.IntPopup(new Rect(layoutX, rect.y, layoutWidth * 0.5f, 20), LISAUtility.GetSelectedLocaleIndex(), options, values));
                    if (GUI.Button(new Rect(layoutX + layoutWidth * 0.5f, rect.y, layoutWidth * 0.5f, 20), "Open Tables", EditorStyles.miniButton))
                    {
                        LocalizationTablesWindow.ShowWindow();
                    }
                    AddLine(ref rect);
                    rect.height = textHeight;
                    GUI.enabled = false;
                    var parsedText = TUFFTextParser.ParseText(text.stringValue);
                    EditorGUI.TextArea(rect, parsedText);
                    GUI.enabled = true;
                    rect.y += textHeight;
                    // End

                    rect.height = 20f;
                }
                AddLine(ref rect, 2);
                //EditorGUI.LabelField(rect, "Override Properties", EditorStyles.boldLabel);
                var voicebank = prop.FindPropertyRelative("voicebank");
                voicebank.isExpanded = EditorGUI.Foldout(rect, voicebank.isExpanded, "Override Properties");
                AddLine(ref rect, 2);
                if(voicebank.isExpanded)
                {
                    EditorGUI.PropertyField(rect, voicebank);
                    AddLine(ref rect, 2);
                    EditorGUI.PropertyField(rect, prop.FindPropertyRelative("textSpeed"));
                    AddLine(ref rect, 2);
                }
            }
            EditorGUI.EndProperty();
            prop.serializedObject.ApplyModifiedProperties();
        }
        private void AddLine(ref Rect position, float spaceMult = 1)
        {
            position.y += (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing * spaceMult);
        }
        
    }
}

