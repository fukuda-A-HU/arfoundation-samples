using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Serialization;

namespace AiToolbox {
/// <summary>
/// Settings for the AI Toolbox ChatGPT requests.
/// </summary>
[Serializable]
public class ChatGptParameters : ISerializationCallbackReceiver {
    public string apiKey;
    public ApiKeyEncryption apiKeyEncryption;
    public string apiKeyRemoteConfigKey;
    public string apiKeyEncryptionPassword;

    public ChatGptModel model;
    public float temperature;
    [CanBeNull]
    public string role;

    public int timeout;
    public int throttle;

    [SerializeField, HideInInspector, FormerlySerializedAs("serialized")]
    private bool _serialized;

    public ChatGptParameters(string apiKey) {
        this.apiKey = apiKey;
    }

    public ChatGptParameters(ChatGptParameters parameters) {
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
        apiKeyRemoteConfigKey = "openai_api_key";
        apiKeyEncryptionPassword = Guid.NewGuid().ToString();
    }

    public void OnAfterDeserialize() { }
}
}