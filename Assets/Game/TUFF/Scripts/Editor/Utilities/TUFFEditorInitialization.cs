using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TUFF.TUFFEditor
{
    [InitializeOnLoad]
    public static class TUFFEditorInitialization
    {
        static TUFFEditorInitialization()
        {
            EditorApplication.playModeStateChanged += LogPlayModeState;
        }

        private static void LogPlayModeState(PlayModeStateChange state)
        {
            //Debug.Log(state);
            if (state == PlayModeStateChange.EnteredPlayMode)
            {
                if (GameManager.instance)
                { 
                    Debug.Log("Found Game Manager");
                    string path = EditorPrefs.GetString("Test Battle Path");
                    if (!string.IsNullOrEmpty(path))
                    {
                        Battle battle = (Battle)AssetDatabase.LoadAssetAtPath(path, typeof(Battle));
                        if (battle) GameManager.instance.TestBattle(battle);
                        else Debug.LogWarning($"Path is invalid! Path: {path}");
                    }
                    EditorPrefs.DeleteKey("Test Battle Path");
                }
            }
        }
    }
}

