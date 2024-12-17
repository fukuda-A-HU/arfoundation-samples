using System;
using System.IO;
using UnityEngine;

namespace AiToolbox {
public static class AudioUtility {
    /// <summary>
    /// Trim the silence from the start and end of the input audio clip.
    /// </summary>
    /// <param name="clip">The input audio clip to trim.</param>
    /// <param name="min">The minimum amplitude to consider as silence.</param>
    /// <returns>The trimmed audio clip.</returns>
    public static AudioClip TrimSilence(AudioClip clip, float min) {
        var data = new float[clip.samples];
        clip.GetData(data, 0);

        int start;
        for (start = 0; start < data.Length; start++) {
            if (Mathf.Abs(data[start]) > min) {
                break;
            }
        }

        int end;
        for (end = data.Length - 1; end > 0; end--) {
            if (Mathf.Abs(data[end]) > min) {
                break;
            }
        }

        var trimmedData = new float[end - start];
        Array.Copy(data, start, trimmedData, 0, trimmedData.Length);

        var trimmedClip = AudioClip.Create(clip.name, trimmedData.Length, clip.channels, clip.frequency, false);
        trimmedClip.SetData(trimmedData, 0);
        return trimmedClip;
    }

    /// <summary>
    /// Convert the input audio clip to a WAV byte array.
    /// </summary>
    /// <param name="clip">The input audio clip to convert.</param>
    /// <returns>The WAV byte array.</returns>
    public static byte[] GetWavData(AudioClip clip) {
        using var memoryStream = CreateEmpty();
        ConvertAndWrite(memoryStream, clip);
        WriteHeader(memoryStream, clip);
        return memoryStream.GetBuffer();
    }

    #region Private

    private const int HeaderSize = 44;

    private static MemoryStream CreateEmpty() {
        var memoryStream = new MemoryStream();
        const byte emptyByte = new();

        for (int i = 0; i < HeaderSize; i++) {
            memoryStream.WriteByte(emptyByte);
        }

        return memoryStream;
    }

    private static void ConvertAndWrite(Stream memoryStream, AudioClip clip) {
        var samples = new float[clip.samples];

        clip.GetData(samples, 0);

        short[] intData = new short[samples.Length];
        byte[] bytesData = new byte[samples.Length * 2];
        const int rescaleFactor = 32767;

        for (int i = 0; i < samples.Length; i++) {
            intData[i] = (short)(samples[i] * rescaleFactor);
            byte[] byteArr = BitConverter.GetBytes(intData[i]);
            byteArr.CopyTo(bytesData, i * 2);
        }

        memoryStream.Write(bytesData, 0, bytesData.Length);
    }

    private static void WriteHeader(Stream memoryStream, AudioClip clip) {
        var hz = clip.frequency;
        var channels = clip.channels;
        var samples = clip.samples;

        memoryStream.Seek(0, SeekOrigin.Begin);

        byte[] riff = System.Text.Encoding.UTF8.GetBytes("RIFF");
        memoryStream.Write(riff, 0, 4);

        byte[] chunkSize = BitConverter.GetBytes(memoryStream.Length - 8);
        memoryStream.Write(chunkSize, 0, 4);

        byte[] wave = System.Text.Encoding.UTF8.GetBytes("WAVE");
        memoryStream.Write(wave, 0, 4);

        byte[] fmt = System.Text.Encoding.UTF8.GetBytes("fmt ");
        memoryStream.Write(fmt, 0, 4);

        byte[] subChunk1 = BitConverter.GetBytes(16);
        memoryStream.Write(subChunk1, 0, 4);

        const ushort one = 1;

        byte[] audioFormat = BitConverter.GetBytes(one);
        memoryStream.Write(audioFormat, 0, 2);

        byte[] numChannels = BitConverter.GetBytes(channels);
        memoryStream.Write(numChannels, 0, 2);

        byte[] sampleRate = BitConverter.GetBytes(hz);
        memoryStream.Write(sampleRate, 0, 4);

        byte[] byteRate = BitConverter.GetBytes(hz * channels * 2);
        memoryStream.Write(byteRate, 0, 4);

        ushort blockAlign = (ushort)(channels * 2);
        memoryStream.Write(BitConverter.GetBytes(blockAlign), 0, 2);

        const ushort bps = 16;
        byte[] bitsPerSample = BitConverter.GetBytes(bps);
        memoryStream.Write(bitsPerSample, 0, 2);

        byte[] dataString = System.Text.Encoding.UTF8.GetBytes("data");
        memoryStream.Write(dataString, 0, 4);

        byte[] subChunk2 = BitConverter.GetBytes(samples * channels * 2);
        memoryStream.Write(subChunk2, 0, 4);
    }

    #endregion
}
}