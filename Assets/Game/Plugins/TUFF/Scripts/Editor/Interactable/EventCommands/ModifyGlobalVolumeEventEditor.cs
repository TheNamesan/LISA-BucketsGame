using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;
using UnityEditor.Rendering;
using UnityEngine.Rendering.Universal;


namespace TUFF.TUFFEditor
{
    [CustomEditor(typeof(ModifyGlobalVolumeEvent)), CanEditMultipleObjects]
    public class ModifyGlobalVolumeEventEditor : EventCommandEditor
    {
        SerializedObject volumeProfileSerialized;
        public override void InspectorGUIContent()
        {
            var mods = target as ModifyGlobalVolumeEvent;
            VolumeComponentListEditor editor = new VolumeComponentListEditor(this);
            if(volumeProfileSerialized == null) volumeProfileSerialized = new SerializedObject(mods.volumeProfile);
            editor.Init(mods.volumeProfile, volumeProfileSerialized);
            editor.OnGUI();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("volumeProfile"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("colorAdjustmentsMods"));
            
            //EditorGUILayout.PropertyField(serializedObject.FindProperty("colorAdjustmentsMods"));
        }
        public override void SummaryGUI(Rect position)
        {
            EditorGUI.LabelField(position, "Modify Global Volume");
        }
    }

}
