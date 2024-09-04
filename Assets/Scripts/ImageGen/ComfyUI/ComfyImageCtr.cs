using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine.Events;

public class ComfyImageCtr: MonoBehaviour
{
    public UnityEvent<string, Texture2D> OnRecieveImage;

    private string currentID;
    [SerializeField] private bool isControlNet;

    public void RequestFileName(string id)
    {
        StartCoroutine(RequestFileNameRoutine(id));
    }

    private IEnumerator RequestFileNameRoutine(string promptID)
    {
        string url = "http://127.0.0.1:8188/history/" + promptID;

        Debug.Log(url);

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
                    //Debug.Log(":\nReceived: " + webRequest.downloadHandler.text);
                    currentID = promptID;
                    string imageURL = "http://127.0.0.1:8188/view?filename=" + ExtractFilename(webRequest.downloadHandler.text);
                    Debug.Log(imageURL);
                    StartCoroutine(DownloadImage(imageURL));
                    break;
            }
        }
    }
    
    string ExtractFilename(string jsonString)
    {
        // Step 1: Identify the part of the string that contains the "filename" key
        string keyToLookFor;
        if (!isControlNet)
            keyToLookFor = "\"filename\":";
        else
            keyToLookFor = "\"24\": {\"images\": [{\"filename\":";

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
        //Debug.Log(filename);
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
                OnRecieveImage?.Invoke(currentID, texture);
            }
            else
            {
                Debug.LogError("Image download failed: " + webRequest.error);
            }
        }
    }

    private void OnDisable()
    {
        OnRecieveImage = null;
    }
}