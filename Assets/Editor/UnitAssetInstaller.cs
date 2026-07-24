using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Luminia.Editor
{
    [InitializeOnLoad]
    public static class UnitAssetInstaller
    {
        private static readonly string[] UnitNames =
        {
            "guardian", "archer", "healer", "mage", "demon_soldier", "demon_archer"
        };

        static UnitAssetInstaller()
        {
            EditorApplication.delayCall += RestoreAll;
        }

        [MenuItem("Luminia/Restore Unit Art")]
        public static void RestoreAll()
        {
            foreach (var unitName in UnitNames)
            {
                Restore(unitName);
            }
        }

        private static void Restore(string unitName)
        {
            var source = "SourceAssets/Units/" + unitName + ".png.b64";
            var destination = "Assets/Resources/Art/Units/" + unitName + ".png";
            try
            {
                if (!File.Exists(destination))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(destination));
                    File.WriteAllBytes(destination, Convert.FromBase64String(File.ReadAllText(source)));
                    AssetDatabase.ImportAsset(destination, ImportAssetOptions.ForceSynchronousImport);
                }

                var importer = AssetImporter.GetAtPath(destination) as TextureImporter;
                if (importer != null &&
                    (importer.textureType != TextureImporterType.Sprite || importer.filterMode != FilterMode.Point))
                {
                    importer.textureType = TextureImporterType.Sprite;
                    importer.spriteImportMode = SpriteImportMode.Single;
                    importer.filterMode = FilterMode.Point;
                    importer.textureCompression = TextureImporterCompression.Uncompressed;
                    importer.mipmapEnabled = false;
                    importer.alphaIsTransparency = true;
                    importer.spritePixelsPerUnit = 48;
                    importer.SaveAndReimport();
                }
            }
            catch (Exception exception)
            {
                Debug.LogError("Could not restore unit art '" + unitName + "'. " + exception);
            }
        }
    }
}
