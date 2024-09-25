using UnityEngine;
using System.IO;
using System.Threading;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class SentimentWeightCommonWords : MonoBehaviour {

	public string PathToTrainedDataFile;
	public string PathToCommonPositiveWords;
	public string PathToCommonNegativeWords;

	public string PathToSaveDataPositive;
	public string PathToSaveDataNegative;

	private string[] trainedData;
	private string[] positiveWords;
	private string[] negativeWords;

	private List <string> trainedWords = new List<string>();
	private List <string> trainedValues = new List<string>();

	public void SaveTrainedData () 
	{
		Debug.Log("Saving ...");

		trainedData = OpenFile(PathToTrainedDataFile, ';');
		positiveWords = OpenFile(PathToCommonPositiveWords, ',');
		negativeWords = OpenFile(PathToCommonNegativeWords, ',');

		for (int i = 0; i < trainedData.Length; i++)
		{
			if (!string.IsNullOrEmpty( trainedData[i] ))
			{
				try
				{
					string word = trainedData[i].Split(':')[0];
					string value = trainedData[i].Split(':')[1];

					if (!string.IsNullOrEmpty( word ) 
					    && ! string.IsNullOrEmpty( value ))
					{
						trainedWords.Add(word);
						trainedValues.Add(value);
					}
				}
				catch
				{
					print(trainedData[i]);
				}
			}
		}

		positiveWords = WeightedCommonData(positiveWords);
		negativeWords = WeightedCommonData(negativeWords);

		SaveTheWeightedData(PathToSaveDataPositive, positiveWords);
		SaveTheWeightedData(PathToSaveDataNegative, negativeWords);

		Debug.Log("Fineshed!!");
	}

	private string[] WeightedCommonData(string[] data)
	{
		for (int i = 0; i < data.Length; i++)
		{
			if (!string.IsNullOrEmpty(data[i]))
			{
				if ( trainedWords.Contains(data[i]) )
				{
					int index = trainedWords.IndexOf(data[i]);

					data[i] += ":" + trainedValues[index];
				}
				else
				{
					data[i] += ":1";
				}
			}
		}

		return data;
	}

	private void SaveTheWeightedData(string path, string[] data)
	{
		try
		{
			string createText = System.String.Empty;
			
			for (int i = 0; i < data.Length; i++)
			{
				createText += data[i] + ",";
			}
			
			File.WriteAllText(path, createText);
		}
		catch
		{
			Debug.Log("Error Saving The Data");
		}
	}

	private string[] OpenFile(string filePath, char splitChar)
	{
		StreamReader theReader = new StreamReader(filePath, Encoding.Default);		
		try
		{
			using (theReader)
			{
				string[] textOfFile = theReader.ReadToEnd().Trim().Split(splitChar); 
				theReader.Close();
				return textOfFile;
			}	
		}	
		catch
		{
			return new string[0];
		}
	}
}
