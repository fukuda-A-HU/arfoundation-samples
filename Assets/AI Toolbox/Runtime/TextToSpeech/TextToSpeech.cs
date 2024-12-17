using System;
using System.Collections;
using UnityEngine;

namespace AiToolbox {
public static partial class TextToSpeech {
    /// <summary>
    /// Request text-to-speech conversion of the input text.
    /// </summary>
    /// <param name="input">The input text to convert to speech.</param>
    /// <param name="parameters">The parameters for the text-to-speech request.</param>
    /// <param name="completeCallback">The callback to be called when the request is completed.</param>
    /// <param name="failureCallback">The callback to be called when the request fails.</param>
    /// <returns></returns>
    public static Action Request(string input, TextToSpeechParameters parameters, Action<AudioClip> completeCallback,
                                 Action<long, string> failureCallback) {
        if (parameters.apiKeyEncryption != ApiKeyEncryption.RemoteConfig) {
            return RequestBlocking(input, parameters, completeCallback, failureCallback);
        }

        var enumerator = RequestCoroutine(input, parameters, completeCallback, failureCallback);
        TextToSpeechContainer.instance.StartCoroutine(enumerator);

        return CancelCallback;

        static IEnumerator RequestCoroutine(string input, TextToSpeechParameters parameters,
                                            Action<AudioClip> completeCallback, Action<long, string> failureCallback) {
            if (parameters.apiKeyEncryption == ApiKeyEncryption.RemoteConfig) {
                yield return GetRemoteConfig(parameters, failureCallback);
            }

            RequestBlocking(input, parameters, completeCallback, failureCallback);
        }

        void CancelCallback() {
            TextToSpeechContainer.instance.StopCoroutine(enumerator);
        }
    }

    /// <summary>
    /// Cancel all pending text-to-speech requests.
    /// </summary>
    public static void CancelAllRequests() {
        while (RequestRecords.Count > 0) {
            RequestRecords[0].Cancel();
        }

        RequestRecords.Clear();
    }
}
}