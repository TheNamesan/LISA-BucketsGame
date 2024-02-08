using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

namespace TUFF.TUFFEditor
{
    [CustomEditor(typeof(ScriptablesBegone))]
    public class ScriptablesBegoneEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            var obj = target as ScriptablesBegone;
            Scene scene = obj.gameObject.scene;
            var rootObjs = scene.GetRootGameObjects();
            var type = typeof(EventCommand);
            var objs = Object.FindObjectsOfType(type);
            for (int i = 0; i < objs.Length; i++)
            {
                EditorGUILayout.ObjectField(new GUIContent(i.ToString()), objs[i], type, true);
            }
            if (GUILayout.Button(new GUIContent("Delete all")))
            {
                for (int i = 0; i < objs.Length; i++)
                {
                    DestroyImmediate(objs[i]);
                }
            }
        }
    }

}
