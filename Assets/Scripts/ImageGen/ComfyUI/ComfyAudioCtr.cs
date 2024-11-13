using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine.Events;
using System;

public class ComfyAudioCtr : MonoBehaviour
{
    private string currentID;
    [SerializeField] private bool isControlNet;
    [SerializeField] private string comfyAudioFilePath;

    public UnityEvent<string, AudioClip> OnRecieveAudio;

    public void SetCurrentID(string currentID)
    {
        this.currentID = currentID;
    }

    public void RequestFileName(string id)
    {
        if (currentID != id)
            return;
        
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
                    string audioURL = "http://127.0.0.1:8188/view?filename=" + ExtractFilename(webRequest.downloadHandler.text);
                    Debug.Log(audioURL);
                    StartCoroutine(DownloadAudio(audioURL));
                    break;
            }
        }
    }
    
    string ExtractFilename(string jsonString)
    {
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

        startIndex += keyToLookFor.Length;

        string fromFileName = jsonString.Substring(startIndex);

        int endIndex = fromFileName.IndexOf(',');

        string filenameWithQuotes = fromFileName.Substring(0, endIndex).Trim();

        string filename = filenameWithQuotes.Trim('"');
        return filename;
    }
    
    private IEnumerator DownloadAudio(string imageUrl)
    {
        yield return new WaitForSeconds(0.5f);

        string fileName = ExtractFilenameFromURL(imageUrl);
        imageUrl = "file://" + comfyAudioFilePath  + "/"  + fileName;
        Debug.Log("Image URL: " + imageUrl);
        using (UnityWebRequest webRequest =  UnityWebRequestMultimedia.GetAudioClip(imageUrl, AudioType.UNKNOWN))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                // Get the downloaded texture
                AudioClip audio = ((DownloadHandlerAudioClip)webRequest.downloadHandler).audioClip;
                OnRecieveAudio?.Invoke(currentID, audio);
            }
            else
            {
                Debug.LogError("Audio download failed: " + webRequest.error);
            }
        }
    }

    private string ExtractFilenameFromURL(string url)
    {
        int filenameIndex = url.IndexOf("filename=");

        if (filenameIndex == -1)
        {
            Console.WriteLine("Filename parameter not found in URL.");
            return null;
        }

        string filename = url.Substring(filenameIndex + "filename=".Length);

        return filename;
    }

    private void OnDisable()
    {
        OnRecieveAudio = null;
    }
}