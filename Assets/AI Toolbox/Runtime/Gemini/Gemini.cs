using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

// ReSharper disable InconsistentNaming
namespace AiToolbox {
public static class Gemini {
    private static readonly List<RequestRecord> _requestRecords = new List<RequestRecord>();

    /// <summary>
    /// Send a request to Gemini.
    /// </summary>
    /// <param name="prompt">The text of the request, e.g. "Generate a character description".</param>
    /// <param name="parameters">Settings of the request.</param>
    /// <param name="completeCallback">The function to be called on successful completion. Gemini response is provided
    /// as a parameter.</param>
    /// <param name="failureCallback">The function to be called on failure. Error code and message are provided as
    /// parameters.</param>
    /// <returns>A function that can be called to cancel the request.</returns>
    // ReSharper disable once MemberCanBePrivate.Global
    public static Action Request(string prompt, GeminiParameters parameters, Action<string> completeCallback,
                                 Action<long, string> failureCallback) {
        return Request(new List<Message> { new Message { role = Role.User, text = prompt } }, parameters,
                       completeCallback, failureCallback);
    }

    /// <summary>
    /// Send a request to Gemini.
    /// </summary>
    /// <param name="messages">Sequence of messages to send to Gemini. The order of messages should be the same as the
    /// chronological order of messages in the conversation, i.e. the first message should be the oldest one. The roles
    /// of the messages should switch between User and AI.</param> 
    /// <param name="parameters">Settings of the request.</param>
    /// <param name="completeCallback">The function to be called on successful completion. Gemini response is provided
    /// as a parameter.</param>
    /// <param name="failureCallback">The function to be called on failure. Error code and message are provided as
    /// parameters.</param>
    /// <returns>A function that can be called to cancel the request.</returns>
    // ReSharper disable once MemberCanBePrivate.Global
    public static Action Request(IEnumerable<Message> messages, GeminiParameters parameters,
                                 Action<string> completeCallback, Action<long, string> failureCallback) {
        Debug.Assert(parameters != null, "Parameters cannot be null.");
        Debug.Assert(!string.IsNullOrEmpty(parameters!.apiKey), "API key cannot be null or empty.");
        Debug.Assert(messages != null, "Messages cannot be null.");

        return QuickRequest(messages, parameters, completeCallback, failureCallback);
    }

    // ReSharper disable once MemberCanBePrivate.Global
    /// <summary>
    /// Cancel all pending requests.
    /// </summary>
    public static void CancelAllRequests() {
        while (_requestRecords.Count > 0) {
            _requestRecords[0].Cancel();
        }

        _requestRecords.Clear();
    }

    private static Action QuickRequest(IEnumerable<Message> messages, GeminiParameters parameters,
                                       Action<string> completeCallback, Action<long, string> failureCallback) {
        if (parameters.apiKeyEncryption != ApiKeyEncryption.RemoteConfig) {
            return QuickRequestBlocking(messages, parameters, completeCallback, failureCallback);
        }

        var enumerator = QuickRequestCoroutine(messages, parameters, completeCallback, failureCallback);
        GeminiContainer.Instance.StartCoroutine(enumerator);

        void CancelCallback() {
            GeminiContainer.Instance.StopCoroutine(enumerator);
        }

        return CancelCallback;
    }

    private static IEnumerator QuickRequestCoroutine(IEnumerable<Message> messages, GeminiParameters parameters,
                                                     Action<string> completeCallback,
                                                     Action<long, string> failureCallback) {
        if (parameters.apiKeyEncryption == ApiKeyEncryption.RemoteConfig) {
            yield return GetRemoteConfig(parameters, failureCallback);
        }

        QuickRequestBlocking(messages, parameters, completeCallback, failureCallback);
    }

    private static Action QuickRequestBlocking(IEnumerable<Message> messages, GeminiParameters parameters,
                                               Action<string> completeCallback, Action<long, string> failureCallback) {
        Debug.Assert(parameters != null, "Parameters cannot be null.");
        Debug.Assert(!string.IsNullOrEmpty(parameters!.apiKey), "API key cannot be null or empty.");
        Debug.Assert(messages != null, "Messages cannot be null.");

        // Throttle.
        if (parameters.throttle > 0) {
            var requestCount = _requestRecords.Count;
            if (requestCount >= parameters.throttle) {
                failureCallback?.Invoke((long)ErrorCodes.ThrottleExceeded,
                                        $"Too many requests. Maximum allowed: {parameters.throttle}.");
                return () => { };
            }
        }

        var messageList = messages.ToList();

        // Prepend context to messages.
        if (!string.IsNullOrEmpty(parameters.role)) {
            messageList.Insert(0, new Message { role = Role.User, text = $"You are a {parameters.role}." });
            messageList.Insert(1, new Message { role = Role.AI, text = "OK, ask me anything." });
        }

        // Convert messages to request format.
        var contents = new RequestContent[messageList.Count];
        for (var i = 0; i < messageList.Count; i++) {
            var message = messageList.ElementAt(i);
            contents[i] = new RequestContent {
                role = message.role == Role.User ? "user" : "model",
                parts = new[] { new RequestPart { text = message.text } }
            };
        }

        var requestObject = new RequestMessage {
            contents = contents,
            generationConfig = new GenerationConfig { temperature = parameters.temperature },
            safetySettings = new[] {
                new SafetySetting { category = "HARM_CATEGORY_DANGEROUS_CONTENT", threshold = "BLOCK_NONE" }
            }
        };

        var requestRecord = new RequestRecord();
        var requestJson = JsonUtility.ToJson(requestObject);
        var request = GetWebRequest(requestJson, parameters, failureCallback, requestRecord);
        var cancelCallback = new Action(() => {
            try {
                request?.Abort();
                request?.Dispose();
                _requestRecords.Remove(requestRecord);
            }
            catch (Exception) {
                // If the request is aborted, accessing the error property will throw an exception.
            }
        });
        requestRecord.SetCancelCallback(cancelCallback);
        _requestRecords.Add(requestRecord);

        request.SendWebRequest().completed += _ => {
            _requestRecords.Remove(requestRecord);
            Application.quitting -= cancelCallback;

            bool isErrorResponse;
            try {
                isErrorResponse = !string.IsNullOrEmpty(request.error);
            }
            catch (Exception) {
                // If the request is aborted, accessing the error property will throw an exception.
                return;
            }

            if (isErrorResponse) {
                failureCallback?.Invoke(request.responseCode, request.error);
                return;
            }

            ResponseMessage response;
            try {
                response = JsonUtility.FromJson<ResponseMessage>(request.downloadHandler.text);
            }
            catch (Exception e) {
                failureCallback?.Invoke((long)ErrorCodes.Unknown, e.Message);
                return;
            }

            if (response.candidates.Length == 0) {
                failureCallback?.Invoke((long)ErrorCodes.Unknown, "No response candidates returned from the server.");
                return;
            }

            string responseMessage;
            try {
                var c = response.candidates[0];
                responseMessage = c.content.parts.Aggregate("", (current, part) => current + part.text);
            }
            catch (Exception e) {
                failureCallback?.Invoke((long)ErrorCodes.Unknown, e.Message);
                return;
            }

            completeCallback?.Invoke(responseMessage);
            request.Dispose();
        };

        Application.quitting += cancelCallback;
        return cancelCallback;
    }

    private static UnityWebRequest GetWebRequest(string requestJson, GeminiParameters parameters,
                                                 Action<long, string> failureCallback, RequestRecord requestRecord) {
        var modelCode = parameters.model.GetModelCode();
        var url = $"https://generativelanguage.googleapis.com/v1beta/models/{modelCode}:generateContent";
        try {
            var apiKey = parameters.apiKey;
            var isEncrypted = parameters.apiKeyEncryption == ApiKeyEncryption.LocallyEncrypted;
            if (isEncrypted) {
                apiKey = Key.B(apiKey, parameters.apiKeyEncryptionPassword);
            }

            url += $"?key={apiKey}";
        }
        catch (Exception e) {
            failureCallback?.Invoke((long)ErrorCodes.Unknown, e.Message);
            _requestRecords.Remove(requestRecord);
        }

#if UNITY_2022_2_OR_NEWER
        var request = UnityWebRequest.Post(url, requestJson, "application/json");
#else
        var request = new UnityWebRequest(url, "POST");
        request.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(requestJson));
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
#endif

        request.timeout = parameters.timeout;

        return request;
    }

#pragma warning disable 0649
// ReSharper disable NotAccessedField.Local

    #region Request

    // https://cloud.google.com/vertex-ai/docs/generative-ai/model-reference/gemini#request_body

    [Serializable]
    internal struct RequestMessage {
        public RequestContent[] contents;
        public GenerationConfig generationConfig;
        public SafetySetting[] safetySettings;
        // public Tool[] tools;
    }

    [Serializable]
    internal struct RequestContent {
        public string role; // USER, MODEL.
        public RequestPart[] parts;
    }

    [Serializable]
    internal struct RequestPart {
        public string text;
        // Omitted: inlineData, fileData, videoMetadata.
    }

    [Serializable]
    internal struct GenerationConfig {
        public float temperature;
        // public float topP;
        // public int topK;
        // public int candidateCount;
        // public int maxOutputTokens;
        // public string[] stopSequences;
    }

    [Serializable]
    internal struct SafetySetting {
        // @formatter:off
        public string category;  // HARM_CATEGORY_SEXUALLY_EXPLICIT
                                 // HARM_CATEGORY_HATE_SPEECH
                                 // HARM_CATEGORY_HARASSMENT
                                 // HARM_CATEGORY_DANGEROUS_CONTENT
        public string threshold; // BLOCK_NONE
                                 // BLOCK_LOW_AND_ABOVE
                                 // BLOCK_MED_AND_ABOVE
                                 // BLOCK_HIGH_AND_ABOVE
        // @formatter:on
    }

    #endregion

    #region Response

    // https://cloud.google.com/vertex-ai/docs/generative-ai/model-reference/gemini#response_body

    [Serializable]
    internal class ResponseWrapper {
        public ResponseMessage[] objects;
    }

    [Serializable]
    internal struct ResponseMessage {
        public Candidate[] candidates;
        public PromptFeedback promptFeedback;
    }

    [Serializable]
    internal struct Candidate {
        public ResponseContent content;

        // Unspecified: The finish reason is unspecified.
        // Stop: Natural stop point of the model or provided stop sequence.
        // MaxTokens: The maximum number of tokens as specified in the request was reached.
        // Safety: The token generation was stopped as the response was flagged for safety reasons. Note that Candidate.content is empty if content filters block the output.
        // Recitation: The token generation was stopped as the response was flagged for unauthorized citations.
        // Other: All other reasons that stopped the token
        public string finishReason;

        public int index;
        public SafetyRating[] safetyRatings;
    }

    [Serializable]
    internal struct ResponseContent {
        public string role;
        public ResponsePart[] parts;
    }

    [Serializable]
    internal struct ResponsePart {
        public string text;
    }

    [Serializable]
    internal struct SafetyRating {
        public string category;
        public string probability;
    }

    [Serializable]
    internal struct UsageMetadata {
        public int promptTokenCount;
        public int candidatesTokenCount;
        public int totalTokenCount;
    }

    [Serializable]
    internal struct PromptFeedback {
        public SafetyRating[] safetyRatings;
    }

    #endregion

    private static IEnumerator GetRemoteConfig(GeminiParameters parameters, Action<long, string> failureCallback) {
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

    private class GeminiContainer : MonoBehaviour {
        private static GeminiContainer _instance;
        internal static GeminiContainer Instance {
            get {
                if (_instance == null) {
                    var container = new GameObject("GeminiContainer");
                    DontDestroyOnLoad(container);
                    container.hideFlags = HideFlags.HideInHierarchy;
                    _instance = container.AddComponent<GeminiContainer>();
                }

                return _instance;
            }
        }

        private void OnApplicationQuit() {
            CancelAllRequests();
        }
    }
}
}