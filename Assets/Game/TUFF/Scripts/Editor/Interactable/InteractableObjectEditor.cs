using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace TUFF.TUFFEditor
{
    [CustomEditor(typeof(InteractableObject)), CanEditMultipleObjects]
    public class InteractableObjectEditor : Editor
    {
        bool verifiedPersistentID = false;
        private ReorderableList list;
        public override void OnInspectorGUI()
        {
            var io = target as InteractableObject;
            GUI.enabled = false;
            SerializedProperty prop = serializedObject.FindProperty("m_Script");
            EditorGUILayout.PropertyField(prop, true, new GUILayoutOption[0]);
            GUI.enabled = true;

            var id = serializedObject.FindProperty("persistentID");
            if(!verifiedPersistentID && !Application.isPlaying)
            {
                if(PersistentInteractableList.VerifyPersistentIDExists(io, true) < 0)
                {
                    if (id.intValue >= 0)
                    {
                        Debug.LogWarning($"Could not find a matching persistent ID for {io.gameObject.name}. Resetting ID.");
                        Undo.RecordObject(io, "Reverted ID in object");
                        id.intValue = -1;
                        Undo.SetCurrentGroupName($"Reset Persistent ID for {io.gameObject.name}");
                        Undo.CollapseUndoOperations(Undo.GetCurrentGroup());
                    }
                }
                else if(id.intValue < 0)
                {
                    Debug.Log($"Interactable {io.gameObject.name} already has a persistent ID. Reassigning ID to component.");
                }
                verifiedPersistentID = true;
            }
            EditorGUILayout.BeginHorizontal();
            GUI.enabled = false;
            EditorGUILayout.PropertyField(id, GUILayout.ExpandWidth(true));
            GUI.enabled = true;
            if(id.intValue < 0)
            {
                if (GUILayout.Button(new GUIContent("Make Persistent"), EditorStyles.miniButton))
                {
                    PersistentInteractableList.AddPersistentInteractable(io);
                    id.intValue = io.persistentID;
                }
            }
            else
            {
                if (GUILayout.Button(new GUIContent("Remove Persistent ID"), EditorStyles.miniButton))
                {
                    PersistentInteractableList.RemovePersistentInteractable(io);
                    id.intValue = io.persistentID;
                }
            }
            if(GUILayout.Button(new GUIContent("Interactables List"), EditorStyles.miniButton))
            {
                EditorGUIUtility.PingObject(PersistentInteractableList.instance);
            }
            EditorGUILayout.EndHorizontal();

            GUI.enabled = false;
            EditorGUILayout.IntField(new GUIContent("Current Switch"), io.currentSwitch);
            GUI.enabled = true;

            if (id.intValue < 0)
            {
                string helpBoxContent = "Interactable has no persistent ID. Interactables without a pesistent ID will have their switches reset when changing scenes.";
                EditorGUILayout.HelpBox(helpBoxContent, MessageType.Warning);
            }

            EditorGUI.BeginChangeCheck();
            if (list == null) GetList();
            //EditorGUILayout.Foldout("");
            list.DoLayoutList();
            if (EditorGUI.EndChangeCheck())
            {
                EventActionListWindow.ResetReferences();
            }
            serializedObject.ApplyModifiedProperties();
        }
        private void GetList()
        {
            list = new ReorderableList(serializedObject, serializedObject.FindProperty("triggerEvents"), true, false, true, true);
            list.drawElementCallback = DrawListItems;
            list.elementHeightCallback = GetElementHeight;
            list.onAddCallback = OnAddCallback;
            list.onRemoveCallback = OnRemoveCallback;
        }
        float GetElementHeight(int index)
        {
            var target = LISAEditorUtility.GetTargetObjectOfProperty(list.serializedProperty) as InteractableEvent[];
            if (target.Length == 0) return 0f;
            var element = list.serializedProperty.GetArrayElementAtIndex(index);
            var elementHeight = EditorGUI.GetPropertyHeight(element);
            return elementHeight + EditorGUIUtility.standardVerticalSpacing;
        }
        void DrawListItems(Rect rect, int index, bool isActive, bool isFocused)
        {
            rect.height = 20f;
            SerializedProperty prop = list.serializedProperty.GetArrayElementAtIndex(index);
            //if (prop != null)
            //    ActionListPD.DrawPreview(rect, prop, new GUIContent("LABEL"));
            EditorGUI.PropertyField(rect, prop, true);
        }
        void OnAddCallback(ReorderableList rl)
        {
            var index = rl.serializedProperty.arraySize;
            rl.serializedProperty.arraySize++;
            rl.index = index;
            var element = rl.serializedProperty.GetArrayElementAtIndex(index);
            Debug.Log(index);
            element.FindPropertyRelative("triggerType").enumValueIndex = 0;
            var actionList = element.FindPropertyRelative("actionList");
            actionList.FindPropertyRelative("content").arraySize = 0;
        }
        void OnRemoveCallback(ReorderableList rl)
        {
            //var index = rl.index;
            //var element = rl.serializedProperty.GetArrayElementAtIndex(index);
            //var actionList = LISAEditorUtility.GetTargetObjectOfProperty(element.FindPropertyRelative("actionList")) as ActionList;
            //EventActionListWindow.RemoveAllEvents(actionList);

            //if(PrefabUtility.IsPartOfAnyPrefab(serializedObject.targetObject))
            //{
            //    PrefabUtility.RevertPropertyOverride(element, InteractionMode.AutomatedAction); // Prevents garbage data from accumulating in the scene file if is IO a prefab
            //}
            ReorderableList.defaultBehaviours.DoRemoveButton(rl);
            
        }

        private void OnDisable()
        {

        }
    }
}

