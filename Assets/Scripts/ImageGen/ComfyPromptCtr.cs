using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

[System.Serializable]
public class ResponseData
{
    public string prompt_id;
}

public class ComfyPromptCtr : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI promptText;
    public string prompts;
    public string setPrompts;
    public string promptJSON;

    private void Start()
    {
        prompts += setPrompts;
        promptText.text = prompts;
    }
    public void ResetPrompts()
    {
        prompts = string.Empty;
        prompts += setPrompts;
        promptText.text = prompts;
    }

    public void QueuePrompt()
    {
        Debug.Log(prompts);

        StartCoroutine(QueuePromptCoroutine(prompts));
    }

    private IEnumerator QueuePromptCoroutine(string positivePrompt)
    {
        string url = "http://127.0.0.1:8188/prompt";
        string promptText = GeneratePromptJson();
        promptText = promptText.Replace("Pprompt", positivePrompt);
        Debug.Log("Prompt Text: " + promptText);

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
        }
    }

    private string GeneratePromptJson()
    {
        string guid = Guid.NewGuid().ToString();

        string promptJsonWithGuid = $@"
        {{
            ""id"": ""{guid}"",
            ""prompt"": {promptJSON}
        }}";

        return promptJsonWithGuid;
    }

    public void AddPrompt(string newPrompt)
    {
        prompts += ", " + newPrompt;
        promptText.text = prompts;
    }
}