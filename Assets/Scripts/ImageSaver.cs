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
        if (!File.Exists(Application.persistentDataPath + "_" + fileName))
        {
            Debug.Log("File does not exist on local disk!");
            return null;
        }

        byte[] textureBytes = File.ReadAllBytes(Application.persistentDataPath + "_" + fileName);
        Texture2D loadTexture = new Texture2D(0, 0);
        loadTexture.LoadImage(textureBytes);
        return Sprite.Create(loadTexture, new Rect(0f, 0f, loadTexture.width, loadTexture.height), new Vector2(0.5f, 0.5f));
    }

    public Texture2D GetTextureFromLocalDisk(string fileName)
    {
        if (!File.Exists(Application.persistentDataPath + "_" + fileName))
        {
            Debug.Log("File does not exist on local disk!");
            return null;
        }

        byte[] textureBytes = File.ReadAllBytes(Application.persistentDataPath + "_" + fileName);
        Texture2D loadTexture = new Texture2D(0, 0);
        loadTexture.LoadImage(textureBytes);
        return loadTexture;
    }
}