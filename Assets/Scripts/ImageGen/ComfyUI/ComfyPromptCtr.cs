using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.Events;

[System.Serializable]
public class ResponseData
{
    public string prompt_id;
}

public class ComfyPromptCtr : MonoBehaviour
{
    [TextArea(3, 100)] public string promptJson;
    public UnityEvent<string> OnQueuePrompt;

    public void QueuePrompt(string prompt)
    {
        ChangeSeedInJson();
        StartCoroutine(QueuePromptCoroutine(prompt));
    }

    public void QueuePromptWithControlNet(string prompt, string controlNetImage)
    {
        ChangeSeedInJson();
        StartCoroutine(QueuePromptCoroutineWithControlNet(prompt, controlNetImage));
    }

    private IEnumerator QueuePromptCoroutine(string positivePrompt)
    {
        string url = "http://127.0.0.1:8188/prompt";
        string promptText = GeneratePromptJson();
        promptText = promptText.Replace("Pprompt", positivePrompt);
        Debug.Log(promptText);

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(promptText);
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(request.error);
        }
        else
        {
            Debug.Log("Prompt queued successfully." + request.downloadHandler.text);

            ResponseData data = JsonUtility.FromJson<ResponseData>(request.downloadHandler.text);
            Debug.Log("Prompt ID: " + data.prompt_id);
            GetComponent<ComfyWebsocket>().promptID = data.prompt_id;
            OnQueuePrompt?.Invoke(data.prompt_id);
        }
    }
    private IEnumerator QueuePromptCoroutineWithControlNet(string positivePrompt, string controlNetImage)
    {
        string url = "http://127.0.0.1:8188/prompt";
        string promptText = GeneratePromptJsonWithControlNet(GetControlNetJson(controlNetImage));
        promptText = promptText.Replace("Pprompt", positivePrompt);
        Debug.Log(promptText);

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(promptText);
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(request.error);
        }
        else
        {
            Debug.Log("Prompt queued successfully." + request.downloadHandler.text);

            ResponseData data = JsonUtility.FromJson<ResponseData>(request.downloadHandler.text);
            Debug.Log("Prompt ID: " + data.prompt_id);
            GetComponent<ComfyWebsocket>().promptID = data.prompt_id;
            OnQueuePrompt?.Invoke(data.prompt_id);
        }
    }

    private string GeneratePromptJson()
    {
        string guid = Guid.NewGuid().ToString();

        string promptJsonWithGuid = $@"
        {{
            ""id"": ""{guid}"",
            ""prompt"": {promptJson}
        }}";

        return promptJsonWithGuid;
    }

    private string GeneratePromptJsonWithControlNet(string json)
    {
        string guid = Guid.NewGuid().ToString();

        string promptJsonWithGuid = $@"
        {{
            ""id"": ""{guid}"",
            ""prompt"": {json}
        }}";

        return promptJsonWithGuid;
    }

    public void ChangeSeedInJson()
    {
        string seedPattern = "\"seed\": ";
        int seedIndex = promptJson.IndexOf(seedPattern);

        if (seedIndex == -1)
        {
            Console.WriteLine("Seed field not found in JSON.");
            return;
        }

        int valueStartIndex = seedIndex + seedPattern.Length;
        int valueEndIndex = promptJson.IndexOf(',', valueStartIndex);

        if (valueEndIndex == -1)
        {
            Console.WriteLine("Could not determine the end of the seed value.");
            return;
        }

        string oldSeed = promptJson.Substring(valueStartIndex, valueEndIndex - valueStartIndex);

        int maxSeedValue = int.MaxValue;
        int randomSeed = Mathf.Abs(UnityEngine.Random.Range(1, maxSeedValue));
        promptJson = promptJson.Replace(oldSeed, randomSeed.ToString());
    }

    public string GetControlNetJson(string controlNetImage)
    {
        string jsonCopy = promptJson;
        return jsonCopy.Replace("placeholder.png", controlNetImage + ".png");
    }

    private void OnDisable()
    {
        OnQueuePrompt = null;
    }
}