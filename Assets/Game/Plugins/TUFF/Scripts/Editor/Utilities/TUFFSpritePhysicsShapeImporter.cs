using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

namespace TUFF.TUFFEditor
{
    public class TUFFSpritePhysicsShapeImporter
    {

        public static void SetPhysicsShape(TextureImporter importer, string spriteName, List<Vector2[]> data)
        {
            var importerSO = new SerializedObject(importer);

            //Get Property
            var physicsShapeProperty = GetPhysicsShapeProperty(importer, spriteName);
            physicsShapeProperty.ClearArray();

            //For each shape in data, apply to target Sprite
            for (int j = 0; j < data.Count; j++)
            {
                physicsShapeProperty.InsertArrayElementAtIndex(j);
                var o = data[j];
                var outlinePathSP = physicsShapeProperty.GetArrayElementAtIndex(0);
                outlinePathSP.ClearArray();
                //For each shape point, apply to target Sprite
                for (int i = 0; i < o.Length; i++)
                {
                    outlinePathSP.InsertArrayElementAtIndex(i);
                    outlinePathSP.GetArrayElementAtIndex(i).vector2Value = o[i];
                }
            }
            physicsShapeProperty.serializedObject.ApplyModifiedPropertiesWithoutUndo();
            physicsShapeProperty.serializedObject.Update();
            importerSO.ApplyModifiedPropertiesWithoutUndo();
        }
        private static SerializedProperty GetPhysicsShapeProperty(TextureImporter importer, string spriteName)
        {
            SerializedObject serializedImporter = new SerializedObject(importer);

            if (importer.spriteImportMode == SpriteImportMode.Multiple)
            {
                var spriteSheetSP = serializedImporter.FindProperty("m_SpriteSheet.m_Sprites");

                for (int i = 0; i < spriteSheetSP.arraySize; i++)
                {
                    if (importer.spritesheet[i].name == spriteName)
                    {
                        var element = spriteSheetSP.GetArrayElementAtIndex(i);
                        return element.FindPropertyRelative("m_PhysicsShape");
                    }
                }
            }
            return serializedImporter.FindProperty("m_SpriteSheet.m_PhysicsShape");
        }
        public static void Save(TextureImporter importer)
        {
            AssetDatabase.ForceReserializeAssets(new string[] { importer.assetPath }, ForceReserializeAssetsOptions.ReserializeMetadata);
            importer.SaveAndReimport();
        }
    }
}

