using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine.SceneManagement;

namespace TUFF.TUFFEditor
{
    [CustomPropertyDrawer(typeof(PlayAnimationAction))]
    public class PlayAnimationActionPD : EventActionPD
    {
        private List<AnimatorState> animatorStates = new();
        private string newStateName = "New State";
        private AnimationClip newClip = null;

        private int selectedIndex = -1;
        static class Styles
        {
            public static readonly GUIContent newLabel = EditorGUIUtility.TrTextContent("New", "Create new Animator Controller.");
            public static readonly GUIContent newState = EditorGUIUtility.TrTextContent("Add State", "Add the State to the Animation Controller with the assigned name and clip.");
        
        }
        public override void InspectorGUIContent()
        {
            var action = targetObject as PlayAnimationAction;
            EditorGUILayout.PropertyField(targetProperty.FindPropertyRelative("animator"));

            DrawPopup(action);
            DrawData(action);
            EditorGUILayout.Space();
            DrawCreateNewState(action);
        }

        private void DrawData(PlayAnimationAction action)
        {
            if (!action.animator) return;
            if (!action.animator.runtimeAnimatorController) return;
            if (animatorStates == null || animatorStates.Count <= 0) return;
            if (selectedIndex < 0 || selectedIndex >= animatorStates.Count) return;

            if (animatorStates[selectedIndex] == null) return;
            var motion = animatorStates[selectedIndex].motion;
            GUI.enabled = false;
            EditorGUILayout.ObjectField("Selected Motion", motion, typeof(Motion), false);
            GUI.enabled = true;
        }

        private void DrawCreateNewState(PlayAnimationAction action)
        {
            if (!action.animator) return;
            if (!action.animator.runtimeAnimatorController) return;
            var controller = action.animator.runtimeAnimatorController as AnimatorController;

            EditorGUILayout.LabelField("Create New State", EditorStyles.boldLabel);
            newStateName = EditorGUILayout.TextField("New State Name", newStateName);
            newClip = (AnimationClip)EditorGUILayout.ObjectField("New State Clip", newClip, typeof(AnimationClip), false);
            if (GUILayout.Button(Styles.newState, EditorStyles.miniButton))
            {
                if (controller.layers.Length > 0)
                {
                    var machine = controller.layers[0].stateMachine;
                    var uniqueName = machine.MakeUniqueStateName(newStateName);
                    var newState = machine.AddState(uniqueName, Vector2.zero);
                    newState.motion = newClip;
                    Debug.Log($"New State {uniqueName} added!");
                }
            }
        }

        private void DrawPopup(PlayAnimationAction action)
        {
            if (!action.animator) return;
            if (!action.animator.runtimeAnimatorController)
            {
                DrawNewControllerPrompt(action);
                return;
            }

            var controller = action.animator.runtimeAnimatorController as AnimatorController;

            var animationNameProp = targetProperty.FindPropertyRelative("animationName");

            List<string> names = new();
            List<int> values = new();
            animatorStates.Clear();
            int tmp = 0;
            for (int i = 0; i < controller.layers.Length; i++)
            {
                var machine = controller.layers[i].stateMachine;
                var layerStates = machine.states;
                for (int j = 0; j < layerStates.Length; j++)
                {
                    names.Add(layerStates[j].state.name);
                    values.Add(tmp);
                    animatorStates.Add(layerStates[j].state);
                    tmp++;
                }
            }
            if (names.Count <= 0)
            {
                EditorGUILayout.LabelField("No Animations Found!");
            }
            else
            {
                selectedIndex = names.IndexOf(action.animationName);
                if (selectedIndex < 0 || selectedIndex >= animatorStates.Count)
                    selectedIndex = 0;

                EditorGUI.BeginChangeCheck();
                selectedIndex = EditorGUILayout.IntPopup("Animations", selectedIndex, names.ToArray(), values.ToArray());
                if (EditorGUI.EndChangeCheck())
                {
                    //
                }

                if (selectedIndex >= 0 && selectedIndex < animatorStates.Count)
                    animationNameProp.stringValue = names[selectedIndex];
            }
            //EditorGUILayout.PropertyField(animationNameProp);
        }

        private void DrawNewControllerPrompt(PlayAnimationAction action)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("No Animator Controller assigned!");
            if (GUILayout.Button(Styles.newLabel, EditorStyles.miniButton))
            {
                var obj = targetProperty.serializedObject.targetObject;
                Scene scene = (obj as InteractableObject).gameObject.scene;
                var targetName = $"{obj.name} {targetObject.eventName}";

                var newController = CreateAnimatorController(scene, targetName);
                action.animator.runtimeAnimatorController = newController;
            }
            EditorGUILayout.EndHorizontal();
        }

        private static AnimatorController CreateAnimatorController(Scene scene, string targetName)
        {
            LISAEditorUtility.CreateFolderForScene(scene);
            var scenePath = System.IO.Path.GetDirectoryName(scene.path);
            var extPath = scene.name;
            var path = scenePath + System.IO.Path.DirectorySeparatorChar + extPath + System.IO.Path.DirectorySeparatorChar;
            path += targetName + " Controller.asset";
            path = AssetDatabase.GenerateUniqueAssetPath(path);

            AnimatorController animController = AnimatorController.CreateAnimatorControllerAtPath(path);

            return animController;
        }
        public override void SummaryGUI(Rect position)
        {
            EditorGUI.LabelField(position, GetSummaryText());
        }
        private string GetSummaryText()
        {
            var action = targetObject as PlayAnimationAction;
            if (action.animator == null) return "No Animator set";
            string animationName = action.animationName;
            return $"Play Animation '{animationName}'";
        }
    }
}

