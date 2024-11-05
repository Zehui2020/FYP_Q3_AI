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
            loadTexture = Resources.Load<Texture2D>("FYP_Q3_AI_" + fileName);
            loadTexture.filterMode = FilterMode.Point;

            return Sprite.Create(loadTexture, new Rect(0f, 0f, loadTexture.width, loadTexture.height), new Vector2(0.5f, 0.5f));
        }

        textureBytes = File.ReadAllBytes(Application.persistentDataPath + "_" + fileName);
        loadTexture = new Texture2D(0, 0);
        loadTexture.LoadImage(textureBytes);
        loadTexture.filterMode = FilterMode.Point;
        return Sprite.Create(loadTexture, new Rect(0f, 0f, loadTexture.width, loadTexture.height), new Vector2(0.5f, 0.5f));
    }
    public Sprite GetSpriteFromLocalDisk(string fileName, Vector2 pivot)
    {
        byte[] textureBytes;
        Texture2D loadTexture;

        if (!File.Exists(Application.persistentDataPath + "_" + fileName))
        {
            loadTexture = Resources.Load<Texture2D>("FYP_Q3_AI_" + fileName);
            loadTexture.filterMode = FilterMode.Point;

            return Sprite.Create(loadTexture, new Rect(0f, 0f, loadTexture.width, loadTexture.height), pivot);
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
            loadTexture = Resources.Load<Texture2D>("FYP_Q3_AI_" + fileName);
            loadTexture.filterMode = FilterMode.Point;

            return loadTexture;
        }

        textureBytes = File.ReadAllBytes(Application.persistentDataPath + "_" + fileName);
        loadTexture = new Texture2D(0, 0);
        loadTexture.LoadImage(textureBytes);
        loadTexture.filterMode = FilterMode.Point;

        return loadTexture;
    }

    public enum TileSize
    {
        Small = 500,
        Medium = 400,
        Large = 200
    }

    public Sprite GetTileSprite(string fileName, TileSize tileSize)
    {
        Texture2D texture = GetTextureFromLocalDisk(fileName);
        return Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0f), (int)tileSize);
    }
}