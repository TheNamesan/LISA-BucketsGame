using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEngine.SceneManagement;

namespace TUFF.TUFFEditor
{
    public class TUFFWizard
    {
        private static Transform FindSceneProperties()
        {
            if (Application.isPlaying)
            {
                return SceneLoaderManager.currentSceneProperties.transform;
            }
            else
            {
                var activeScene = SceneManager.GetActiveScene();
                if (activeScene.IsValid())
                {
                    var gameObjects = activeScene.GetRootGameObjects();
                    var scenePropGO = System.Array.Find(gameObjects, q => q.gameObject.CompareTag("SceneProperties"));
                    if (!scenePropGO) return null;
                    var sceneProperties = scenePropGO.GetComponent<SceneProperties>();
                    if (sceneProperties) return sceneProperties.transform;
                }
            }
            return null;
        }
        [MenuItem("TUFF/Create/Interactable")]
        public static void CreateInteractable()
        {
            CreateItem(TUFFSettings.interactablePrefab, FindSceneProperties());
        }
        [MenuItem("TUFF/Create/Overworld Character")]
        public static void CreateOverworldCharacter()
        {
            CreateItem(TUFFSettings.overworldCharacterPrefab, FindSceneProperties());
        }
        [MenuItem("TUFF/Stop Preview BGM")]
        public static void StopPreviewBGM()
        {
            GameObject.FindWithTag("AudioManager")?.GetComponent<AudioManager>()?.StopMusic();
        }
        public static void CreateItem(Object item, Transform parent = null)
        {
            if (item == null) return;
            var instance = PrefabUtility.InstantiatePrefab(item, parent);
            Undo.RegisterCreatedObjectUndo(instance, $"Create {instance.name}");
            Selection.activeObject = instance;
        }
        [MenuItem("TUFF/Set Graphic Native Size")]
        public static void SetGraphicSize()
        {
            RectTransform rect;
            Image img;
            if (Selection.activeTransform == null) return;
            Selection.activeTransform.TryGetComponent(out rect);
            if (rect == null) return;
            img = rect.GetComponentInChildren<Image>();
            
            if (img == null) return;
            if (img.sprite == null) return;

            float ratioSize = 10.4f;
            rect.sizeDelta = new Vector2(img.sprite.rect.width * ratioSize, img.sprite.rect.height * ratioSize);
        }
        [MenuItem("TUFF/Sprites/Apply Platform Physics Shape")]
        public static void ApplyPlatformPhysicsShape()
        {
            if (Selection.count <= 1)
            {
                var obj = Selection.activeObject;
                if (obj is Texture2D)
                {
                    var texture = obj as Texture2D;
                    var path = AssetDatabase.GetAssetPath(obj);
                    var importer = AssetImporter.GetAtPath(path) as TextureImporter;
                    Object[] sprites = AssetDatabase.LoadAllAssetsAtPath(path);
                    Undo.RegisterImporterUndo(path, $"Applied Platform Physics Shape to {texture.name}");
                    for (int i = 0; i < sprites.Length; i++)
                    {
                        var asset = sprites[i];
                        if (asset is not Sprite) continue;
                        var sprite = asset as Sprite;
                        List<Vector2[]> data = GetPlatformPhysicsShape(sprite.bounds.size, sprite.pivot);
                        TUFFSpritePhysicsShapeImporter.SetPhysicsShape(importer, sprite.name, data);
                    }
                    TUFFSpritePhysicsShapeImporter.Save(importer);
                    Debug.Log($"Physics Shape Applied to Texture {texture.name}.");
                }
                if (obj is Sprite)
                {
                    var path = AssetDatabase.GetAssetPath(obj);
                    var importer = AssetImporter.GetAtPath(path) as TextureImporter;
                    var sprite = obj as Sprite;
                    Undo.RegisterImporterUndo(path, $"Applied Platform Physics Shape to {sprite.name}");
                    List<Vector2[]> data = GetPlatformPhysicsShape(sprite.bounds.size, sprite.pivot);
                    
                    TUFFSpritePhysicsShapeImporter.SetPhysicsShape(importer, sprite.name, data);
                    TUFFSpritePhysicsShapeImporter.Save(importer);
                    Debug.Log($"Physics Shape Applied to {sprite.name}.");
                }
            }
            else
            {
                int textureCount = 0;
                int spriteCount = 0;
                List<TextureImporter> importers = new List<TextureImporter>();
                for (int i = 0; i < Selection.count; i++)
                {
                    var obj = Selection.objects[i];
                    if (obj is Texture2D)
                    {
                        var path = AssetDatabase.GetAssetPath(obj);
                        var importer = AssetImporter.GetAtPath(path) as TextureImporter;
                        Object[] sprites = AssetDatabase.LoadAllAssetsAtPath(path);
                        if (!importers.Contains(importer))
                        {
                            Undo.RegisterImporterUndo(path, $"Applied Platform Physics Shape to sprite");
                            importers.Add(importer);
                        }
                        for (int j = 0; j < sprites.Length; j++)
                        {
                            var asset = sprites[j];
                            if (asset is not Sprite) continue;
                            var sprite = asset as Sprite;
                            List<Vector2[]> data = GetPlatformPhysicsShape(sprite.bounds.size, sprite.pivot);
                            TUFFSpritePhysicsShapeImporter.SetPhysicsShape(importer, sprite.name, data);
                        }
                        textureCount++;
                    }
                    if (obj is Sprite)
                    {
                        var path = AssetDatabase.GetAssetPath(obj);
                        var importer = AssetImporter.GetAtPath(path) as TextureImporter;
                        var sprite = obj as Sprite;
                        if (!importers.Contains(importer))
                        {
                            Undo.RegisterImporterUndo(path, $"Applied Platform Physics Shape to texture");
                            importers.Add(importer);
                        }
                        List<Vector2[]> data = GetPlatformPhysicsShape(sprite.bounds.size, sprite.pivot);
                        TUFFSpritePhysicsShapeImporter.SetPhysicsShape(importer, sprite.name, data);
                        spriteCount++;
                    }
                }
                for(int i = 0; i < importers.Count; i++)
                {
                    TUFFSpritePhysicsShapeImporter.Save(importers[i]);
                }
                Undo.SetCurrentGroupName("Applied Platform Physics to Sprites and Textures.");
                Undo.CollapseUndoOperations(Undo.GetCurrentGroup());
                Debug.Log($"Physics Shape Applied to {spriteCount} sprites and {textureCount} textures.");
            }
        }
        private static List<Vector2[]> GetPlatformPhysicsShape(Vector3 spriteSize, Vector2 spritePivot)
        {
            const float sep = 3f;
            //Apply pixel sizes
            var s = spriteSize;
            var pivot = spritePivot;
            //var s = sprite.bounds.size;
            //var pivot = sprite.pivot;
            var data = new List<Vector2[]> {
                    new Vector2[] {
                        new Vector2(-s.x * pivot.x, s.y * pivot.y - sep),
                        new Vector2(-s.x * pivot.x, s.y * pivot.y),
                        new Vector2(s.x * pivot.x, s.y * pivot.y),
                        new Vector2(s.x * pivot.x, s.y * pivot.y - sep)
                    }};
            return data;
        }

        [MenuItem("TUFF/Play Game")]
        public static void PlayGame()
        {
            //EditorPrefs.SetString("Test", "Hello World");
            EditorApplication.EnterPlaymode();
        }

        public static void TestBattle(Battle battle)
        {
            if (!battle) return;

            if (Application.isPlaying)
            {
                GameManager.instance.TestBattle(battle);
            }
            else
            {
                string path = AssetDatabase.GetAssetPath(battle);
                Debug.Log(path);
                if (!string.IsNullOrEmpty(path)) EditorPrefs.SetString("Test Battle Path", path);
                EditorApplication.EnterPlaymode();
            }
        }
    }
}
