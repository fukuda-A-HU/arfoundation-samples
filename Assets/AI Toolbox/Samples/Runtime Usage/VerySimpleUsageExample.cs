using AiToolbox;
using UnityEngine;

namespace AiToolboxRuntimeExample {
public class VerySimpleUsageExample : MonoBehaviour {
    public ChatGptParameters parameters;
    public string prompt = "Generate a character description";

    void Start() {
        // Check if the API Key is set in the Inspector, just in case.
        if (parameters == null || string.IsNullOrEmpty(parameters.apiKey)) {
            const string errorMessage = "Please set the <b>API Key</b> in the <b>ChatGPT Dialogue</b> Game Object.";
            Debug.LogError(errorMessage);
            return;
        }

        // This request provides only `completeCallback` and `failureCallback` parameters. Since the `updateCallback`
        // is not provided, the request will be completed in one step, and the `completeCallback` will be called only
        // once, with the full text of the answer.
        ChatGpt.Request(prompt, parameters, response => {
            Debug.Log("Full response: " + response);
        }, (errorCode, errorMessage) => {
            var errorType = (ErrorCodes)errorCode;
            Debug.LogError("Error: " + errorType + " - " + errorMessage);
        });

        // This request provides all three callbacks: `completeCallback`, `updateCallback`, and `failureCallback`.
        // Since the `updateCallback` is provided, the request will be completed in multiple steps, and the
        // `completeCallback` will be called only once, with the full text of the answer.
        ChatGpt.Request(prompt, parameters, response => {
            Debug.Log("Full response: " + response);
        }, (errorCode, errorMessage) => {
            var errorType = (ErrorCodes)errorCode;
            Debug.LogError("Error: " + errorType + " - " + errorMessage);
        }, chunk => {
            Debug.Log("Next part of response: " + chunk);
        });
    }

    void OnDestroy() {
        ChatGpt.CancelAllRequests();
    }
}
}