using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace AiToolbox {
public static partial class Moderation {
    internal struct ModerationRequest {
        public string input;
        public string model;
    }

    private static readonly List<RequestRecord> RequestRecords = new();

    private static Action QuickRequestBlocking(string text, ModerationParameters parameters,
                                               Action<ModerationResponse> completeCallback,
                                               Action<long, string> failureCallback) {
        Debug.Assert(parameters != null, "Parameters cannot be null.");
        Debug.Assert(!string.IsNullOrEmpty(parameters!.apiKey), "API key cannot be null or empty.");
        Debug.Assert(!string.IsNullOrEmpty(text), "Input text cannot be null or empty.");

        // Throttle.
        if (parameters.throttle > 0) {
            var requestCount = RequestRecords.Count;
            if (requestCount >= parameters.throttle) {
                failureCallback?.Invoke((long)ErrorCodes.ThrottleExceeded,
                                        $"Too many requests. Maximum allowed: {parameters.throttle}.");
                return () => { };
            }
        }

        var requestObject = new ModerationRequest {
            input = text,
            model = parameters.moderationModel switch {
                OpenAiModerationModel.Stable => "text-moderation-stable",
                OpenAiModerationModel.Latest => "text-moderation-latest",
                _ => "text-moderation-latest"
            },
        };

        var requestRecord = new RequestRecord();
        var requestJson = JsonUtility.ToJson(requestObject);
        var webRequest = GetWebRequest(requestJson, parameters, failureCallback, requestRecord);
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

            var response =
                Newtonsoft.Json.JsonConvert.DeserializeObject<ModerationResponse>(webRequest.downloadHandler.text);
            completeCallback?.Invoke(response);
            webRequest.Dispose();
        };

        Application.quitting += cancelCallback;
        return cancelCallback;
    }

    private static IEnumerator GetRemoteConfig(ModerationParameters parameters, Action<long, string> failureCallback) {
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

    private static UnityWebRequest GetWebRequest(string requestJson, ModerationParameters parameters,
                                                 Action<long, string> failureCallback, RequestRecord requestRecord) {
        var url = "https://api.openai.com/v1/moderations";
#if UNITY_2022_2_OR_NEWER
        var request = UnityWebRequest.Post(url, requestJson, "application/json");
#else
        var request = new UnityWebRequest(url, "POST");
        request.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(requestJson));
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
#endif

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
            // 0d344651-d8d3-46d2-b91c-031a0a12d4e8
            RequestRecords.Remove(requestRecord);
        }

        return request;
    }
}
}