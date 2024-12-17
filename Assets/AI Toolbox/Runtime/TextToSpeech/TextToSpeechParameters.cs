using System;
using UnityEngine;

namespace AiToolbox {
/// <summary>
/// Settings for the AI Toolbox speech to text requests.
/// </summary>
[Serializable]
public class TextToSpeechParameters : ISerializationCallbackReceiver {
    public enum Voice {
        Alloy,
        Echo,
        Fable,
        Onyx,
        Nova,
        Shimmer,
    }

    public enum Model {
        [InspectorName("tts-1")]
        Tts1,
        [InspectorName("tts-1-hd")]
        Tts1Hd,
    }

    public string apiKey;
    public ApiKeyEncryption apiKeyEncryption;
    public string apiKeyRemoteConfigKey;
    public string apiKeyEncryptionPassword;

    [Tooltip("tts-1 is optimized for real time text to speech use cases and tts-1-hd is optimized for quality.")]
    public Model model = Model.Tts1;

    [Tooltip("The voice to use for the speech synthesis.")]
    public Voice voice;

    [Range(0.25f, 4.0f)]
    [Tooltip("The speed of the generated speech. 1.0 is the default value.")]
    public float speed = 1.0f;

    public int timeout;
    public int throttle;

    [SerializeField, HideInInspector]
    private bool _serialized;

    public TextToSpeechParameters(string apiKey) {
        this.apiKey = apiKey;
    }

    public TextToSpeechParameters(TextToSpeechParameters parameters) {
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
            Model.Tts1 => "tts-1",
            Model.Tts1Hd => "tts-1-hd",
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}
}