using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;
using UnityEditor.Tilemaps;

namespace TUFF.TUFFEditor
{
    [CustomEditor(typeof(TilemapHighlighter))]
    public class TilemapHighlighterEditor : Editor
    {
        
        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("Open Tile Palette Utility Window"))
            {
                TilePaletteUtilityWindow.ShowWindow();
            }
            serializedObject.Update();
            serializedObject.ApplyModifiedProperties();
        }
    }
}
