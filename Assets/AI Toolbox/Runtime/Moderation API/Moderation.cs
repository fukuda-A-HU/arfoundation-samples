using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace AiToolbox {
public static partial class Moderation {
    /// <summary>
    /// Request moderation of the input text.
    /// </summary>
    /// <param name="text">The input text to check for toxicity.</param>
    /// <param name="parameters">The parameters for the moderation request.</param>
    /// <param name="completeCallback">The callback to be called when the request is completed.</param>
    /// <param name="failureCallback">The callback to be called when the request fails.</param>
    /// <returns></returns>
    public static Action Request(string text, ModerationParameters parameters,
                                 Action<ModerationResponse> completeCallback, Action<long, string> failureCallback) {
        if (parameters.apiKeyEncryption != ApiKeyEncryption.RemoteConfig) {
            return QuickRequestBlocking(text, parameters, completeCallback, failureCallback);
        }

        var enumerator = QuickRequestCoroutine(text, parameters, completeCallback, failureCallback);
        ModerationContainer.instance.StartCoroutine(enumerator);

        return CancelCallback;

        static IEnumerator QuickRequestCoroutine(string text, ModerationParameters parameters,
                                                 Action<ModerationResponse> completeCallback,
                                                 Action<long, string> failureCallback) {
            if (parameters.apiKeyEncryption == ApiKeyEncryption.RemoteConfig) {
                yield return GetRemoteConfig(parameters, failureCallback);
            }

            QuickRequestBlocking(text, parameters, completeCallback, failureCallback);
        }

        void CancelCallback() {
            ModerationContainer.instance.StopCoroutine(enumerator);
        }
    }

    /// <summary>
    /// Cancel all pending moderation requests.
    /// </summary>
    public static void CancelAllRequests() {
        while (RequestRecords.Count > 0) {
            RequestRecords[0].Cancel();
        }

        RequestRecords.Clear();
    }

    // https://platform.openai.com/docs/api-reference/moderations#moderations-create-input
}
}