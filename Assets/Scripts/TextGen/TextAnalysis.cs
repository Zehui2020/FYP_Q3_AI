using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;
using UnitySentiment;

public class TextAnalysis : MonoBehaviour
{
    public SentimentAnalysis predictionObject;

    private bool responseFromThread = false;
    private bool threadStarted = false;
    private Vector3 SentimentAnalysisResponse;
    void OnEnable()
    {
        Application.runInBackground = true;
        // Initialize the local database
        predictionObject.Initialize();
        // Listedn to the Events
        // Sentiment analysis response
        SentimentAnalysis.OnAnlysisFinished += GetAnalysisFromThread;
        // Error response
        SentimentAnalysis.OnErrorOccurs += Errors;
    }

    void OnDestroy()
    {
        // Unload Listeners
        SentimentAnalysis.OnAnlysisFinished -= GetAnalysisFromThread;
        SentimentAnalysis.OnErrorOccurs -= Errors;
    }

    public void SendPredictionText(string AI_Text_To_Analyse)
    {
        // Thread-safe computations
        predictionObject.PredictSentimentText(AI_Text_To_Analyse);

        if (!threadStarted)
        {// Thread Started
            threadStarted = true;
            StartCoroutine(WaitResponseFromThread());
        }
    }

    // Sentiment Analysis Thread
    private void GetAnalysisFromThread(Vector3 analysisResult)
    {
        SentimentAnalysisResponse = analysisResult;
        responseFromThread = true;
        //trick to call method to the main Thread
    }

    private IEnumerator WaitResponseFromThread()
    {
        while (!responseFromThread) // Waiting For the response
        {
            yield return null;
        }
        // Main Thread Action
        PrintAnalysis();
        // Reset
        responseFromThread = false;
        threadStarted = false;
    }

    private void PrintAnalysis()
    {
        Debug.Log(SentimentAnalysisResponse.x + " % : Positive");
        Debug.Log(SentimentAnalysisResponse.y + " % : Negative");
        Debug.Log(SentimentAnalysisResponse.z + " % : Neutral");
    }

    // Sentiment Analysis Thread
    private void Errors(int errorCode, string errorMessage)
    {
        Debug.Log(errorMessage + "\nCode: " + errorCode);
    }
}
