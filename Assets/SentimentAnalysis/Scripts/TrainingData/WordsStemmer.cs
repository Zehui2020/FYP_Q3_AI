using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Text;
using System.Linq;
using UnitySentiment;

public class WordsStemmer : MonoBehaviour 
{
	public string UrlToDocument;
	public string UrlToDocumentSave;

	Stemmer stem = new Stemmer();

	private List <string> names = new List<string>();

	void Start () 
	{
		string[] data = OpenFile(UrlToDocument);

		for (int i = 0; i < data.Length; i++)
		{
			stem.SetCharArray(data[i].Trim().ToCharArray(), data[i].Length);
			stem.stem();

			data[i] = stem.Tostring();
			names.Add(data[i]);
		}


		names = names.Distinct().ToList();

		string createText = System.String.Empty;

		for (int i = 0; i< names.Count; i++)
		{	
			createText += names[i] + ",";	
		}

		File.AppendAllText(UrlToDocumentSave, createText);
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
