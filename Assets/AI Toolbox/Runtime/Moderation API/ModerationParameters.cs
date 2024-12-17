using System;
using AiToolbox;
using UnityEngine;

namespace AiToolbox {
/// <summary>
/// Settings for the AI Toolbox ChatGPT requests.
/// </summary>
[Serializable]
public class ModerationParameters : ISerializationCallbackReceiver {
    public string apiKey;
    public ApiKeyEncryption apiKeyEncryption;
    public string apiKeyRemoteConfigKey;
    public string apiKeyEncryptionPassword;

    public OpenAiModerationModel moderationModel;

    public int timeout;
    public int throttle;

    [SerializeField, HideInInspector]
    private bool _serialized;

    public ModerationParameters(string apiKey) {
        this.apiKey = apiKey;
    }

    public ModerationParameters(ModerationParameters parameters) {
        apiKey = parameters.apiKey;
        apiKeyEncryption = parameters.apiKeyEncryption;
        apiKeyRemoteConfigKey = parameters.apiKeyRemoteConfigKey;
        apiKeyEncryptionPassword = parameters.apiKeyEncryptionPassword;
        moderationModel = parameters.moderationModel;
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
}
}