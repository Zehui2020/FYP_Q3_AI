using UnityEngine;
using System.Collections;
using UnityEditor;
using UnitySentiment;

[CustomEditor(typeof(SentimentAnalysis))]
public class SentimentAnalysisEditor : Editor {

	private SentimentAnalysis sentimentAnalysis;

	void OnEnable ()
	{
		this.sentimentAnalysis = (SentimentAnalysis)target;
	}

	public override void OnInspectorGUI()
	{
		GUILayout.Space(10f);
		
		sentimentAnalysis.SetPositiveWordsFileText(EditorGUILayout.TextField("Path to Positive weighted Words", sentimentAnalysis.GetPositiveWordsFileText()));
		sentimentAnalysis.SetNegativeWordsFileText(EditorGUILayout.TextField("Path to Negative weighted Words", sentimentAnalysis.GetNegativeWordsFileText()));
		sentimentAnalysis.SetStopWordsFileText(EditorGUILayout.TextField("Path to Stop Words", sentimentAnalysis.GetStopWordsFileText()));
		sentimentAnalysis.SetContractionWordsFileText(EditorGUILayout.TextField("Path to Contraction Words", sentimentAnalysis.GetContractionWordsFileText()));
		
		GUILayout.Label("System Morale", EditorStyles.boldLabel);
		
		GUILayout.BeginHorizontal();
		{
			GUILayout.Label("Positive", EditorStyles.label);
			sentimentAnalysis.SetSentimentValue(EditorGUILayout.Slider(sentimentAnalysis.GetSentimentValue(), 0f, 1f));
			GUILayout.Label("Negative", EditorStyles.label);
		}
		GUILayout.EndHorizontal();
		
		EditorGUILayout.HelpBox("The \"System Morale\" helps the Bot to focus more on Positive or Negative expressions.\n\nIt changes the overall System sensibility", MessageType.Info);
	}
}

[CustomEditor(typeof(WordsStemmer))]
public class WordsStemmerEditor : Editor {
	
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();
		EditorGUILayout.HelpBox("In order to Stem words you have to divide every single word from each other with a \',\' in the .txt file", MessageType.Warning);
	}
}

[CustomEditor(typeof(SentimentWeightCommonWords))]
public class SentimentWeightCommonWordsEditor : Editor {
	
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();
		EditorGUILayout.HelpBox("This scripts valuate the importance of each positive / negative word used by the System Bot", MessageType.Info);
		EditorGUILayout.HelpBox("For better results, add positive / negative words to the file in the Common File Path", MessageType.Info);
	}
}

[CustomEditor(typeof(SentimentDataTraining))]
public class SentimentDataTrainingEditor : Editor {
	
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();
		EditorGUILayout.HelpBox("Increase the number of Positive / Negative Training Data for better results", MessageType.Info);
	}
}