using System.IO;
using UnityEditor;
using UnityEngine;

namespace Assets._CrossyroadTest.Editor.Scripts
{
    internal class NamespaceFixer
    {
        [MenuItem("Tools/Fix Namespaces")]
        public static void FixNamespaces()
        {
            string targetFolder = Path.Combine(Application.dataPath, "_CrossyroadTest");


            foreach (var file in Directory.GetFiles(targetFolder, "*.cs", SearchOption.AllDirectories))
            {
                string fullPath = file.Replace("\\", "/");
                int assetsIndex = fullPath.IndexOf("/Assets/");
                if (assetsIndex < 0) continue;

                string relativePath = fullPath.Substring(assetsIndex + 1);
                string directorypath = Path.GetDirectoryName(relativePath).Replace("\\", "/");
                string newNamespace = directorypath.Replace('/', '.');

                string[] lines = File.ReadAllLines(file);

                for (int i = 0; i < lines.Length; i++)
                {
                    if (lines[i].TrimStart().StartsWith("namespace "))
                    {
                        lines[i] = $"namespace {newNamespace}";
                        break;
                    }
                }
                File.WriteAllLines(file, lines);
            }

            AssetDatabase.Refresh();
            Debug.Log("Namespaces updated in Scripts folder.");
        }
    }
}
