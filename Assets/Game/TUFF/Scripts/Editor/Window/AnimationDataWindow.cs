using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;

namespace TUFF.TUFFEditor
{
    public class AnimationDataWindow : EditorWindow
    {
        public enum TimeDisplayMode { TotalTime = 0, Duration = 1 }
        public enum TargetSpritesheetComponent { Image = 0, SpriteRenderer = 1 }
        public enum AnimationDataWindowTab { ClipData = 0, Duplication = 1 }

        public static AnimationDataWindow instance;
        public static AnimationDataWindowTab windowTab = AnimationDataWindowTab.ClipData;
        private static readonly Vector2 windowMinSize = new Vector2(100f, 200f);
        private static Vector2 scrollPos = new Vector2();
        private const float upperPanelMinHeight = 54f;

        public static AnimationClip clip;
        public static float interval = 0.1f;
        public static TimeDisplayMode timeDisplayMode = TimeDisplayMode.TotalTime;

        public static Dictionary<EditorCurveBinding, List<float>> objKeyframesDuration = new Dictionary<EditorCurveBinding, List<float>>();
        //public static List<List<>> objKeyframesDuration = new List<int[]>();

        // Spritesheet
        public static Texture2D spritesheet;
        public static TargetSpritesheetComponent targetComponent;

        // Duplication
        public static int animPopupValue = 0;
        public static BattleAnimation battleAnimation = null;
        public static string cloneName = "x_NewAnim";


        [MenuItem("TUFF/Animation Data Window")]
        public static void ShowWindow()
        {
            instance = GetWindow<AnimationDataWindow>("Animation Data");
            instance.AdjustSize();
        }
        private void AdjustSize()
        {
            //scrollViewHeight = (position.height * 0.5f);
            //boxLineArea = new Rect(0, scrollViewHeight, position.width, lineHeight);
            //lineTexture = EditorGUIUtility.whiteTexture;
            minSize = windowMinSize;
        }
        private void OnGUI()
        {
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Height(position.height));
            DrawTabButtons();

            
            
            LoadAnimClip();
            QuickDuplication();

            EditorGUILayout.EndScrollView();
        }
        private static void DrawTabButtons()
        {
            EditorGUILayout.BeginHorizontal("box");
            // Clip Data
            DrawTabButton("Clip Data", AnimationDataWindowTab.ClipData);
            DrawTabButton("Duplication", AnimationDataWindowTab.Duplication);
            EditorGUILayout.EndHorizontal();
        }

        private static void DrawTabButton(string name, AnimationDataWindowTab tab)
        {
            var prevBgColor = GUI.backgroundColor;
            if (windowTab == tab) GUI.backgroundColor = Color.gray;
            else GUI.backgroundColor = Color.white;
            if (GUILayout.Button(name, EditorStyles.miniButton, GUILayout.Width(100f)))
            {
                windowTab = tab;
            }
            GUI.backgroundColor = prevBgColor;
        }

        private static void QuickDuplication()
        {
            if (windowTab != AnimationDataWindowTab.Duplication) return;
            EditorGUILayout.LabelField("Duplication");
            battleAnimation = DatabaseDropdownDrawer.DrawAnimationsDropdown(ref animPopupValue, battleAnimation);
            battleAnimation = (BattleAnimation)EditorGUILayout.ObjectField(new GUIContent("Battle Animation"), battleAnimation, typeof(BattleAnimation), false);
            cloneName = EditorGUILayout.TextField("Animation Name", cloneName);
            if (GUILayout.Button("Duplicate") && battleAnimation != null)
            {
                AnimatorController cloneController = null;
                RuntimeAnimatorController runtimeController = null;
                AnimatorState cloneAnimatorDefaultState = null;

                var orgAnimator = battleAnimation.anim;
                if (orgAnimator != null) runtimeController = orgAnimator.runtimeAnimatorController;

                // Duplicate Controller
                if (runtimeController != null)
                {
                    string orgPath = AssetDatabase.GetAssetPath(runtimeController);
                    string path = CreateClonePath(orgPath, "ACTR_" + cloneName + ".controller");

                    // Duplicate Asset
                    AssetDatabase.CopyAsset(orgPath, path);
                    cloneController = AssetDatabase.LoadAssetAtPath<AnimatorController>(path);

                    Debug.Log("Controller duplicated");
                    
                    var cloneCtrlStateMachine = cloneController.layers[0].stateMachine;
                    cloneAnimatorDefaultState = cloneCtrlStateMachine.defaultState;
                }

                // Duplicate Clip
                AnimationClip orgClip = ((cloneAnimatorDefaultState != null)
                    ? (AnimationClip)cloneAnimatorDefaultState.motion : null);
                if (orgClip != null)
                {
                    var orgPath = AssetDatabase.GetAssetPath(orgClip);
                    var path = CreateClonePath(orgPath, "SKL_" + cloneName + ".anim");

                    // Duplicate Asset
                    AssetDatabase.CopyAsset(orgPath, path);
                    var cloneClip = AssetDatabase.LoadAssetAtPath<AnimationClip>(path);

                    Debug.Log("Clip duplicated");

                    // Assign Clip to Duplicated Controller
                    cloneAnimatorDefaultState.motion = cloneClip;
                }

                // Duplicate Game Object
                if (battleAnimation != null)
                {
                    var orgPath = AssetDatabase.GetAssetPath(battleAnimation);
                    var path = CreateClonePath(orgPath, cloneName + ".prefab");

                    // Duplicate Asset
                    AssetDatabase.CopyAsset(orgPath, path);
                    var cloneBattleAnimation = AssetDatabase.LoadAssetAtPath<BattleAnimation>(path);
                    cloneBattleAnimation.gameObject.name = cloneName;
                    AnimatorController.SetAnimatorController(cloneBattleAnimation.anim, cloneController);

                    Debug.Log("Game Object duplicated");
                }
                
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                
            }
        }

        private static string CreateClonePath(string originalPath, string assetName)
        {
            string path = originalPath;
            path = path.Substring(0, path.LastIndexOf("/") + 1);
            path += assetName;
            Debug.Log(path);
            path = AssetDatabase.GenerateUniqueAssetPath(path);
            return path;
        }
        public static void CreateKeyframesFromSpritesheet(AnimationClip clip, Texture2D spritesheet)
        {
            if (clip == null)
            {
                Debug.LogWarning("No clip assigned");
                return;
            }
            if (spritesheet == null)
            {
                Debug.LogWarning("No spritesheet assigned");
                return;
            }
            var path = AssetDatabase.GetAssetPath(spritesheet);
            var importer = AssetImporter.GetAtPath(path) as TextureImporter; // Get Texture2D as TextureImporter for spritesheet data
            Object[] objs = AssetDatabase.LoadAllAssetsAtPath(path);
            var sprites = new List<Sprite>();
            for (int i = 0; i < objs.Length; i++) // Get only sprites from Texture2D
            {
                if (objs[i] is Sprite spr)
                {
                    sprites.Add(spr);
                }
            }

            Debug.Log("Spritesheet count: " + sprites.Count);
            EditorCurveBinding[] objectBindings = AnimationUtility.GetObjectReferenceCurveBindings(clip);
            Debug.Log("Object Bindings count: " + objectBindings.Length);

            var targetPath = "Sprite";
            var targetType = typeof(UnityEngine.UI.Image);
            if (targetComponent == TargetSpritesheetComponent.SpriteRenderer)
                targetType = typeof(UnityEngine.SpriteRenderer);
            
                
            var targetPropertyName = "m_Sprite";

            for (int i = 0; i < objectBindings.Length; i++)
            {
                Debug.Log($"Path{i}: {objectBindings[i].path}");
                Debug.Log($"Type{i}: {objectBindings[i].type}");
                Debug.Log($"Property Name{i}: {objectBindings[i].propertyName}");
            }
            var existingIndex = System.Array.FindIndex(objectBindings, e => e.path == targetPath && e.type == targetType && e.propertyName == targetPropertyName);
            Debug.Log("Existing Index: " + existingIndex);
            EditorCurveBinding targetBinding;
            if (existingIndex >= 0) targetBinding = objectBindings[existingIndex];
            
            else targetBinding = EditorCurveBinding.PPtrCurve(targetPath, targetType, targetPropertyName);
            var newKeyframes = new ObjectReferenceKeyframe[sprites.Count];

            for (int i = 0; i < sprites.Count; i++) // Create keyframes from sprites
            {
                float time = i * 0.1f;
                newKeyframes[i].time = time;
                newKeyframes[i].value = sprites[i];
            }
            Undo.RecordObject(clip, $"Assigned new keyframes to {clip.name} clip");
            AnimationUtility.SetObjectReferenceCurve(clip, targetBinding, newKeyframes);
            Debug.Log("Keyframes assigned!");
        }

        private static void LoadAnimClip()
        {
            if (windowTab != AnimationDataWindowTab.ClipData) return;
            clip = (AnimationClip)EditorGUILayout.ObjectField(new GUIContent("Animation Clip"), clip, typeof(AnimationClip), false);
            if (clip == null) return;
            EditorGUILayout.Space();

            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Creation", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            spritesheet = (Texture2D)EditorGUILayout.ObjectField(new GUIContent("Spritesheet"), spritesheet, typeof(Texture2D), false);
            EditorGUILayout.BeginVertical();
            if (GUILayout.Button("Assign Keyframes from Spritesheet"))
            {
                CreateKeyframesFromSpritesheet(clip, spritesheet);
            }
            targetComponent =
                (TargetSpritesheetComponent)EditorGUILayout.IntPopup("Target Component", (int)targetComponent,
                new string[] { "Image", "Sprite Renderer" },
                new int[] { 0, 1 });
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();

            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("General", EditorStyles.boldLabel);
            clip.frameRate = EditorGUILayout.FloatField("Sample Rate", clip.frameRate);
            var settings = AnimationUtility.GetAnimationClipSettings(clip);
            EditorGUI.BeginChangeCheck();
            settings.loopTime = EditorGUILayout.Toggle(Styles.clipLoopLabel, settings.loopTime);
            if (EditorGUI.EndChangeCheck())
            {
                AnimationUtility.SetAnimationClipSettings(clip, settings);
            }
            EditorGUILayout.LabelField($"Total Length: {clip.length}");
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();

            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Curves", EditorStyles.boldLabel);
            // Draw Time Display Mode
            EditorGUI.BeginChangeCheck();
            timeDisplayMode = (TimeDisplayMode)EditorGUILayout.IntPopup("Time Display Mode", (int)timeDisplayMode,
                new string[] { "Total Time", "Duration" },
                new int[] { 0, 1 });
            if (EditorGUI.EndChangeCheck())
            {
                objKeyframesDuration.Clear();
            }
            EditorGUILayout.LabelField("= OBJECT REFERENCES =");
            var objectBindings = AnimationUtility.GetObjectReferenceCurveBindings(clip);
            for (int k = 0; k < objectBindings.Length; k++)
            {
                EditorCurveBinding binding = objectBindings[k];
                var keyframes = AnimationUtility.GetObjectReferenceCurve(clip, binding);

                EditorGUILayout.LabelField($"{binding.path} ({binding.propertyName})");
                // Draw Assign timings from GIF option
                if (GUILayout.Button("Assign timings from GIF"))
                {
                    string gifPath = EditorUtility.OpenFilePanel("Select a GIF file", "", "gif");
                    if (string.IsNullOrEmpty(gifPath)) Debug.LogWarning("Path was empty!");
                    float[] timings = TUFFImageParser.GetGIFTimings(gifPath);
                    if (timings != null)
                    {
                        for (int i = 0; i < timings.Length; i++)
                            Debug.Log($"Frame {i}: {timings[i]}");
                        if (timings.Length != keyframes.Length)
                            Debug.LogWarning($"Keyframe count mismatch! Not all timings will be assigned. ({timings.Length}/{keyframes.Length})");
                        AssignTimingsFromDurations(binding, keyframes, timings);

                        Debug.Log("Timings assigned!");
                        return;
                    }
                }
                
                if (!objKeyframesDuration.ContainsKey(binding))
                {
                    objKeyframesDuration.Add(binding, new List<float>());
                }
                
                EditorGUILayout.BeginHorizontal();
                interval = EditorGUILayout.FloatField("Interval", interval);
                if (GUILayout.Button("Set Interval"))
                {
                    var copyKeyframes = AnimationUtility.GetObjectReferenceCurve(clip, binding);
                    float time = 0;
                    for (int i = 0; i < copyKeyframes.Length; i++)
                    {
                        copyKeyframes[i].time = time;
                        time += interval;
                    }
                    AnimationUtility.SetObjectReferenceCurve(clip, binding, copyKeyframes);
                }
                EditorGUILayout.EndHorizontal();

                var durations = objKeyframesDuration[binding];
                // Time
                if (timeDisplayMode == TimeDisplayMode.Duration)
                {
                    if (GUILayout.Button("Apply Durations"))
                    {
                        Debug.Log("Apply Durations");
                        if (durations.Count > 0)
                        {
                            AssignTimingsFromDurations(binding, keyframes, durations);
                            objKeyframesDuration.Clear();
                            return;
                        }
                    }
                }
                EditorGUI.BeginChangeCheck();
                for (int i = 0; i < keyframes.Length; i++)
                {
                    float duration = (i + 1 < keyframes.Length ? keyframes[i + 1].time - keyframes[i].time : clip.length - keyframes[i].time);
                    float totalTime = keyframes[i].time;
                    if (durations.Count != keyframes.Length)
                    {
                        durations.Add(duration);
                    }
                    if (timeDisplayMode == TimeDisplayMode.TotalTime)
                    {
                        EditorGUILayout.BeginHorizontal();
                        keyframes[i].time = EditorGUILayout.FloatField("Time " + (i + 1), keyframes[i].time);
                        EditorGUILayout.LabelField($"[{duration.ToString("F2")}]");
                        if (keyframes[i].value)
                        {
                            System.Type valueType = keyframes[i].value.GetType();
                            keyframes[i].value =
                                //EditorGUILayout.ObjectField(Styles.keyframeValueLabel, keyframes[i].value, valueType, false, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                                EditorGUILayout.ObjectField(keyframes[i].value, valueType, false, GUILayout.Width(45f), GUILayout.Height(45f));
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                    else if (timeDisplayMode == TimeDisplayMode.Duration)
                    {
                        EditorGUILayout.BeginHorizontal();
                        durations[i] = EditorGUILayout.FloatField("Time " + (i + 1), durations[i]);
                        EditorGUILayout.LabelField($"[{totalTime}]");
                        if (keyframes[i].value)
                        {
                            System.Type valueType = keyframes[i].value.GetType();
                            keyframes[i].value =
                                //EditorGUILayout.ObjectField(Styles.keyframeValueLabel, keyframes[i].value, valueType, false, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                                EditorGUILayout.ObjectField(keyframes[i].value, valueType, false, GUILayout.Width(45f), GUILayout.Height(45f));
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                    
                }
                if (EditorGUI.EndChangeCheck())
                {
                    if (timeDisplayMode == TimeDisplayMode.TotalTime) 
                        AnimationUtility.SetObjectReferenceCurve(clip, binding, keyframes);
                }
                if (GUILayout.Button("Add Keyframe"))
                {
                    System.Array.Resize(ref keyframes, keyframes.Length + 1);
                    keyframes[^1].time = keyframes[^2].time;
                    keyframes[^1].value = keyframes[^2].value;
                    AnimationUtility.SetObjectReferenceCurve(clip, binding, keyframes);
                }
                
                EditorGUILayout.Space();
            }
            EditorGUILayout.EndVertical();
            //EditorGUILayout.LabelField("= CURVES =");
            //var curveBindings = AnimationUtility.GetCurveBindings(clip);
            //foreach (var binding in curveBindings)
            //{
            //    EditorGUILayout.LabelField(binding.path);
            //    AnimationCurve curve = AnimationUtility.GetEditorCurve(clip, binding);
            //    EditorGUILayout.LabelField(binding.path + "/" + binding.propertyName + ", Keys: " + curve.keys);
            //}
        }

        private static void AssignTimingsFromDurations(EditorCurveBinding binding, ObjectReferenceKeyframe[] keyframes, float[] durations)
        {
            float baseTime = durations[0];
            for (int i = 0; i < keyframes.Length; i++)
            {
                if (i == 0) { keyframes[i].time = 0; continue; }
                keyframes[i].time = baseTime;
                baseTime += durations[i];
            }
            AnimationUtility.SetObjectReferenceCurve(clip, binding, keyframes);
        }
        private static void AssignTimingsFromDurations(EditorCurveBinding binding, ObjectReferenceKeyframe[] keyframes, List<float> durations)
        {
            AssignTimingsFromDurations(binding, keyframes, durations.ToArray());
        }
        private static class Styles
        {
            public static GUIContent clipLoopLabel = new GUIContent("Loop");
            public static GUIContent keyframeValueLabel = new GUIContent("Value");
        }
    }
}

