using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace TUFF.TUFFEditor
{
    [CustomPropertyDrawer(typeof(ConditionalBranchAction))]
    public class ConditionalBranchActionPD : EventActionPD
    {
        private ReorderableList branchesList;
        private SerializedProperty branches;
        private List<List<EventActionPD>> m_branchesDrawers = new List<List<EventActionPD>>(); // Class where drawers will be stored for each branch
        private List<EventActionPD> m_elseDrawer = new List<EventActionPD>();
        private static bool queueReset = false;
        private void GetList()
        {
            branches = targetProperty.FindPropertyRelative("branches");
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
        public override void InspectorGUIContent()
        {
            if (branchesList == null)
            {
                GetList();
            }
            var eventAction = targetObject as ConditionalBranchAction;
            EditorGUILayout.PropertyField(targetProperty.FindPropertyRelative("addBranchWhenNoConditionsApply"));
            EditorGUI.BeginChangeCheck();
            branchesList.DoLayoutList();
            if (EditorGUI.EndChangeCheck())
            {
                queueReset = true;
            }
        }
        void DrawListItems(Rect rect, int index, bool isActive, bool isFocused)
        {
            rect.height = 20f;
            SerializedProperty prop = branchesList.serializedProperty.GetArrayElementAtIndex(index);
            var content = prop.FindPropertyRelative("actionList.content");
            EditorGUI.PropertyField(rect, prop, new GUIContent($"Condition | Event Count: {content.arraySize}"));
        }
        float GetElementHeight(int index)
        {
            var target = (targetObject as ConditionalBranchAction).branches;
            if (target.Count == 0) return 0f;
            var element = branchesList.serializedProperty.GetArrayElementAtIndex(index);
            var elementHeight = EditorGUI.GetPropertyHeight(element);
            return elementHeight + EditorGUIUtility.standardVerticalSpacing;
        }
        void DrawHeader(Rect rect)
        {
            EditorGUI.LabelField(rect, "Branches");
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
        public override void SummaryGUI(Rect position)
        {
            var conditionalBranchAction = targetObject as ConditionalBranchAction;
            if (conditionalBranchAction == null) { Debug.LogWarning("Object is not Conditional Branch (Summary)"); return; }
            branches = targetProperty.FindPropertyRelative("branches");

            if (m_branchesDrawers == null || m_branchesDrawers.Count != conditionalBranchAction.branches.Count || m_elseDrawer == null || queueReset)
            {
                ResetEventEditorsList(conditionalBranchAction);
                if (queueReset) queueReset = false;
            }

            EditorGUI.LabelField(position, "Conditional");
            position.y += 20f;

            GUILayout.BeginVertical();
            for (int i = 0; i < branches.arraySize; i++)
            {
                if (i >= conditionalBranchAction.branches.Count)
                {
                    Debug.Log($"{i}/{conditionalBranchAction.branches.Count}");
                    continue;
                }
                // ================================================
                string condition = BranchActionContentPD.GetConditionText(conditionalBranchAction.branches[i]);
                string labelText = $"=== Branch #{i}: {condition}";
                string selectionPanelTitle = $"{conditionalBranchAction.eventName} Branch #{i}";

                SerializedProperty actionListContentProp = branches.GetArrayElementAtIndex(i).FindPropertyRelative("actionList.content");
                ActionList list = conditionalBranchAction.branches[i].actionList;

                if (m_branchesDrawers[i] == null || m_branchesDrawers[i].Count != list.content.Count)
                {
                    m_branchesDrawers[i] = new List<EventActionPD>();
                    EventActionListWindow.UpdatePDs(actionListContentProp, list.content, m_branchesDrawers[i]); // Important!
                }
                //EditorGUILayout.BeginVertical("box");

                position = EventActionListWindow.DrawBranch(position, targetProperty.propertyPath, m_branchesDrawers[i], labelText, selectionPanelTitle, actionListContentProp, list);
            }
            if (conditionalBranchAction.addBranchWhenNoConditionsApply)
            {
                string labelText = $"=== Else";
                string selectionPanelTitle = $"{conditionalBranchAction.eventName} Else Branch";
                SerializedProperty actionListContentProp = targetProperty.FindPropertyRelative("elseActionList.content");
                ActionList list = conditionalBranchAction.elseActionList;
                if (m_elseDrawer == null || m_elseDrawer.Count != list.content.Count)
                {
                    m_elseDrawer = new List<EventActionPD>();
                    EventActionListWindow.UpdatePDs(actionListContentProp, list.content, m_elseDrawer); // Important!
                }
                position = EventActionListWindow.DrawBranch(position, targetProperty.propertyPath, m_elseDrawer, labelText, selectionPanelTitle, actionListContentProp, list);
            }
            GUILayout.EndVertical();
        }

        

        public override float GetSummaryHeight()
        {
            if (EventActionListWindow.eventDeleted) { Debug.LogWarning("Event Deleted!"); return 20f; };
            var conditionalBranchAction = targetObject as ConditionalBranchAction;
            if (conditionalBranchAction == null) { Debug.LogWarning("Object is not Conditional Branch"); return 20f; }
            
            float height = 0;
            branches = targetProperty.FindPropertyRelative("branches");
            
            if (branches != null && conditionalBranchAction.branches != null && conditionalBranchAction.branches.Count > 0)
            {
                for (int i = 0; i < branches.arraySize; i++)
                {
                    if (conditionalBranchAction.branches[i] == null) continue;
                    ActionList list = conditionalBranchAction.branches[i].actionList;
                    if (list == null) continue;

                    height += 20f;
                    height += EventActionListWindow.GetListHeight(targetProperty.propertyPath, list.content);
                }
            }
            if (conditionalBranchAction.addBranchWhenNoConditionsApply)
            {
                height += 20f;
                height += EventActionListWindow.GetListHeight(targetProperty.propertyPath, conditionalBranchAction.elseActionList.content);
            }
            return 20f + height;
        }
        private void ResetEventEditorsList(ConditionalBranchAction action)
        {
            m_branchesDrawers.Clear();
            for (int i = 0; i < action.branches.Count; i++)
            {
                m_branchesDrawers.Add(new List<EventActionPD>(action.branches[i].actionList.content.Count));
            }
            m_elseDrawer = new List<EventActionPD>(action.elseActionList.content.Count);
        }
    }
}
