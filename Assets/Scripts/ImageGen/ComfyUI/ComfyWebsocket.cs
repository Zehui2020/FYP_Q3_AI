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

    public ComfyImageCtr comfyImageCtr;
    [HideInInspector] public string promptID;

    [HideInInspector] public int currentProgress;
    [HideInInspector] public int maxProgress;

    private bool isConnected = false;

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

            string response = stringBuilder.ToString();
            //Debug.Log("Received: " + response);

            currentProgress = ParseJsonValue(response, "value");
            maxProgress = ParseJsonValue(response, "max");

            if (response.Contains("\"queue_remaining\": 0"))
            {
                if (isConnected)
                    comfyImageCtr.RequestFileName(promptID);
                else
                    isConnected = true;
            }
        }
    }

    private int ParseJsonValue(string json, string key)
    {
        // Find the key in the JSON string
        int keyIndex = json.IndexOf($"\"{key}\"");
        if (keyIndex == -1)
        {
            return -1;
        }

        // Find the colon after the key
        int colonIndex = json.IndexOf(':', keyIndex);
        if (colonIndex == -1)
        {
            return -1;
        }

        // Extract the value part (assumes the value is an integer)
        int commaIndex = json.IndexOf(',', colonIndex);
        int endIndex = commaIndex != -1 ? commaIndex : json.Length;

        string valueString = json.Substring(colonIndex + 1, endIndex - colonIndex - 1).Trim();

        // Parse the value to an integer
        if (int.TryParse(valueString, out int result))
        {
            return result;
        }
        else
        {
            Debug.LogError($"Failed to parse value for key \"{key}\".");
            return -1;
        }
    }

    void OnDestroy()
    {
        if (ws != null && ws.State == WebSocketState.Open)
            ws.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
    }
}