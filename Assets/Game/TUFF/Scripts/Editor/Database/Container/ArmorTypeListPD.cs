using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace TUFF.TUFFEditor
{
    [CustomPropertyDrawer(typeof(ArmorTypeList))]
    public class ArmorTypeListPD : PropertyDrawer
    {
        public ReorderableList list;
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var list = property.FindPropertyRelative(nameof(ArmorTypeList.armorTypes));
            float height = EditorGUI.GetPropertyHeight(list);
            return height;
        }
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            position.height = 20f;
            var array = property.FindPropertyRelative(nameof(ArmorTypeList.armorTypes));
            if (list == null) list = GetList(array);
            array.isExpanded = EditorGUI.Foldout(position, array.isExpanded, label, true);
            position.y += 20f;
            if (list != null && array.isExpanded)
            {
                list.DoList(position);
                position.y += list.GetHeight();
            }
            property.serializedObject.ApplyModifiedProperties();
        }
        public ReorderableList GetList(SerializedProperty arrayProperty)
        {
            if (arrayProperty == null) return null;
            var list = new ReorderableList(arrayProperty.serializedObject, arrayProperty, true, false, true, true);
            list.drawElementCallback = DrawListItems;
            list.elementHeightCallback = GetElementHeight;
            return list;
        }
        private void DrawListItems(Rect rect, int index, bool isActive, bool isFocused)
        {
            rect.height = 20f;
            rect.y += 1f;
            var armorTypes = TUFFSettings.armorTypes;
            string[] options = new string[armorTypes.Count];
            int[] values = new int[armorTypes.Count];
            for (int i = 0; i < armorTypes.Count; i++)
            {
                options[i] = armorTypes[i].GetName();
                values[i] = i;
            }
            list.serializedProperty.GetArrayElementAtIndex(index).intValue =
                EditorGUI.IntPopup(rect, "Armor Type", list.serializedProperty.GetArrayElementAtIndex(index).intValue, options, values);
        }
        private float GetElementHeight(int index)
        {
            return 20f;
        }
    }
}
