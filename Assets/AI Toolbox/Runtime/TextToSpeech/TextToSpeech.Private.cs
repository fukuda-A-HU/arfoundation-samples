using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace AiToolbox {
public static partial class TextToSpeech {
    private static readonly List<RequestRecord> RequestRecords = new();

    private static Action RequestBlocking(string input, TextToSpeechParameters parameters,
                                          Action<AudioClip> completeCallback, Action<long, string> failureCallback) {
        Debug.Assert(parameters != null, "Parameters cannot be null.");
        Debug.Assert(!string.IsNullOrEmpty(parameters!.apiKey), "API key cannot be null or empty.");
        Debug.Assert(!string.IsNullOrEmpty(input), "Input text cannot be null or empty.");

        if (parameters.throttle > 0) {
            var requestCount = RequestRecords.Count;
            if (requestCount >= parameters.throttle) {
                failureCallback?.Invoke((long)ErrorCodes.ThrottleExceeded,
                                        $"Too many requests. Maximum allowed: {parameters.throttle}.");
                return () => { };
            }
        }

        var requestRecord = new RequestRecord();
        var webRequest = GetWebRequest(input, parameters, failureCallback, requestRecord);
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

            var audioClip = DownloadHandlerAudioClip.GetContent(webRequest);
            audioClip.name = "TextToSpeech";
            completeCallback?.Invoke(audioClip);
            webRequest.Dispose();
        };

        Application.quitting += cancelCallback;
        return cancelCallback;
    }

    private static IEnumerator
        GetRemoteConfig(TextToSpeechParameters parameters, Action<long, string> failureCallback) {
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

    private static UnityWebRequest GetWebRequest(string input, TextToSpeechParameters parameters,
                                                 Action<long, string> failureCallback, RequestRecord requestRecord) {
        const string url = "https://api.openai.com/v1/audio/speech";
        var requestObject = new TextToSpeechRequest {
            input = input,
            model = parameters.GetModelString(),
            voice = parameters.voice.ToString().ToLower(),
            speed = parameters.speed
        };
        var json = JsonUtility.ToJson(requestObject);
        var request = new UnityWebRequest(url, "POST");
        var uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(json));
        request.uploadHandler = uploadHandler;
        request.downloadHandler = new DownloadHandlerAudioClip((string)null, AudioType.MPEG);
        request.SetRequestHeader("Content-Type", "application/json");
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