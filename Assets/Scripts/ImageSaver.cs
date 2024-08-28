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
        File.WriteAllBytes(Application.persistentDataPath + fileName, textureBytes);
        Debug.Log("Saved to local disk!");
    }

    public Texture2D GetTextureFromLocalDisk(string fileName)
    {
        if (!File.Exists(Application.persistentDataPath + fileName))
        {
            Debug.Log("File does not exist on local disk!");
            return null;
        }

        byte[] textureBytes = File.ReadAllBytes(Application.persistentDataPath + fileName);
        Texture2D loadTexture = new Texture2D(0, 0);
        loadTexture.LoadImage(textureBytes);
        return loadTexture;
    }
}