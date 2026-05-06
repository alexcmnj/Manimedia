#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public class CreateBorder : EditorWindow
{
    [MenuItem("Tools/Create Border Sprite")]
    static void Create()
    {
        int size = 32;
        int border = 2;

        Texture2D tex = new Texture2D(size, size);
        Color transparent = new Color(0, 0, 0, 0);
        Color white = Color.white;

        // Llenar todo de transparente
        for (int x = 0; x < size; x++)
            for (int y = 0; y < size; y++)
                tex.SetPixel(x, y, transparent);

        // Dibujar solo el borde
        for (int x = 0; x < size; x++)
            for (int y = 0; y < size; y++)
                if (x < border || x >= size - border ||
                    y < border || y >= size - border)
                    tex.SetPixel(x, y, white);

        tex.Apply();

        byte[] png = tex.EncodeToPNG();
        string path = "Assets/Sprites/BorderSprite.png";

        System.IO.Directory.CreateDirectory("Assets/Sprites");
        System.IO.File.WriteAllBytes(path, png);
        AssetDatabase.Refresh();

        // Configurar como sprite sliced
        TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
        importer.textureType = TextureImporterType.Sprite;
        importer.spriteBorder = new Vector4(4, 4, 4, 4);
        importer.SaveAndReimport();

        Debug.Log("✅ BorderSprite creado en Assets/Sprites/BorderSprite.png");
    }
}
#endif