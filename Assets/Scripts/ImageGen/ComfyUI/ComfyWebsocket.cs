using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class ComfyWebsocket : MonoBehaviour
{
    private string serverAddress = "127.0.0.1:8188";
    private string clientId = Guid.NewGuid().ToString();
    private ClientWebSocket ws = new ClientWebSocket();

    [HideInInspector] public string response;
    public ComfyImageCtr comfyImageCtr;
    public string promptID;

    [HideInInspector] public int currentProgress = -1;
    [HideInInspector] public int maxProgress = -1;

    [SerializeField] private bool saveImageAfter = true;

    public async void InitWebsocket()
    {
        await ws.ConnectAsync(new Uri($"ws://{serverAddress}/ws?clientId={clientId}"), CancellationToken.None);
        StartListening();
    }

    private async void StartListening()
    {
        var buffer = new byte[1024 * 4];
        WebSocketReceiveResult result = null;

        while (ws.State == WebSocketState.Open)
        {
            var stringBuilder = new StringBuilder();
            do
            {
                result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                if (result.MessageType == WebSocketMessageType.Close)
                {
                    await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
                }
                else
                {
                    var str = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    stringBuilder.Append(str);
                }
            }
            while (!result.EndOfMessage);

            response = stringBuilder.ToString();
            //Debug.Log("Received: " + response);

            if (ParsePromptID(response).Equals(promptID))
            {
                currentProgress = Utility.ParseJsonValue(response, "value");
                maxProgress = Utility.ParseJsonValue(response, "max");
            }
            else
            {
                currentProgress = 0;
                maxProgress = 0;
            }

            if (response.Contains("\"queue_remaining\": 0") && promptID != string.Empty && promptID != "0")
            {
                if (saveImageAfter)
                    comfyImageCtr.RequestFileName(promptID);

                currentProgress = 0;
                maxProgress = 0;
                response = string.Empty;
            }
        }
    }


    private string ParsePromptID(string json)
    {
        string promptIdKey = "\"prompt_id\": \"";
        int startIndex = json.IndexOf(promptIdKey) + promptIdKey.Length;
        int endIndex = json.IndexOf("\"", startIndex);
        return json.Substring(startIndex, endIndex - startIndex);
    }

    void OnDestroy()
    {
        if (ws != null && ws.State == WebSocketState.Open)
            ws.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
    }
}