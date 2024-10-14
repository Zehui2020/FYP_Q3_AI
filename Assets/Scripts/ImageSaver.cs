using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ImageSaver : MonoBehaviour
{
    public void SaveImageToLocalDisk(Texture2D texture, string fileName)
    {
        if (texture == null)
        {
            Debug.Log("Nothing to save to local disk");
            return;
        }

        byte[] textureBytes = texture.EncodeToPNG();
        File.WriteAllBytes(Application.persistentDataPath + "_" + fileName, textureBytes);
        Debug.Log("Saved to local disk!");
    }

    public Sprite GetSpriteFromLocalDisk(string fileName)
    {
        byte[] textureBytes;
        Texture2D loadTexture;

        if (!File.Exists(Application.persistentDataPath + "_" + fileName))
        {
            string filePath = Path.Combine(Application.dataPath, "Placeholders", "FYP_Q3_AI_" + fileName);
            textureBytes = File.ReadAllBytes(filePath);
            loadTexture = new Texture2D(0, 0);
            loadTexture.LoadImage(textureBytes);
            loadTexture.filterMode = FilterMode.Point;

            return Sprite.Create(loadTexture, new Rect(0f, 0f, loadTexture.width, loadTexture.height), new Vector2(0.5f, 0.5f));
        }

        textureBytes = File.ReadAllBytes(Application.persistentDataPath + "_" + fileName);
        loadTexture = new Texture2D(0, 0);
        loadTexture.LoadImage(textureBytes);
        loadTexture.filterMode = FilterMode.Point;
        return Sprite.Create(loadTexture, new Rect(0f, 0f, loadTexture.width, loadTexture.height), new Vector2(0.5f, 0.5f));
    }

    public Texture2D GetTextureFromLocalDisk(string fileName)
    {
        byte[] textureBytes;
        Texture2D loadTexture;

        if (!File.Exists(Application.persistentDataPath + "_" + fileName))
        {
            string filePath = Path.Combine(Application.dataPath, "Placeholders", "FYP_Q3_AI_" + fileName);
            textureBytes = File.ReadAllBytes(filePath);
            loadTexture = new Texture2D(0, 0);
            loadTexture.LoadImage(textureBytes);
            loadTexture.filterMode = FilterMode.Point;

            return loadTexture;
        }

        textureBytes = File.ReadAllBytes(Application.persistentDataPath + "_" + fileName);
        loadTexture = new Texture2D(0, 0);
        loadTexture.LoadImage(textureBytes);
        loadTexture.filterMode = FilterMode.Point;

        return loadTexture;
    }
}