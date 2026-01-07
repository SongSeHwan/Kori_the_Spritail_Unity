using UnityEditor;
using UnityEngine;

public class PNGBatchResizer : EditorWindow
{
    private int targetSize = 1024;

    [MenuItem("Tools/Resize PNG Textures")]
    public static void Open()
    {
        GetWindow<PNGBatchResizer>("PNG Resizer");
    }

    private void OnGUI()
    {
        targetSize = EditorGUILayout.IntField("Target Size", targetSize);

        if (GUILayout.Button("Resize All PNGs"))
        {
            ResizeAllPNGs(targetSize);
        }
    }

    static void ResizeAllPNGs(int size)
    {
        string[] paths = AssetDatabase.FindAssets("t:Texture2D");

        foreach (string guid in paths)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);

            if (!path.EndsWith(".png")) continue;

            var importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer == null) continue;

            importer.maxTextureSize = size;
            importer.SaveAndReimport();
        }

        Debug.Log($"PNG resize ¿Ï·á: Max {size}px");
    }
}
