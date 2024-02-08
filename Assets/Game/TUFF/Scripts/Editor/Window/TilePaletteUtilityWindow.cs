using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;
using UnityEditor.Tilemaps;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

namespace TUFF.TUFFEditor
{
    public class TilePaletteUtilityWindow : EditorWindow
    {
        private static Vector2 scrollPos = new Vector2();
        private static readonly Vector2 windowMinSize = new Vector2(100f, 200f);
        public static TilePaletteUtilityWindow instance;
        [MenuItem("TUFF/Tile Palette Utility Window")]
        public static void ShowWindow()
        {
            instance = GetWindow<TilePaletteUtilityWindow>("Tile Palette Utility");
            instance.AdjustSize();
        }
        private void AdjustSize()
        {
            minSize = windowMinSize;
        }
        private void AssignUtilValues()
        {
            TilemapUtilValues.hidden = EditorPrefs.GetBool("TilemapUtilValues.hidden", false);
            TilemapUtilValues.tileColor.r = EditorPrefs.GetFloat("TilemapUtilValues.tileColor.r", TilemapUtilValues.defaultColorR);
            TilemapUtilValues.tileColor.g = EditorPrefs.GetFloat("TilemapUtilValues.tileColor.g", TilemapUtilValues.defaultColorG);
            TilemapUtilValues.tileColor.b = EditorPrefs.GetFloat("TilemapUtilValues.tileColor.b", TilemapUtilValues.defaultColorB);
            TilemapUtilValues.tileColor.a = EditorPrefs.GetFloat("TilemapUtilValues.tileColor.a", TilemapUtilValues.defaultColorA);
            TilemapUtilValues.tileSize.x = EditorPrefs.GetFloat("TilemapUtilValues.tileSize.x", 1f);
            TilemapUtilValues.tileSize.y = EditorPrefs.GetFloat("TilemapUtilValues.tileSize.y", 1f);
        }
        public void OnEnable()
        {
            UpdateTilemap(GridPaintingState.scenePaintTarget);
            EditorSceneManager.activeSceneChanged += UpdateTilemap;
            GridPaintingState.scenePaintTargetChanged += UpdateTilemap;
            AssignUtilValues();
        }
        public void OnDisable()
        {
            EditorSceneManager.activeSceneChanged -= UpdateTilemap;
            GridPaintingState.scenePaintTargetChanged -= UpdateTilemap;
        }
        private void OnGUI()
        {
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Height(position.height));
            Draw();
            EditorGUILayout.EndScrollView();
        }
        private void Draw()
        {
            // Hidden
            EditorGUI.BeginChangeCheck();
            TilemapUtilValues.hidden = EditorGUILayout.Toggle("Hidden", TilemapUtilValues.hidden);
            if (EditorGUI.EndChangeCheck())
            {
                EditorPrefs.SetBool("TilemapUtilValues.hidden", TilemapUtilValues.hidden);
                SceneView.RepaintAll(); // Updating Scene view to update gizmo visibility
            }
            
            // Color
            EditorGUI.BeginChangeCheck();
            TilemapUtilValues.tileColor = EditorGUILayout.ColorField("Color", TilemapUtilValues.tileColor);
            if (EditorGUI.EndChangeCheck())
            {
                EditorPrefs.SetFloat("TilemapUtilValues.tileColor.r", TilemapUtilValues.tileColor.r);
                EditorPrefs.SetFloat("TilemapUtilValues.tileColor.g", TilemapUtilValues.tileColor.g);
                EditorPrefs.SetFloat("TilemapUtilValues.tileColor.b", TilemapUtilValues.tileColor.b);
                EditorPrefs.SetFloat("TilemapUtilValues.tileColor.a", TilemapUtilValues.tileColor.a);
                SceneView.RepaintAll(); // Updating Scene view to update gizmo color
            }

            // Size
            EditorGUI.BeginChangeCheck();
            TilemapUtilValues.tileSize = EditorGUILayout.Vector2Field("Tile Size", TilemapUtilValues.tileSize);
            if (EditorGUI.EndChangeCheck())
            {
                EditorPrefs.SetFloat("TilemapUtilValues.tileSize.x", TilemapUtilValues.tileSize.x);
                EditorPrefs.SetFloat("TilemapUtilValues.tileSize.y", TilemapUtilValues.tileSize.y);
                SceneView.RepaintAll(); // Updating Scene view to update gizmo size
            }
            var gui = GUI.enabled;
            GUI.enabled = false;
            EditorGUILayout.ObjectField("Tilemap", TilemapUtilValues.tilemap, typeof(Tilemap), true);
            GUI.enabled = gui;
        }
        private void UpdateTilemap(Scene current, Scene next)
        { 
            UpdateTilemap(GridPaintingState.scenePaintTarget); 
        }
        private static void UpdateTilemap(GameObject obj)
        {
            if (obj == null) return;
            var activeTilemap = obj?.GetComponent<Tilemap>(); // Get Active Tilemap
            if (activeTilemap != null)
            {
                TilemapUtilValues.tilemap = activeTilemap;
            }
            //else Debug.LogWarning("No instance!");
        }
    }
}
