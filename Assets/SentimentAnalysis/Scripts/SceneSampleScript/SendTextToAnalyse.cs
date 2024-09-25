using UnityEngine;
using System.Collections;
using System.Threading;
using UnityEngine.UI;
using UnitySentiment;

public class SendTextToAnalyse : MonoBehaviour {

	public SentimentAnalysis predictionObject ;
	public InputField textToSend;

	public Image ChangeSentimentalColor;
	public Color PositiveResponse;
	public Color NegativeResponse;
	public Color NeutralResponse;

	public Text PositivePercent;
	public Text NegativePercent;
	public Text NeutralPercent;

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

	public void SendPredictionText()
	{
		// Thread-safe computations
		predictionObject.PredictSentimentText(textToSend.text);

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
		while(!responseFromThread) // Waiting For the response
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
		PositivePercent.text = SentimentAnalysisResponse.x + " % : Positive"; 
		NegativePercent.text = SentimentAnalysisResponse.y + " % : Negative";
		NeutralPercent.text = SentimentAnalysisResponse.z + " % : Neutral";
		
		if ( SentimentAnalysisResponse.x >  SentimentAnalysisResponse.y &&  SentimentAnalysisResponse.x >  SentimentAnalysisResponse.z)
		{
			ChangeSentimentalColor.color = PositiveResponse;
		}
		else if (SentimentAnalysisResponse.y >  SentimentAnalysisResponse.x &&  SentimentAnalysisResponse.y >  SentimentAnalysisResponse.z)
		{
			ChangeSentimentalColor.color = NegativeResponse;
		}
		else if (SentimentAnalysisResponse.z >  SentimentAnalysisResponse.x &&  SentimentAnalysisResponse.z >  SentimentAnalysisResponse.y)
		{
			ChangeSentimentalColor.color = NeutralResponse;
		}
	}

	// Sentiment Analysis Thread
	private void Errors(int errorCode, string errorMessage)
	{
		Debug.Log(errorMessage + "\nCode: " + errorCode);
	}
}