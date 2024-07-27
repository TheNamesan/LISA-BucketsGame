using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace TUFF.TUFFEditor
{
    [CustomEditor(typeof(Battle))]
    public class BattleEditor : Editor
    {
        private ReorderableList list = null;

        SerializedProperty battleEvents = null;
        SerializedProperty autoPlayBGM = null;
        SerializedProperty bgm = null;
        public void OnEnable()
        {
            battleEvents = serializedObject.FindProperty("battleEvents");
            autoPlayBGM = serializedObject.FindProperty("autoPlayBGM");
            bgm = serializedObject.FindProperty("bgm");
        }
        override public void OnInspectorGUI()
        {
            var battle = target as Battle;

            if (GUILayout.Button("Test Battle", (EditorStyles.miniButton)))
            {
                TUFFWizard.TestBattle(battle);
            }
            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("background"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("initialEnemies"));
            if (GUILayout.Button("Instantiate Enemy Graphic", (EditorStyles.miniButton)))
            {
                if (battle.gameObject.scene.name != null)
                {
                    TUFFWizard.CreateItem(TUFFSettings.enemyGraphicPrefab, battle.transform);
                }
                else Debug.LogWarning("Can only add Enemy Graphics while the prefab's open.");
            }
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Battle Events", EditorStyles.boldLabel);

            battleEvents.isExpanded = EditorGUILayout.Foldout(battleEvents.isExpanded, battleEvents.displayName);
            if (battleEvents.isExpanded)
            {
                EditorGUI.BeginChangeCheck();
                if (list == null) GetList();
                list.DoLayoutList();
                if (EditorGUI.EndChangeCheck())
                {
                    EventActionListWindow.ResetReferences();
                }
            }

            EditorGUILayout.PropertyField(autoPlayBGM);
            if (autoPlayBGM.boolValue) EditorGUILayout.PropertyField(bgm);

            

            serializedObject.ApplyModifiedProperties();
        }
        private void GetList()
        {
            list = new ReorderableList(serializedObject, battleEvents, true, false, true, true);
            list.drawElementCallback = DrawListItems;
            list.elementHeightCallback = GetElementHeight;
            list.onAddCallback = OnAddCallback;
        }
        void DrawListItems(Rect rect, int index, bool isActive, bool isFocused)
        {
            rect.height = 20f;
            SerializedProperty prop = list.serializedProperty.GetArrayElementAtIndex(index);
            EditorGUI.PropertyField(rect, prop, true);
        }
        float GetElementHeight(int index)
        {
            var target = LISAEditorUtility.GetTargetObjectOfProperty(list.serializedProperty) as BattleEvent[];
            if (target.Length == 0) return 0f;
            var element = list.serializedProperty.GetArrayElementAtIndex(index);
            var elementHeight = EditorGUI.GetPropertyHeight(element);
            return elementHeight + EditorGUIUtility.standardVerticalSpacing;
        }
        void OnAddCallback(ReorderableList rl)
        {
            Debug.Log(rl.serializedProperty);
            var index = rl.serializedProperty.arraySize;
            rl.serializedProperty.arraySize++;
            rl.index = index;
            var element = rl.serializedProperty.GetArrayElementAtIndex(index);
            Debug.Log(index);
            var actionListContent = element.FindPropertyRelative("actionList.content");
            actionListContent.arraySize = 0;
        }
    }
}

