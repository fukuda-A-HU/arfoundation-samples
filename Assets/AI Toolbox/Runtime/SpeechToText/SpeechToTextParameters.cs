using System;
using UnityEngine;

namespace AiToolbox {
/// <summary>
/// Settings for the AI Toolbox speech to text requests.
/// </summary>
[Serializable]
public class SpeechToTextParameters : ISerializationCallbackReceiver {
    public enum Model {
        [InspectorName("whisper-1")]
        Whisper1,
    }

    public string apiKey;
    public ApiKeyEncryption apiKeyEncryption;
    public string apiKeyRemoteConfigKey;
    public string apiKeyEncryptionPassword;

    [Tooltip("The language of the input audio. Supplying the input language in ISO-639-1 format will improve accuracy " +
             "and latency.")]
    public string language = "en";

    [Tooltip("An optional text to guide the model's style or continue a previous audio segment. The prompt should " +
             "match the audio language.")]
    public string prompt;

    [Tooltip("ID of the model to use. Only whisper-1 (which is powered by Whisper V2 model) is currently available.")]
    public Model model = Model.Whisper1;

    [Tooltip("The sampling temperature, between 0 and 1. Higher values like 0.8 will make the output more random, " +
             "while lower values like 0.2 will make it more focused and deterministic. If set to 0, the model will " +
             "use log probability to automatically increase the temperature until certain thresholds are hit.")]
    [Range(0.0f, 1.0f)]
    public float temperature = 0.0f;

    public int timeout;
    public int throttle;

    [SerializeField, HideInInspector]
    private bool _serialized;

    public SpeechToTextParameters(string apiKey) {
        this.apiKey = apiKey;
    }

    public SpeechToTextParameters(SpeechToTextParameters parameters) {
        apiKey = parameters.apiKey;
        apiKeyEncryption = parameters.apiKeyEncryption;
        apiKeyRemoteConfigKey = parameters.apiKeyRemoteConfigKey;
        apiKeyEncryptionPassword = parameters.apiKeyEncryptionPassword;
        timeout = parameters.timeout;
        throttle = parameters.throttle;
        _serialized = parameters._serialized;
    }

    public void OnBeforeSerialize() {
        if (_serialized) return;
        _serialized = true;
        timeout = 0;
        throttle = 0;
        apiKeyRemoteConfigKey = "openai_api_key";
        apiKeyEncryptionPassword = Guid.NewGuid().ToString();
    }

    public void OnAfterDeserialize() { }

    internal string GetModelString() {
        return model switch {
            Model.Whisper1 => "whisper-1",
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}
}