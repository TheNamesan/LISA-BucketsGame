using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace TUFF.TUFFEditor
{
    [CustomPropertyDrawer(typeof(ShowChoicesAction))]
    public class ShowChoicesActionPD : EventActionPD
    {
        private ReorderableList branchesList;
        private SerializedProperty branches;
        private List<List<EventActionPD>> m_branchesDrawers = new List<List<EventActionPD>>(); // Class where drawers will be stored for each branch
        private List<EventActionPD> m_cancelDrawer = new List<EventActionPD>();
        private static bool queueReset = false;
        private void GetList()
        {
            branches = targetProperty.FindPropertyRelative("choices");
            branchesList = new ReorderableList(targetProperty.serializedObject, branches, true, true, true, true);
            branchesList.drawElementCallback = DrawListItems;
            branchesList.drawHeaderCallback = DrawHeader;
            branchesList.elementHeightCallback = GetElementHeight;
            branchesList.onAddCallback = OnAddCallback;
            branchesList.onChangedCallback = (ReorderableList l) => {
                Debug.Log("Moved in branch");
                EventActionListWindow.ForceResetList();
            };
        }
        void DrawListItems(Rect rect, int index, bool isActive, bool isFocused)
        {
            var showChoicesAction = targetObject as ShowChoicesAction;
            rect.height = 20f;
            SerializedProperty prop = branchesList.serializedProperty.GetArrayElementAtIndex(index);
            SerializedProperty choiceText = prop.FindPropertyRelative("choice");
            EditorGUI.PropertyField(rect, choiceText, 
                new GUIContent($"{showChoicesAction.choices[index].ParsedChoiceText()} | Event Count: {showChoicesAction.choices[index].actionList.content.Count}"));
        }
        float GetElementHeight(int index)
        {
            return 20f;
        }
        void DrawHeader(Rect rect)
        {
            EditorGUI.LabelField(rect, "Choices");
        }
        private void OnAddCallback(ReorderableList rl)
        {
            var index = rl.serializedProperty.arraySize;
            rl.serializedProperty.arraySize++;
            rl.index = index;
            var element = rl.serializedProperty.GetArrayElementAtIndex(index);
            Debug.Log(index);
            var actionList = element.FindPropertyRelative("actionList");
            actionList.FindPropertyRelative("content").arraySize = 0;
        }
        public override void InspectorGUIContent()
        {
            var cancelBehaviour = targetProperty.FindPropertyRelative("cancelBehaviour");
            EditorGUILayout.PropertyField(cancelBehaviour);
            if ((ChoicesCancelBehaviour)cancelBehaviour.enumValueIndex == ChoicesCancelBehaviour.ChooseIndex)
            {
                EditorGUILayout.PropertyField(targetProperty.FindPropertyRelative("cancelChoiceIndex"));
            }
            if (branchesList == null)
            {
                GetList();
            }
            EditorGUI.BeginChangeCheck();
            branchesList.DoLayoutList();
            if (EditorGUI.EndChangeCheck())
            {
                queueReset = true;
            }
        }
        public override void SummaryGUI(Rect position)
        {
            //EditorGUI.LabelField(position, GetSummaryText());
            var showChoicesAction = targetObject as ShowChoicesAction;
            if (showChoicesAction == null) { Debug.LogWarning("Object is not Show Choices (Summary)"); return; }
            branches = targetProperty.FindPropertyRelative("choices");

            if (m_branchesDrawers == null || m_branchesDrawers.Count != showChoicesAction.choices.Count || m_cancelDrawer == null || queueReset)
            {
                ResetEventEditorsList(showChoicesAction);
                if (queueReset) queueReset = false;
            }

            EditorGUI.LabelField(position, "Show Choices");
            position.y += 20f;

            for (int i = 0; i < branches.arraySize; i++)
            {
                if (EventActionListWindow.eventDeleted) { Debug.LogWarning("Event Deleted!"); return; };
                if (i >= showChoicesAction.choices.Count)
                {
                    Debug.Log($"{i}/{showChoicesAction.choices.Count}");
                    continue;
                }
                // ================================================
                string labelText = $"=== When {showChoicesAction.choices[i].ParsedChoiceText()}";
                string selectionPanelTitle = $"{showChoicesAction.eventName} When '{showChoicesAction.choices[i].ParsedChoiceText()}'";

                SerializedProperty actionListContentProp = branches.GetArrayElementAtIndex(i).FindPropertyRelative("actionList.content");
                ActionList list = showChoicesAction.choices[i].actionList;

                if (m_branchesDrawers[i] == null || m_branchesDrawers[i].Count != list.content.Count)
                {
                    m_branchesDrawers[i] = new List<EventActionPD>();
                    EventActionListWindow.UpdatePDs(actionListContentProp, list.content, m_branchesDrawers[i]); // Important!
                }

                position = EventActionListWindow.DrawBranch(position, targetProperty.propertyPath, m_branchesDrawers[i], labelText, selectionPanelTitle, actionListContentProp, list);
                if (EventActionListWindow.eventDeleted) { Debug.LogWarning("Event Deleted!"); return; };
            }
            if (showChoicesAction.cancelBehaviour == ChoicesCancelBehaviour.Branch)
            {
                string labelText = $"=== When Cancel";
                string selectionPanelTitle = $"{showChoicesAction.eventName} When Cancel";
                SerializedProperty actionListContentProp = targetProperty.FindPropertyRelative("cancelActionList.content");
                ActionList list = showChoicesAction.cancelActionList;
                if (m_cancelDrawer == null || m_cancelDrawer.Count != list.content.Count)
                {
                    m_cancelDrawer = new List<EventActionPD>();
                    EventActionListWindow.UpdatePDs(actionListContentProp, list.content, m_cancelDrawer); // Important!
                }
                position = EventActionListWindow.DrawBranch(position, targetProperty.propertyPath, m_cancelDrawer, labelText, selectionPanelTitle, actionListContentProp, list);
            }
        }
        public override float GetSummaryHeight()
        {
            if (EventActionListWindow.eventDeleted) { Debug.LogWarning("Event Deleted!"); return 20f; };
            var showChoicesAction = targetObject as ShowChoicesAction;
            if (showChoicesAction == null) { Debug.LogWarning("Object is not Show Choices"); return 20f; }

            float height = 0;
            branches = targetProperty.FindPropertyRelative("choices");

            if (branches != null && showChoicesAction.choices != null && showChoicesAction.choices.Count > 0)
            {
                for (int i = 0; i < branches.arraySize; i++)
                {
                    if (showChoicesAction.choices[i] == null) continue;
                    ActionList list = showChoicesAction.choices[i].actionList;
                    if (list == null) continue;

                    height += 20f;
                    height += EventActionListWindow.GetListHeight(targetProperty.propertyPath, list.content);
                }
            }
            if (showChoicesAction.cancelBehaviour == ChoicesCancelBehaviour.Branch)
            {
                height += 20f;
                height += EventActionListWindow.GetListHeight(targetProperty.propertyPath, showChoicesAction.cancelActionList.content);
            }
            return 20f + height;
        }
        private void ResetEventEditorsList(ShowChoicesAction action)
        {
            m_branchesDrawers.Clear();
            for (int i = 0; i < action.choices.Count; i++)
            {
                m_branchesDrawers.Add(new List<EventActionPD>(action.choices[i].actionList.content.Count));
            }
            m_cancelDrawer = new List<EventActionPD>(action.cancelActionList.content.Count);
        }
    }
}

