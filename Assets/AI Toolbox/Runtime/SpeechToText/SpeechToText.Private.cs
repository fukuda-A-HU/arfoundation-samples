using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace AiToolbox {
public static partial class SpeechToText {
    private static readonly List<RequestRecord> RequestRecords = new();

    private static Action RequestBlocking(AudioClip audio, SpeechToTextParameters parameters,
                                          Action<SpeechToTextResponse> completeCallback,
                                          Action<long, string> failureCallback) {
        Debug.Assert(parameters != null, "Parameters cannot be null.");
        Debug.Assert(!string.IsNullOrEmpty(parameters!.apiKey), "API key cannot be null or empty.");
        Debug.Assert(audio != null, "Audio clip cannot be null.");

        if (parameters.throttle > 0) {
            var requestCount = RequestRecords.Count;
            if (requestCount >= parameters.throttle) {
                failureCallback?.Invoke((long)ErrorCodes.ThrottleExceeded,
                                        $"Too many requests. Maximum allowed: {parameters.throttle}.");
                return () => { };
            }
        }

        var requestRecord = new RequestRecord();
        var webRequest = GetWebRequest(audio, parameters, failureCallback, requestRecord);
        var cancelCallback = new Action(() => {
            try {
                webRequest?.Abort();
                webRequest?.Dispose();
                RequestRecords.Remove(requestRecord);
            }
            catch (Exception) {
                // If the request is aborted, accessing the error property will throw an exception.
            }
        });
        requestRecord.SetCancelCallback(cancelCallback);
        RequestRecords.Add(requestRecord);

        webRequest.SendWebRequest().completed += _ => {
            RequestRecords.Remove(requestRecord);
            Application.quitting -= cancelCallback;

            bool isErrorResponse;
            try {
                isErrorResponse = !string.IsNullOrEmpty(webRequest.error);
            }
            catch (Exception) {
                // If the request is aborted, accessing the error property will throw an exception.
                return;
            }

            if (isErrorResponse) {
                failureCallback?.Invoke(webRequest.responseCode, webRequest.error);
                return;
            }

            var response = JsonConvert.DeserializeObject<SpeechToTextResponse>(webRequest.downloadHandler.text);
            completeCallback?.Invoke(response);
            webRequest.Dispose();
        };

        Application.quitting += cancelCallback;
        return cancelCallback;
    }

    private static IEnumerator
        GetRemoteConfig(SpeechToTextParameters parameters, Action<long, string> failureCallback) {
        var apiKeySet = false;
        var task = RemoteKeyService.GetApiKey(parameters.apiKeyRemoteConfigKey, s => {
            parameters.apiKeyEncryption = ApiKeyEncryption.None;
            parameters.apiKey = s;
            apiKeySet = true;
        }, (errorCode, error) => {
            failureCallback?.Invoke(errorCode, error);
            apiKeySet = true;
        });

        yield return new WaitUntil(() => task.IsCompleted && apiKeySet);

        if (task.IsFaulted) {
            failureCallback?.Invoke((long)ErrorCodes.RemoteConfigConnectionFailure,
                                    "Failed to retrieve API key from remote config.");
        }
    }

    private static UnityWebRequest GetWebRequest(AudioClip audio, SpeechToTextParameters parameters,
                                                 Action<long, string> failureCallback, RequestRecord requestRecord) {
        var form = new List<IMultipartFormSection> {
            new MultipartFormFileSection("file", AudioUtility.GetWavData(audio), "audio.wav", "audio/wav"),
            new MultipartFormDataSection("model", parameters.GetModelString()),
            new MultipartFormDataSection("language", parameters.language),
            new MultipartFormDataSection("temperature", parameters.temperature.ToString(CultureInfo.InvariantCulture))
        };

        if (!string.IsNullOrEmpty(parameters.prompt)) {
            form.Add(new MultipartFormDataSection("prompt", parameters.prompt));
        }

        const string url = "https://api.openai.com/v1/audio/transcriptions";
        var request = UnityWebRequest.Post(url, form);
        request.timeout = parameters.timeout;

        try {
            var apiKey = parameters.apiKey;
            var isEncrypted = parameters.apiKeyEncryption == ApiKeyEncryption.LocallyEncrypted;
            if (isEncrypted) {
                apiKey = Key.B(apiKey, parameters.apiKeyEncryptionPassword);
            }

            request.SetRequestHeader("Authorization", "Bearer " + apiKey);
        }
        catch (Exception e) {
            failureCallback?.Invoke((long)ErrorCodes.Unknown, e.Message);
            RequestRecords.Remove(requestRecord);
        }

        return request;
    }
}
}