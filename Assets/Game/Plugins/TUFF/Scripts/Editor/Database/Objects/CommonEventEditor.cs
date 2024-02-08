using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using System.IO;

namespace TUFF.TUFFEditor
{
    [CustomEditor(typeof(CommonEvent))]
    public class CommonEventEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var content = serializedObject.FindProperty("actionList.content");
            EditorGUILayout.LabelField($"Event Count: {content.arraySize}");
            if(GUILayout.Button(new GUIContent("Content", "Show the event list.")))
            {
                EventActionListWindow.ShowWindow();
            }

            serializedObject.ApplyModifiedProperties();
        }

        //public static CommonEvent CreateCommonEvent(Scene scene, string targetName, List<EventCommand> eventCommands = null)
        //{
        //    string path;

        //    if (string.IsNullOrEmpty(scene.path))
        //    {
        //        path = "Assets/";
        //    }
        //    else
        //    {
        //        var scenePath = Path.GetDirectoryName(scene.path);
        //        var extPath = scene.name;
        //        var commonEventPath = scenePath + Path.DirectorySeparatorChar + extPath;

        //        if (!AssetDatabase.IsValidFolder(commonEventPath))
        //        {
        //            var directories = commonEventPath.Split(Path.DirectorySeparatorChar);
        //            string rootPath = "";
        //            foreach (var directory in directories)
        //            {
        //                var newPath = rootPath + directory;
        //                if (!AssetDatabase.IsValidFolder(newPath))
        //                    AssetDatabase.CreateFolder(rootPath.TrimEnd(Path.DirectorySeparatorChar), directory);
        //                rootPath = newPath + Path.DirectorySeparatorChar;
        //            }
        //        }

        //        path = commonEventPath + Path.DirectorySeparatorChar;
        //    }

        //    path += targetName + "CommonEvent.asset";
        //    var asset = (CommonEvent)AssetDatabase.LoadMainAssetAtPath(path);
        //    if (asset != null) { Debug.Log($"Already exists at {path}"); return asset; }
        //    path = AssetDatabase.GenerateUniqueAssetPath(path);

        //    var commonEvent = ScriptableObject.CreateInstance<CommonEvent>();
        //    AssetDatabase.CreateAsset(commonEvent, path);
        //    if (eventCommands != null)
        //    {
        //        for (int i = 0; i < eventCommands.Count; i++)
        //        {
        //            var eventCom = Instantiate(eventCommands[i]);
        //            AssetDatabase.AddObjectToAsset(eventCom, path);
        //            commonEvent.content.Add(eventCom);
        //        }
        //    }

        //    AssetDatabase.SaveAssets();
        //    AssetDatabase.Refresh();
        //    Debug.Log("Created Event at :" + path);

        //    return commonEvent;
        //}
    }
}

