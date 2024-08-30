using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.U2D;

[System.Serializable]
public class ImageData
{
    public string filename;
    public string subfolder;
    public string type;
}

[System.Serializable]
public class OutputData
{
    public ImageData[] images;
}

[System.Serializable]
public class PromptData
{
    public OutputData outputs;
}

public class ComfyImageCtr: MonoBehaviour
{
    public ImageSaver imageSaver;
    public MenuBackground menuBackground;
    public string fileName;

    public UnityEvent ObtainImage;

    public void RequestFileName(string id)
    {
        if (this == null)
            return;

        StartCoroutine(RequestFileNameRoutine(id));
    }

    private IEnumerator RequestFileNameRoutine(string promptID)
    {
        string url = "http://127.0.0.1:8188/history/" + promptID;
        
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();
            
            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError(": Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError(": HTTP Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.Success:
                    Debug.Log(":\nReceived: " + webRequest.downloadHandler.text);
                    string imageURL = "http://127.0.0.1:8188/view?filename=" +ExtractFilename(webRequest.downloadHandler.text);
                    StartCoroutine(DownloadImage(imageURL));
                    break;
            }
        }
    }
    
    private string ExtractFilename(string jsonString)
    {
        // Step 1: Identify the part of the string that contains the "filename" key
        string keyToLookFor = "\"filename\":";
        int startIndex = jsonString.IndexOf(keyToLookFor);

        if (startIndex == -1)
        {
            return "filename key not found";
        }

        // Adjusting startIndex to get the position right after the keyToLookFor
        startIndex += keyToLookFor.Length;

        // Step 2: Extract the substring starting from the "filename" key
        string fromFileName = jsonString.Substring(startIndex);

        // Assuming that filename value is followed by a comma (,)
        int endIndex = fromFileName.IndexOf(',');

        // Extracting the filename value (assuming it's wrapped in quotes)
        string filenameWithQuotes = fromFileName.Substring(0, endIndex).Trim();

        // Removing leading and trailing quotes from the extracted value
        string filename = filenameWithQuotes.Trim('"');
        return filename;
    }
    
    private IEnumerator DownloadImage(string imageUrl)
    {
        yield return new WaitForSeconds(0.5f);
        using (UnityWebRequest webRequest = UnityWebRequestTexture.GetTexture(imageUrl))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                // Get the downloaded texture
                Texture2D texture = ((DownloadHandlerTexture)webRequest.downloadHandler).texture;
                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
                imageSaver.SaveImageToLocalDisk(sprite.texture, fileName);
                menuBackground.SetupBackground(imageSaver.GetSpriteFromLocalDisk(fileName)); ObtainImage?.Invoke();
            }
            else
            {
                Debug.Log("Image download failed: " + webRequest.error);
            }
        }
    }

    private void OnDisable()
    {
        ObtainImage = null;
    }
}