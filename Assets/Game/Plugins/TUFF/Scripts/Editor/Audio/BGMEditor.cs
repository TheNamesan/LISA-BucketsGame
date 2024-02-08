using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
//using OggVorbis;

namespace TUFF.TUFFEditor
{
    [CustomEditor(typeof(BGM)), CanEditMultipleObjects]
    public class BGMEditor : Editor
    {
        private SerializedProperty m_clip;
        private SerializedProperty m_loopStart;
        private SerializedProperty m_loopEnd;
        private SerializedProperty m_prebakedLoopIntro;
        private SerializedProperty m_prebakedLoopMain;
        private SerializedProperty m_songName;
        private SerializedProperty m_author;
        public void OnEnable()
        {
            m_clip = serializedObject.FindProperty("clip");
            m_loopStart = serializedObject.FindProperty("loopStart");
            m_loopEnd = serializedObject.FindProperty("loopEnd");
            m_prebakedLoopIntro = serializedObject.FindProperty("prebakedLoopIntro");
            m_prebakedLoopMain = serializedObject.FindProperty("prebakedLoopMain");
            m_songName = serializedObject.FindProperty("songName");
            m_author = serializedObject.FindProperty("author");
        }
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(m_clip);
            EditorGUILayout.PropertyField(m_loopStart);
            EditorGUILayout.PropertyField(m_loopEnd);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Prebaked Clips", EditorStyles.boldLabel);
            if (GUILayout.Button("Bake from loop timestamps"))
            {
                AudioClip clip = m_clip.objectReferenceValue as AudioClip;
                int loopStart = m_loopStart.intValue;
                int loopEnd = m_loopEnd.intValue;
                if (clip != null && loopStart > 0 && loopEnd > 0)
                {
                    //var introClipTmp = LISAUtility.CutAudioClip(clip, 0, loopStart, "_intro");
                    //var loopClipTmp = LISAUtility.CutAudioClip(clip, loopStart, loopEnd, "_loop");

                    //var clipPath = AssetDatabase.GetAssetPath(clip);

                    //var introPath = CreateClonePath(clipPath, introClipTmp.name, ".ogg");
                    ////VorbisPlugin.Save(introPath, introClipTmp, 0.6f);
                    //var loopPath = CreateClonePath(clipPath, loopClipTmp.name, ".ogg");
                    ////VorbisPlugin.Save(loopPath, loopClipTmp, 0.6f);
                    //AssetDatabase.Refresh();

                    //m_prebakedLoopIntro.objectReferenceValue = AssetDatabase.LoadAssetAtPath<AudioClip>(introPath);
                    //m_prebakedLoopMain.objectReferenceValue = AssetDatabase.LoadAssetAtPath<AudioClip>(loopPath);
                    //Debug.Log("Clip creation done.");
                }
            }
            EditorGUILayout.PropertyField(m_prebakedLoopIntro);
            EditorGUILayout.PropertyField(m_prebakedLoopMain);

            EditorGUILayout.PropertyField(m_songName);
            EditorGUILayout.PropertyField(m_author);

            serializedObject.ApplyModifiedProperties();
        }
        private static string CreateClonePath(string originalPath, string assetName, string filename)
        {
            string path = originalPath;
            path = path.Substring(0, path.LastIndexOf("/") + 1);
            path += assetName + filename;
            Debug.Log(path);
            path = AssetDatabase.GenerateUniqueAssetPath(path);
            return path;
        }
    }

}
