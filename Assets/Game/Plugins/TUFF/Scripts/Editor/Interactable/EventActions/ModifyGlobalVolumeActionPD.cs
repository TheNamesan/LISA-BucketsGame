using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEditor.Rendering;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

namespace TUFF.TUFFEditor
{
    [CustomPropertyDrawer(typeof(ModifyGlobalVolumeAction))]
    public class ModifyGlobalVolumeActionPD : EventActionPD
    {
        static class Styles
        {
            public static readonly GUIContent newLabel = EditorGUIUtility.TrTextContent("New", "Create a new profile.");
            public static readonly GUIContent cloneLabel = EditorGUIUtility.TrTextContent("Clone", "Create a new profile and copy the content of the currently assigned profile.");
            public static readonly string noVolumeMessage = L10n.Tr("Please select or create a new Volume profile to begin applying effects to the scene.");
        }

        SerializedObject volumeProfileSerialized;
        Editor originEditor;
        public override void InspectorGUIContent()
        {
            var mods = targetObject as ModifyGlobalVolumeAction;

            var profile = targetProperty.FindPropertyRelative("volumeProfile");
            
            bool showCopy = profile.objectReferenceValue != null;
            Rect lineRect = EditorGUILayout.GetControlRect();

            int buttonWidth = showCopy ? 45 : 60;
            float indentOffset = EditorGUI.indentLevel * 15f;
            var labelRect = new Rect(lineRect.x, lineRect.y, EditorGUIUtility.labelWidth - indentOffset - 3, lineRect.height);
            var fieldRect = new Rect(labelRect.xMax + 5, lineRect.y, lineRect.width - labelRect.width - buttonWidth * (showCopy ? 2 : 1) - 5, lineRect.height);
            var buttonNewRect = new Rect(fieldRect.xMax, lineRect.y, buttonWidth, lineRect.height);
            var buttonCopyRect = new Rect(buttonNewRect.xMax, lineRect.y, buttonWidth, lineRect.height);

            EditorGUI.PrefixLabel(labelRect, new GUIContent(profile.displayName, profile.tooltip));
            profile.objectReferenceValue = EditorGUI.ObjectField(fieldRect, profile.objectReferenceValue, typeof(VolumeProfile), false);

            // New Button
            if (GUI.Button(buttonNewRect, Styles.newLabel, showCopy ? EditorStyles.miniButtonLeft : EditorStyles.miniButton))
            {
                // By default, try to put assets in a folder next to the currently active
                // scene file. If the user isn't a scene, put them in root instead.
                var obj = targetProperty.serializedObject.targetObject;
                var targetName = $"{obj.name} {targetObject.eventName}";
                Scene scene = (obj is GameObject ? (obj as GameObject).scene : new Scene());
                var path = CreatePath(scene, targetName, obj);
                Debug.Log(path);
                profile.objectReferenceValue = VolumeProfileFactory.CreateVolumeProfileAtPath(path);
            }
            // Clone Button
            if (showCopy && GUI.Button(buttonCopyRect, Styles.cloneLabel, EditorStyles.miniButtonRight))
            {
                // Duplicate the currently assigned profile and save it as a new profile
                var origin = mods.volumeProfile;
                var path = AssetDatabase.GetAssetPath(profile.objectReferenceValue);

                path = AssetDatabase.GenerateUniqueAssetPath(path);

                var asset = Object.Instantiate(origin);
                asset.components.Clear();
                AssetDatabase.CreateAsset(asset, path);

                foreach (var item in origin.components)
                {
                    var itemCopy = Object.Instantiate(item);
                    itemCopy.hideFlags = HideFlags.HideInInspector | HideFlags.HideInHierarchy;
                    itemCopy.name = item.name;
                    asset.components.Add(itemCopy);
                    AssetDatabase.AddObjectToAsset(itemCopy, asset);
                }

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                profile.objectReferenceValue = asset;
            }

            if (originEditor == null) originEditor = Editor.CreateEditor(targetProperty.serializedObject.targetObject);
            VolumeComponentListEditor editor = new VolumeComponentListEditor(originEditor);
            if (mods.volumeProfile != null)
            {
                if (volumeProfileSerialized == null) volumeProfileSerialized = new SerializedObject(mods.volumeProfile);
                editor.Init(mods.volumeProfile, volumeProfileSerialized);
                editor.OnGUI();
            }

            if (profile.objectReferenceValue == null)
                EditorGUILayout.HelpBox(Styles.noVolumeMessage, MessageType.Info);

            EditorGUILayout.PropertyField(targetProperty.FindPropertyRelative("colorAdjustmentsMods"));
        }
        public override void SummaryGUI(Rect position)
        {
            EditorGUI.LabelField(position, "Modify Global Volume");
        }
        public static string CreatePath(Scene scene, string targetName, Object rootObject = null)
        {
            string path;

            if (rootObject is ScriptableObject)
            {
                var scriptObj = rootObject as ScriptableObject;
                var assetPath = AssetDatabase.GetAssetPath(scriptObj);
                if (!string.IsNullOrEmpty(assetPath))
                {
                    path = assetPath.Substring(0, assetPath.LastIndexOf("/") + 1);
                }
                else path = "Assets/";
            }
            else
            {
                if (string.IsNullOrEmpty(scene.path))
                {
                    path = "Assets/";
                }
                else
                {
                    var scenePath = Path.GetDirectoryName(scene.path);
                    var extPath = scene.name;
                    var profilePath = scenePath + Path.DirectorySeparatorChar + extPath;

                    if (!AssetDatabase.IsValidFolder(profilePath))
                    {
                        var directories = profilePath.Split(Path.DirectorySeparatorChar);
                        string rootPath = "";
                        foreach (var directory in directories)
                        {
                            var newPath = rootPath + directory;
                            if (!AssetDatabase.IsValidFolder(newPath))
                                AssetDatabase.CreateFolder(rootPath.TrimEnd(Path.DirectorySeparatorChar), directory);
                            rootPath = newPath + Path.DirectorySeparatorChar;
                        }
                    }

                    path = profilePath + Path.DirectorySeparatorChar;
                }
            }

            path += targetName + " Profile.asset";
            path = AssetDatabase.GenerateUniqueAssetPath(path);
            return path;
        }
    }
}

