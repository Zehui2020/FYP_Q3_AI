using System;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using UnitySentiment;
using System.Threading;
using System.Collections;
using System.Collections.Generic;

public class SentimentDataTraining : MonoBehaviour
{
	public string PathToPositiveDataTrainingFolder;
	public string PathToNegativeDataTrainingFolder;
	public string PathStopWordsDataTrainingFile;
	public string PathContractionWordsDataTrainingFile;
	public string PathToSaveTrainedData;
	public string NameOfTrainedData;

	public bool WriteWeightedFile = false;
	public SentimentWeightCommonWords sentimentWeightCommonWords;

	private string[] stopWords;
	private string[] contractionWords;

	private FileInfo currentDataTraining;

	private List<string> Words = new List<string>();
	private List<int> PositiveTimes = new List<int>();
	private List<int> NegativeTimes = new List<int>();

	private int totalReview;

	private readonly object SyncObject = new object();

	void Start()
	{
		Application.runInBackground = true;

		Thread trainingThread = new Thread(StartTraining);
		trainingThread.Start();
	}

	private void StartTraining()
	{
		lock(SyncObject)
		{
			Debug.Log("Training Started, please wait!");

			stopWords = OpenFile(PathStopWordsDataTrainingFile);
			contractionWords = OpenFile(PathContractionWordsDataTrainingFile);
			
			ProcessDataTraining();
			
			Debug.Log("Training Finished");

			if (WriteWeightedFile)
			{
				sentimentWeightCommonWords.SaveTrainedData();
			}
		}
	}

	private void ProcessDataTraining()
	{
		BuildFileOfData(PathToPositiveDataTrainingFolder,true);
		BuildFileOfData(PathToNegativeDataTrainingFolder,false);

		SaveTheTrainedData(PathToSaveTrainedData + NameOfTrainedData);
	}

	private void BuildFileOfData(string path, bool isPositive)
	{
		FileInfo[] filesInPath = GetFilesInDirectory(path);

		totalReview += filesInPath.Length;

		for (int i = 0; i < filesInPath.Length; i++)
		{
			string TextOfFile = GetTextFile(filesInPath[i].FullName);		
			Words.AddRange( SplitsTheParagraphInWords(TextOfFile, isPositive) );
		}
	}

	private void SaveTheTrainedData(string path)
	{
		try
		{
			string createText = System.String.Empty;
			
			for (int i = 0; i < Words.Count; i++)
			{
				// Weighting Logic
				// Optimize at will

				float positiveWeight = (float)PositiveTimes[i] / (float)( PositiveTimes[i] + NegativeTimes[i] );
				float negativeWeight = (float)NegativeTimes[i] / (float)( PositiveTimes[i] + NegativeTimes[i] );

				float value = ( positiveWeight > negativeWeight ) ? positiveWeight : negativeWeight;

				createText += Words[i] + ":" + value.ToString() + ";";
			}

			File.WriteAllText(path, createText);
		}
		catch
		{
			Debug.Log("Error WritingFile \"SaveTheTrainedData\" Method");
		}
	}
	
	private List<string> SplitsTheParagraphInWords(string text, bool isPositive)
	{
		List<string> WordsList = new List<string>();
		Stemmer stemmer = new Stemmer();

		text = text.Trim().ToLower();

		if (text[text.Length - 1 ] != '.') text += ".";

		text = text.Replace(',', ' ');
		text = text.Replace(';', ' ');
		text = text.Replace(':', ' ');
		text = text.Replace('\"', ' ');
		text = text.Replace('\'', ' ');
		text = text.Replace('!', ' ');
		text = text.Replace('?', ' ');
		text = text.Replace('(', ' ');
		text = text.Replace(')', ' ');
		text = text.Replace(']', ' ');
		text = text.Replace('[', ' ');
		text = text.Replace('<', ' ');
		text = text.Replace('>', ' ');
		text = text.Replace('+', ' ');
		text = text.Replace('*', ' ');
		text = text.Replace('%', ' ');
		text = text.Replace('&', ' ');
		text = text.Replace('$', ' ');
		text = text.Replace('=', ' ');
		text = text.Replace('^', ' ');
		text = text.Replace('-', ' ');
		text = text.Replace('/', ' ');
		text = text.Replace('\\', ' ');
		text = text.Replace('\'', ' ');
		text = text.Replace('@', ' ');
		text = text.Replace('_', ' ');


		string[] Sentences = text.Split('.');

		for (int i = 0; i < Sentences.Length; i++)
		{
			if (!string.IsNullOrEmpty(Sentences[i]))
			{
				string[] Words = Sentences[i].Trim().Split(' ');

				for (int j = 0; j < Words.Length; j++)
				{
					if ( !IsWordContractionOrStop(Words[j]) && !string.IsNullOrEmpty(Words[j]) && Words[j].Length > 3 && Words[j].Length < 13)
					{
						Words[j] = StemTheWord(Words[j], stemmer);

						if (! IsStemmedWordPartOfList(Words[j], isPositive) )
						{
							if ( !WordsList.Contains(Words[j]) )
							{
								if (Words[j].Length > 2)
								{
									WordsList.Add(Words[j]);

									if (isPositive)
									{
										PositiveTimes.Add(1);
										NegativeTimes.Add(0);
									}
									else
									{
										PositiveTimes.Add(0);
										NegativeTimes.Add(1);
									}
								}
							}
							else
							{
								try
								{
									int indexOfWord = WordsList.IndexOf(Words[j]);

									if (isPositive)
									{
										PositiveTimes[PositiveTimes.Count - WordsList.Count + indexOfWord] ++; 
									}
									else
									{
										NegativeTimes[NegativeTimes.Count - WordsList.Count + indexOfWord] ++; 
									}
								}
								catch
								{
									Debug.Log("Error Index \"SplitsTheParagraphInWords\" Method");
								}
							}
						}
					}
				}
			}
		}

		return WordsList;
	}

	private bool IsWordContractionOrStop(string Word)
	{
		if (stopWords.Contains(Word) || contractionWords.Contains(Word) ) return true;

		return false;
	}

	private bool IsStemmedWordPartOfList(string stemmedWord, bool isPositive)
	{
		if (Words.Contains(stemmedWord))
		{
			int indexOfWord = Words.IndexOf(stemmedWord);

			try
			{
				if (isPositive)
				{				
					PositiveTimes[indexOfWord] ++; 
				}
				else
				{
					NegativeTimes[indexOfWord] ++; 
				}
				
				return true;
			}
			catch
			{
				Debug.Log("Error Index \"IsStemmedWordPartOfList\" Method");
			}
		}
		
		return false;
	}

	private string StemTheWord(string word, Stemmer stemmer)
	{
		stemmer.SetCharArray(word.ToCharArray(), word.Length);
		stemmer.stem();

		return stemmer.Tostring();
	}

	private FileInfo[] GetFilesInDirectory(string path)
	{
		DirectoryInfo dir = new DirectoryInfo(path);
		return dir.GetFiles();
	}

	private string GetTextFile(string filePath)
	{
		StreamReader theReader = new StreamReader(filePath, Encoding.Default);		
		try
		{
			using (theReader)
			{
				string textOfFile = theReader.ReadToEnd(); 
				theReader.Close();
				return textOfFile;
			}	
		}	
		catch
		{
			return System.String.Empty;
		}
	}

	private string[] OpenFile(string filePath)
	{
		StreamReader theReader = new StreamReader(filePath, Encoding.Default);		
		try
		{
			using (theReader)
			{
				string[] textOfFile = theReader.ReadToEnd().Trim().Split(','); 
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