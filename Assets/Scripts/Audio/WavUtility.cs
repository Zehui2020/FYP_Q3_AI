﻿using UnityEngine;
using System.Text;
using System.IO;
using System;
public class WavUtility
{
	const int BlockSize_16Bit = 2;

	public static AudioClip ToAudioClip(string filename)
	{
		byte[] fileBytes = File.ReadAllBytes (Application.persistentDataPath + "/" + filename);
		return ToAudioClip (fileBytes, 0);
	}

	public static AudioClip ToAudioClip (byte[] fileBytes, int offsetSamples = 0, string name = "wav")
	{
		int subchunk1 = BitConverter.ToInt32 (fileBytes, 16);
		UInt16 audioFormat = BitConverter.ToUInt16 (fileBytes, 20);

		string formatCode = FormatCode (audioFormat);
		Debug.AssertFormat (audioFormat == 1 || audioFormat == 65534, "Detected format code '{0}' {1}, but only PCM and WaveFormatExtensable uncompressed formats are currently supported.", audioFormat, formatCode);

		UInt16 channels = BitConverter.ToUInt16 (fileBytes, 22);
		int sampleRate = BitConverter.ToInt32 (fileBytes, 24);
		UInt16 bitDepth = BitConverter.ToUInt16 (fileBytes, 34);

		int headerOffset = 16 + 4 + subchunk1 + 4;
		int subchunk2 = BitConverter.ToInt32 (fileBytes, headerOffset);

		float[] data;
		switch (bitDepth) {
		case 8:
			data = Convert8BitByteArrayToAudioClipData (fileBytes, headerOffset, subchunk2);
			break;
		case 16:
			data = Convert16BitByteArrayToAudioClipData (fileBytes, headerOffset, subchunk2);
			break;
		case 24:
			data = Convert24BitByteArrayToAudioClipData (fileBytes, headerOffset, subchunk2);
			break;
		case 32:
			data = Convert32BitByteArrayToAudioClipData (fileBytes, headerOffset, subchunk2);
			break;
		default:
			throw new Exception (bitDepth + " bit depth is not supported.");
		}

		AudioClip audioClip = AudioClip.Create (name, data.Length, (int)channels, sampleRate, false);
		audioClip.SetData (data, 0);
		return audioClip;
	}

	#region wav file bytes to Unity AudioClip conversion methods

	private static float[] Convert8BitByteArrayToAudioClipData (byte[] source, int headerOffset, int dataSize)
	{
		int wavSize = BitConverter.ToInt32 (source, headerOffset);
		headerOffset += sizeof(int);
		Debug.AssertFormat (wavSize > 0 && wavSize == dataSize, "Failed to get valid 8-bit wav size: {0} from data bytes: {1} at offset: {2}", wavSize, dataSize, headerOffset);

		float[] data = new float[wavSize];

		sbyte maxValue = sbyte.MaxValue;

		int i = 0;
		while (i < wavSize) {
			data [i] = (float)source [i] / maxValue;
			++i;
		}

		return data;
	}

	private static float[] Convert16BitByteArrayToAudioClipData (byte[] source, int headerOffset, int dataSize)
	{
		int wavSize = BitConverter.ToInt32 (source, headerOffset);
		headerOffset += sizeof(int);
		Debug.AssertFormat (wavSize > 0 && wavSize == dataSize, "Failed to get valid 16-bit wav size: {0} from data bytes: {1} at offset: {2}", wavSize, dataSize, headerOffset);

		int x = sizeof(Int16); // block size = 2
		int convertedSize = wavSize / x;

		float[] data = new float[convertedSize];

		Int16 maxValue = Int16.MaxValue;

		int offset = 0;
		int i = 0;
		while (i < convertedSize) {
			offset = i * x + headerOffset;
			data [i] = (float)BitConverter.ToInt16 (source, offset) / maxValue;
			++i;
		}

		Debug.AssertFormat (data.Length == convertedSize, "AudioClip .wav data is wrong size: {0} == {1}", data.Length, convertedSize);

		return data;
	}

	private static float[] Convert24BitByteArrayToAudioClipData (byte[] source, int headerOffset, int dataSize)
	{
		int wavSize = BitConverter.ToInt32 (source, headerOffset);
		headerOffset += sizeof(int);
		Debug.AssertFormat (wavSize > 0 && wavSize == dataSize, "Failed to get valid 24-bit wav size: {0} from data bytes: {1} at offset: {2}", wavSize, dataSize, headerOffset);

		int x = 3; // block size = 3
		int convertedSize = wavSize / x;

		int maxValue = Int32.MaxValue;

		float[] data = new float[convertedSize];

		byte[] block = new byte[sizeof(int)]; // using a 4 byte block for copying 3 bytes, then copy bytes with 1 offset

		int offset = 0;
		int i = 0;
		while (i < convertedSize) {
			offset = i * x + headerOffset;
			Buffer.BlockCopy (source, offset, block, 1, x);
			data [i] = (float)BitConverter.ToInt32 (block, 0) / maxValue;
			++i;
		}

		Debug.AssertFormat (data.Length == convertedSize, "AudioClip .wav data is wrong size: {0} == {1}", data.Length, convertedSize);

		return data;
	}

	private static float[] Convert32BitByteArrayToAudioClipData (byte[] source, int headerOffset, int dataSize)
	{
		int wavSize = BitConverter.ToInt32 (source, headerOffset);
		headerOffset += sizeof(int);
		Debug.AssertFormat (wavSize > 0 && wavSize == dataSize, "Failed to get valid 32-bit wav size: {0} from data bytes: {1} at offset: {2}", wavSize, dataSize, headerOffset);

		int x = sizeof(float); //  block size = 4
		int convertedSize = wavSize / x;

		Int32 maxValue = Int32.MaxValue;

		float[] data = new float[convertedSize];

		int offset = 0;
		int i = 0;
		while (i < convertedSize) {
			offset = i * x + headerOffset;
			data [i] = (float)BitConverter.ToInt32 (source, offset) / maxValue;
			++i;
		}

		Debug.AssertFormat (data.Length == convertedSize, "AudioClip .wav data is wrong size: {0} == {1}", data.Length, convertedSize);

		return data;
	}

	#endregion

	public static byte[] FromAudioClip (AudioClip audioClip)
	{
		string file;
		return FromAudioClip (audioClip, out file, false);
	}

	public static byte[] FromAudioClip (AudioClip audioClip, out string filepath, bool saveAsFile = true, string dirname = "recordings")
	{
		MemoryStream stream = new MemoryStream ();

		const int headerSize = 44;

		// get bit depth
		UInt16 bitDepth = 16; //BitDepth (audioClip);

		// NB: Only supports 16 bit
		//Debug.AssertFormat (bitDepth == 16, "Only converting 16 bit is currently supported. The audio clip data is {0} bit.", bitDepth);

		// total file size = 44 bytes for header format and audioClip.samples * factor due to float to Int16 / sbyte conversion
		int fileSize = audioClip.samples * BlockSize_16Bit + headerSize; // BlockSize (bitDepth)

		// chunk descriptor (riff)
		WriteFileHeader (ref stream, fileSize);
		// file header (fmt)
		WriteFileFormat (ref stream, audioClip.channels, audioClip.frequency, bitDepth);
		// data chunks (data)
		WriteFileData (ref stream, audioClip, bitDepth);

		byte[] bytes = stream.ToArray ();

		// Validate total bytes
		Debug.AssertFormat (bytes.Length == fileSize, "Unexpected AudioClip to wav format byte count: {0} == {1}", bytes.Length, fileSize);

		// Save file to persistant storage location
		if (saveAsFile) {
			filepath = string.Format ("{0}/{1}/{2}.{3}", Application.persistentDataPath, dirname, DateTime.UtcNow.ToString ("yyMMdd-HHmmss-fff"), "wav");
			Directory.CreateDirectory (Path.GetDirectoryName (filepath));
			File.WriteAllBytes (filepath, bytes);
			//Debug.Log ("Auto-saved .wav file: " + filepath);
		} else {
			filepath = null;
		}

		stream.Dispose ();

		return bytes;
	}

	#region write .wav file functions

	private static int WriteFileHeader (ref MemoryStream stream, int fileSize)
	{
		int count = 0;
		int total = 12;

		byte[] riff = Encoding.ASCII.GetBytes ("RIFF");
		count += WriteBytesToMemoryStream (ref stream, riff, "ID");

		int chunkSize = fileSize - 8;
		count += WriteBytesToMemoryStream (ref stream, BitConverter.GetBytes (chunkSize), "CHUNK_SIZE");

		byte[] wave = Encoding.ASCII.GetBytes ("WAVE");
		count += WriteBytesToMemoryStream (ref stream, wave, "FORMAT");

		Debug.AssertFormat (count == total, "Unexpected wav descriptor byte count: {0} == {1}", count, total);

		return count;
	}

	private static int WriteFileFormat (ref MemoryStream stream, int channels, int sampleRate, UInt16 bitDepth)
	{
		int count = 0;
		int total = 24;

		byte[] id = Encoding.ASCII.GetBytes ("fmt ");
		count += WriteBytesToMemoryStream (ref stream, id, "FMT_ID");

		int subchunk1Size = 16;
		count += WriteBytesToMemoryStream (ref stream, BitConverter.GetBytes (subchunk1Size), "SUBCHUNK_SIZE");

		UInt16 audioFormat = 1;
		count += WriteBytesToMemoryStream (ref stream, BitConverter.GetBytes (audioFormat), "AUDIO_FORMAT");

		UInt16 numChannels = Convert.ToUInt16 (channels);
		count += WriteBytesToMemoryStream (ref stream, BitConverter.GetBytes (numChannels), "CHANNELS");

		count += WriteBytesToMemoryStream (ref stream, BitConverter.GetBytes (sampleRate), "SAMPLE_RATE");

		int byteRate = sampleRate * channels * BytesPerSample (bitDepth);
		count += WriteBytesToMemoryStream (ref stream, BitConverter.GetBytes (byteRate), "BYTE_RATE");

		UInt16 blockAlign = Convert.ToUInt16 (channels * BytesPerSample (bitDepth));
		count += WriteBytesToMemoryStream (ref stream, BitConverter.GetBytes (blockAlign), "BLOCK_ALIGN");

		count += WriteBytesToMemoryStream (ref stream, BitConverter.GetBytes (bitDepth), "BITS_PER_SAMPLE");

		Debug.AssertFormat (count == total, "Unexpected wav fmt byte count: {0} == {1}", count, total);

		return count;
	}

	private static int WriteFileData (ref MemoryStream stream, AudioClip audioClip, UInt16 bitDepth)
	{
		int count = 0;
		int total = 8;

		float[] data = new float[audioClip.samples * audioClip.channels];
		audioClip.GetData (data, 0);

		byte[] bytes = ConvertAudioClipDataToInt16ByteArray (data);

		byte[] id = Encoding.ASCII.GetBytes ("data");
		count += WriteBytesToMemoryStream (ref stream, id, "DATA_ID");

		int subchunk2Size = Convert.ToInt32 (audioClip.samples * BlockSize_16Bit); // BlockSize (bitDepth)
		count += WriteBytesToMemoryStream (ref stream, BitConverter.GetBytes (subchunk2Size), "SAMPLES");

		Debug.AssertFormat (count == total, "Unexpected wav data id byte count: {0} == {1}", count, total);

		count += WriteBytesToMemoryStream (ref stream, bytes, "DATA");

		Debug.AssertFormat (bytes.Length == subchunk2Size, "Unexpected AudioClip to wav subchunk2 size: {0} == {1}", bytes.Length, subchunk2Size);

		return count;
	}

	private static byte[] ConvertAudioClipDataToInt16ByteArray (float[] data)
	{
		MemoryStream dataStream = new MemoryStream ();

		int x = sizeof(Int16);

		Int16 maxValue = Int16.MaxValue;

		int i = 0;
		while (i < data.Length) {
			dataStream.Write (BitConverter.GetBytes (Convert.ToInt16 (data [i] * maxValue)), 0, x);
			++i;
		}
		byte[] bytes = dataStream.ToArray ();

		Debug.AssertFormat (data.Length * x == bytes.Length, "Unexpected float[] to Int16 to byte[] size: {0} == {1}", data.Length * x, bytes.Length);

		dataStream.Dispose ();

		return bytes;
	}

	private static int WriteBytesToMemoryStream (ref MemoryStream stream, byte[] bytes, string tag = "")
	{
		int count = bytes.Length;
		stream.Write (bytes, 0, count);
		return count;
	}

	#endregion

	public static UInt16 BitDepth (AudioClip audioClip)
	{
		UInt16 bitDepth = Convert.ToUInt16 (audioClip.samples * audioClip.channels * audioClip.length / audioClip.frequency);
		Debug.AssertFormat (bitDepth == 8 || bitDepth == 16 || bitDepth == 32, "Unexpected AudioClip bit depth: {0}. Expected 8 or 16 or 32 bit.", bitDepth);
		return bitDepth;
	}

	private static int BytesPerSample (UInt16 bitDepth)
	{
		return bitDepth / 8;
	}

	private static int BlockSize (UInt16 bitDepth)
	{
		switch (bitDepth) {
		case 32:
			return sizeof(Int32);
		case 16:
			return sizeof(Int16); 
		case 8:
			return sizeof(sbyte);
		default:
			throw new Exception (bitDepth + " bit depth is not supported.");
		}
	}

	private static string FormatCode (UInt16 code)
	{
		switch (code) {
		case 1:
			return "PCM";
		case 2:
			return "ADPCM";
		case 3:
			return "IEEE";
		case 7:
			return "μ-law";
		case 65534:
			return "WaveFormatExtensable";
		default:
			Debug.LogWarning ("Unknown wav code format:" + code);
			return "";
		}
	}

}