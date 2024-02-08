using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace TUFF.TUFFEditor
{
    [CustomPropertyDrawer(typeof(CombatGraphics))]
    public class CombatGraphicsPD : PropertyDrawer
    {
        public const int maxFaceDisplays = 10;
        public int currentPage = 0;
        private ReorderableList list;
        private State[] states = new State[0];
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            int lines = 1;
            float separation = 0f;
            if (property.isExpanded)
            {
                lines += 3;
                lines += states.Length;
            }
            return separation + 20f * lines;
        }
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            position.height = 20f;
            property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, label, true);
            position.y += 20f;
            if (property.isExpanded)
            {
                position.x += 15f;
                position.width -= 15f;

                EditorGUI.PropertyField(position, property.FindPropertyRelative("defaultFaceGraphic"));
                position.y += 20f;
                EditorGUI.PropertyField(position, property.FindPropertyRelative("KOFaceGraphic"));
                position.y += 20f;

                var stateFaces = property.FindPropertyRelative("stateFaces");

                states = Resources.LoadAll<State>("Database/10States");
                if (stateFaces.arraySize != states.Length) stateFaces.arraySize = states.Length; //Adjust List size if total states have not been updated
                if (list == null) GetList(property.serializedObject, stateFaces);
                list.DoList(position);
                position.y += 20f;

                position.x -= 15f;
                position.width += 15f;
            }
            property.serializedObject.ApplyModifiedProperties();
        }
        private void GetList(SerializedObject serializedObject, SerializedProperty elements)
        {
            list = new ReorderableList(serializedObject, elements, true, false, false, false);
            list.drawElementCallback = DrawListItems;
            list.elementHeightCallback = GetElementHeight;
        }
        float GetElementHeight(int index)
        {
            var element = list.serializedProperty.GetArrayElementAtIndex(index);
            var elementHeight = EditorGUI.GetPropertyHeight(element);
            return elementHeight;
        }
        void DrawListItems(Rect rect, int index, bool isActive, bool isFocused)
        {
            rect.height = 20f;
            SerializedProperty prop = list.serializedProperty.GetArrayElementAtIndex(index);
            EditorGUI.PropertyField(rect, prop, new GUIContent($"{index} {states[index].GetName()}"));
            LISAEditorUtility.DrawSprite(new Rect(rect.x + EditorGUIUtility.labelWidth - 20f, rect.y, 20f, 20f), states[index].icon);
        }
    }

}
