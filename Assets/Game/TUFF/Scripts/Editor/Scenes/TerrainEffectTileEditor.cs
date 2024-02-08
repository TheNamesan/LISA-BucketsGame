using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;
using System.Reflection;

namespace TUFF.TUFFEditor
{
    [CustomEditor(typeof(TerrainEffectTile)), CanEditMultipleObjects]
    public class TerrainEffectTileEditor : Editor
    {
        private const float k_PreviewWidth = 32;
        private const float k_PreviewHeight = 32;

        private SerializedProperty m_Sprite; 

        private Tile tile
        {
            get { return (target as Tile); }
        }
        private static class Styles
        {
            public static readonly GUIContent invalidMatrixLabel = EditorGUIUtility.TrTextContent("Invalid Matrix", "No valid Position / Rotation / Scale components available for this matrix");
            public static readonly GUIContent resetMatrixLabel = EditorGUIUtility.TrTextContent("Reset Matrix");
            public static readonly GUIContent previewLabel = EditorGUIUtility.TrTextContent("Preview", "Preview of tile with attributes set");

            public static readonly GUIContent spriteEditorLabel = EditorGUIUtility.TrTextContent("Sprite Editor");
            public static readonly GUIContent offsetLabel = EditorGUIUtility.TrTextContent("Offset");
            public static readonly GUIContent rotationLabel = EditorGUIUtility.TrTextContent("Rotation");
            public static readonly GUIContent scaleLabel = EditorGUIUtility.TrTextContent("Scale");
        }

        private void OnEnable()
        {
            m_Sprite = serializedObject.FindProperty("m_Sprite");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DoTilePreview(tile.sprite, tile.color, Matrix4x4.identity);
            EditorGUILayout.PropertyField(m_Sprite);

            using (new EditorGUI.DisabledGroupScope(m_Sprite.objectReferenceValue == null))
            {
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (GUILayout.Button(Styles.spriteEditorLabel))
                {
                    Selection.activeObject = m_Sprite.objectReferenceValue;
                    //SpriteUtilityWindow.ShowSpriteEditorWindow();
                }
                GUILayout.EndHorizontal();
            }

            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Color"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_ColliderType"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("terrainData"));

            serializedObject.ApplyModifiedProperties();
        }
        public static void DoTilePreview(Sprite sprite, Color color, Matrix4x4 transform)
        {
            if (sprite == null)
                return;

            Rect guiRect = EditorGUILayout.GetControlRect(false, k_PreviewHeight);
            guiRect = EditorGUI.PrefixLabel(guiRect, new GUIContent(Styles.previewLabel));
            Rect previewRect = new Rect(guiRect.xMin, guiRect.yMin, k_PreviewWidth, k_PreviewHeight);
            Rect borderRect = new Rect(guiRect.xMin - 1, guiRect.yMin - 1, k_PreviewWidth + 2, k_PreviewHeight + 2);

            if (Event.current.type == EventType.Repaint)
                EditorStyles.textField.Draw(borderRect, false, false, false, false);


            Texture2D texture = LISAEditorUtility.SpriteRenderStaticPreview(sprite, color, 32, 32, transform);//(Texture2D)staticPreviewMethod.Invoke(spriteUtility, new object[] { sprite, color, 32, 32, transform });

            //Texture2D texture = SpriteUtility.RenderStaticPreview(sprite, color, 32, 32, transform);
            EditorGUI.DrawTextureTransparent(previewRect, texture, ScaleMode.StretchToFill);
        }

        public override Texture2D RenderStaticPreview(string assetPath, Object[] subAssets, int width, int height)
        {
            return LISAEditorUtility.SpriteRenderStaticPreview(tile.sprite, tile.color, width, height);
        }
    }
}

