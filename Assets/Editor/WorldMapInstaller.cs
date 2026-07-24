using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Luminia.Editor
{
    [InitializeOnLoad]
    public static class WorldMapInstaller
    {
        private const string EncodedSourcePath = "SourceAssets/world_map.png.b64";
        private const string GeneratedAssetPath = "Assets/Resources/Art/world_map.png";

        static WorldMapInstaller()
        {
            EditorApplication.delayCall += EnsureWorldMapExists;
        }

        [MenuItem("Luminia/Restore World Map")]
        public static void EnsureWorldMapExists()
        {
            try
            {
                if (!File.Exists(GeneratedAssetPath))
                {
                    RestoreEncodedImage();
                }

                ConfigureAsSprite();
            }
            catch (Exception exception)
            {
                Debug.LogError("Failed to restore the Luminia world map. " + exception);
            }
        }

        private static void RestoreEncodedImage()
        {
            if (!File.Exists(EncodedSourcePath))
            {
                throw new FileNotFoundException("Encoded world map source is missing.", EncodedSourcePath);
            }

            var directory = Path.GetDirectoryName(GeneratedAssetPath);
            if (!string.IsNullOrEmpty(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var encoded = File.ReadAllText(EncodedSourcePath);
            var imageBytes = Convert.FromBase64String(encoded);
            File.WriteAllBytes(GeneratedAssetPath, imageBytes);
            AssetDatabase.ImportAsset(GeneratedAssetPath, ImportAssetOptions.ForceSynchronousImport);
            Debug.Log("Restored generated world map: " + GeneratedAssetPath);
        }

        private static void ConfigureAsSprite()
        {
            var importer = AssetImporter.GetAtPath(GeneratedAssetPath) as TextureImporter;
            if (importer == null)
            {
                AssetDatabase.ImportAsset(GeneratedAssetPath, ImportAssetOptions.ForceSynchronousImport);
                importer = AssetImporter.GetAtPath(GeneratedAssetPath) as TextureImporter;
            }

            if (importer == null)
            {
                throw new InvalidOperationException("Unity could not create a TextureImporter for the world map.");
            }

            if (importer.textureType != TextureImporterType.Sprite || importer.mipmapEnabled)
            {
                importer.textureType = TextureImporterType.Sprite;
                importer.spriteImportMode = SpriteImportMode.Single;
                importer.mipmapEnabled = false;
                importer.alphaIsTransparency = true;
                importer.SaveAndReimport();
            }
        }
    }
}
