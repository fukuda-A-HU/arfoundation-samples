using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Serialization;

namespace AiToolbox {
/// <summary>
/// Settings for the AI Toolbox Gemini requests.
/// </summary>
[Serializable]
public class GeminiParameters : ISerializationCallbackReceiver {
    public string apiKey;
    public ApiKeyEncryption apiKeyEncryption;
    public string apiKeyRemoteConfigKey;
    public string apiKeyEncryptionPassword;

    public GeminiModel model = GeminiModel.Gemini15Flash;
    public float temperature;
    [CanBeNull]
    public string role;

    public int timeout;
    public int throttle;

    [SerializeField, HideInInspector, FormerlySerializedAs("serialized")]
    private bool _serialized;

    public GeminiParameters(string apiKey) {
        this.apiKey = apiKey;
    }

    public GeminiParameters(GeminiParameters parameters) {
        apiKey = parameters.apiKey;
        apiKeyEncryption = parameters.apiKeyEncryption;
        apiKeyRemoteConfigKey = parameters.apiKeyRemoteConfigKey;
        apiKeyEncryptionPassword = parameters.apiKeyEncryptionPassword;
        model = parameters.model;
        temperature = parameters.temperature;
        timeout = parameters.timeout;
        role = parameters.role;
        _serialized = parameters._serialized;
        throttle = parameters.throttle;
    }

    public void OnBeforeSerialize() {
        if (_serialized) return;
        _serialized = true;
        temperature = 1;
        timeout = 0;
        throttle = 0;
        apiKeyRemoteConfigKey = "gemini_api_key";
        apiKeyEncryptionPassword = Guid.NewGuid().ToString();
    }

    public void OnAfterDeserialize() { }
}
}