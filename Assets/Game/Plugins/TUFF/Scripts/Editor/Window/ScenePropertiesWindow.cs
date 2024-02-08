using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.SceneManagement;
using UnityEditor;

namespace TUFF.TUFFEditor
{
    public class ScenePropertiesWindow
    {
        [MenuItem("TUFF/Scene Properties")]
        public static void SelectSceneProperties()
        {
            if (EditorSceneManager.loadedSceneCount <= 0) return;
            var sp = GameObject.FindGameObjectWithTag("SceneProperties");
            if (sp != null) Selection.activeTransform = sp.transform;
        }
    }
}

