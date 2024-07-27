using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace TUFF.TUFFEditor
{
    [CustomPropertyDrawer(typeof(WeaponTypeList))]
    public class WeaponTypeListPD : PropertyDrawer
    {
        public ReorderableList list;
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var list = property.FindPropertyRelative(nameof(WeaponTypeList.weaponTypes));
            float height = EditorGUI.GetPropertyHeight(list);
            return height;
        }
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            position.height = 20f;
            var array = property.FindPropertyRelative(nameof(WeaponTypeList.weaponTypes));
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
            var weaponTypes = TUFFSettings.weaponTypes;
            string[] options = new string[weaponTypes.Count];
            int[] values = new int[weaponTypes.Count];
            for (int i = 0; i < weaponTypes.Count; i++)
            {
                options[i] = weaponTypes[i].GetName();
                values[i] = i;
            }
            list.serializedProperty.GetArrayElementAtIndex(index).intValue = 
                EditorGUI.IntPopup(rect, "Weapon Type", list.serializedProperty.GetArrayElementAtIndex(index).intValue, options, values);
        }
        private float GetElementHeight(int index)
        {
            return 20f;
        }
    }
}
