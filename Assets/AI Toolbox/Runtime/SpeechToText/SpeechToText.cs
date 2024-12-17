using System;
using System.Collections;
using UnityEngine;

namespace AiToolbox {
public static partial class SpeechToText {
    public static Action Request(AudioClip audio, SpeechToTextParameters parameters,
                                 Action<SpeechToTextResponse> completeCallback, Action<long, string> failureCallback) {
        if (parameters.apiKeyEncryption != ApiKeyEncryption.RemoteConfig) {
            return RequestBlocking(audio, parameters, completeCallback, failureCallback);
        }

        var enumerator = RequestCoroutine(audio, parameters, completeCallback, failureCallback);
        SpeechToTextContainer.instance.StartCoroutine(enumerator);

        return CancelCallback;

        static IEnumerator RequestCoroutine(AudioClip audio, SpeechToTextParameters parameters,
                                            Action<SpeechToTextResponse> completeCallback,
                                            Action<long, string> failureCallback) {
            if (parameters.apiKeyEncryption == ApiKeyEncryption.RemoteConfig) {
                yield return GetRemoteConfig(parameters, failureCallback);
            }

            RequestBlocking(audio, parameters, completeCallback, failureCallback);
        }

        void CancelCallback() {
            SpeechToTextContainer.instance.StopCoroutine(enumerator);
        }
    }

    /// <summary>
    /// Cancel all pending requests.
    /// </summary>
    public static void CancelAllRequests() {
        while (RequestRecords.Count > 0) {
            RequestRecords[0].Cancel();
        }

        RequestRecords.Clear();
    }
}
}