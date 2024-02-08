using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TUFF.TUFFEditor
{
    public static class DatabaseDropdownDrawer 
    {
        private static void DrawDropdown(Object[] loadedObjs, ref int popupValue, SerializedProperty target)
        {
            string[] options;
            int[] values;
            GetOptionsAndValues(loadedObjs, out options, out values);
            //Button
            EditorGUILayout.BeginHorizontal();
            if (popupValue >= loadedObjs.Length) popupValue = loadedObjs.Length - 1;
            popupValue = EditorGUILayout.IntPopup("Assign From Resources", popupValue, options, values);
            if (GUILayout.Button(new GUIContent("Assign"), GUILayout.Width(50)) && loadedObjs.Length > 0)
            {
                target.objectReferenceValue = loadedObjs[popupValue];
            }
            EditorGUILayout.EndHorizontal();
        }
        private static Object DrawDropdown(Object[] loadedObjs, ref int popupValue, Object originalValue)
        {
            Object newValue = originalValue;
            string[] options;
            int[] values;
            GetOptionsAndValues(loadedObjs, out options, out values);
            //Button
            EditorGUILayout.BeginHorizontal();
            if (popupValue >= loadedObjs.Length) popupValue = loadedObjs.Length - 1;
            popupValue = EditorGUILayout.IntPopup("Assign From Resources", popupValue, options, values);
            if (GUILayout.Button(new GUIContent("Assign"), GUILayout.Width(50)) && loadedObjs.Length > 0)
            {
                newValue = loadedObjs[popupValue];
            }
            EditorGUILayout.EndHorizontal();
            return newValue;
        }
        private static void GetOptionsAndValues(Object[] loadedObjs, out string[] options, out int[] values)
        {
            options = new string[loadedObjs.Length];
            values = new int[loadedObjs.Length];
            for (int i = 0; i < loadedObjs.Length; i++)
            {
                options[i] = loadedObjs[i].name;
                values[i] = i;
            }
        }
        public static void DrawBattlesDropdown(ref int popupValue, SerializedProperty target)
        {
            Battle[] battles = Resources.LoadAll<Battle>(DatabaseLoader.battlePath);
            DrawDropdown(battles, ref popupValue, target);
        }
        public static Battle DrawBattlesDropdown(ref int popupValue, Battle originalValue)
        {
            Battle[] battles = Resources.LoadAll<Battle>(DatabaseLoader.battlePath);
            return (Battle)DrawDropdown(battles, ref popupValue, originalValue);
        }
        public static void DrawAnimationsDropdown(ref int popupValue, SerializedProperty target)
        {
            BattleAnimation[] anims = Resources.LoadAll<BattleAnimation>(DatabaseLoader.animationsPath);
            DrawDropdown(anims, ref popupValue, target);
        }
        public static BattleAnimation DrawAnimationsDropdown(ref int popupValue, BattleAnimation originalValue)
        {
            BattleAnimation[] anims = Resources.LoadAll<BattleAnimation>(DatabaseLoader.animationsPath);
            return (BattleAnimation)DrawDropdown(anims, ref popupValue, originalValue);
        }
    }
}