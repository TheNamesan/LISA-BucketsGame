using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Linq;

namespace TUFF.TUFFEditor
{
    [CustomEditor(typeof(ConditionalBranchEvent)), CanEditMultipleObjects]
    public class ConditionalBranchEventEditor : EventCommandEditor
    {
        private ReorderableList list;
        private SerializedProperty branches;
        private List<EventListEditors> eventListEditorsList;
        private List<BranchContent> previousBranches;
        private static bool queueReset = false;
        private void GetList()
        {
            branches = serializedObject.FindProperty("branches");
            list = new ReorderableList(serializedObject, branches, true, true, true, true);
            list.drawElementCallback = DrawListItems;
            list.drawHeaderCallback = DrawHeader;
            list.elementHeightCallback = GetElementHeight;
        }
        public override void InspectorGUIContent()
        {
            if (list == null)
            {
                GetList();
            }
            var eventCommand = target as ConditionalBranchEvent;
            EditorGUILayout.LabelField("WIP. Do not use.");
            EditorGUI.BeginChangeCheck();
            list.DoLayoutList();
            if(EditorGUI.EndChangeCheck())
            {
                queueReset = true;
            }
        }
        void DrawListItems(Rect rect, int index, bool isActive, bool isFocused)
        {
            rect.height = 20f;
            SerializedProperty prop = list.serializedProperty.GetArrayElementAtIndex(index);
            var condition = prop.FindPropertyRelative("condition");
            EditorGUI.PropertyField(rect, condition);
            /*if(condition.boolValue)
            {
                AddLine(ref rect);
                EditorGUI.PropertyField(rect, prop.FindPropertyRelative("condition"));
            }*/
        }
        float GetElementHeight(int index)
        {
            var target = LISAEditorUtility.GetTargetObjectOfProperty(list.serializedProperty) as List<BranchContent>;
            if (target.Count == 0) return 0f;
            var element = list.serializedProperty.GetArrayElementAtIndex(index);
            var elementHeight = EditorGUI.GetPropertyHeight(element);
            /*if (target[index].condition)
            {
                elementHeight += EditorGUI.GetPropertyHeight(element); //same as EditorGUIUtility.singleLineHeight
            }*/
            return elementHeight + EditorGUIUtility.standardVerticalSpacing;
        }
        private void AddLine(ref Rect position)
        {
            position.y += (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing);
        }
        void DrawHeader(Rect rect)
        {
            EditorGUI.LabelField(rect, "Branches");
        }
        public override void OnEditorInstantiate()
        {
            var eventCommand = target as ConditionalBranchEvent;
        }

        public override void SummaryGUI(Rect position)
        {
            var eventCommand = target as ConditionalBranchEvent;
            if (eventListEditorsList == null || queueReset)
            {
                ResetEventEditorsList(eventCommand);
                if (queueReset) queueReset = false;
            }
            GUILayout.BeginVertical();
            GUILayout.Label($"Conditional");
            for (int i = 0; i < eventCommand.branches.Count; i++)
            {
                var editors = new List<EventCommandEditor>();
                InteractableEventList list = eventCommand.branches[i].content;
                if (eventListEditorsList[i] == null)
                {
                    eventListEditorsList[i] = new EventListEditors();
                    GetEditorsFromEventList(list, eventListEditorsList[i]);
                }
                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.LabelField($"Branch #{i}: if =conditional= is {eventCommand.branches[i].condition}");
                EventListWindow.DisplayEventListContent(position, list, eventListEditorsList[i], $"{eventCommand.eventName} Branch #{i}");
                EditorGUILayout.EndVertical();
            }
            GUILayout.EndVertical();
        }

        private void ResetEventEditorsList(ConditionalBranchEvent eventCommand)
        {
            eventListEditorsList = new List<EventListEditors>();
            for (int i = 0; i < eventCommand.branches.Count; i++)
            {
                eventListEditorsList.Add(null);
            }
        }
        private static void GetEditorsFromEventList(InteractableEventList eventList, EventListEditors eventListEditors)
        {
            eventListEditors.UpdateEditors(eventList);
        }
    }
}
