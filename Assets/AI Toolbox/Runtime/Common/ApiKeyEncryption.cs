using UnityEngine;

namespace AiToolbox {
/// <summary>
/// The security and origin of the OpenAI API key.
/// </summary>
public enum ApiKeyEncryption {
    None = 0,
    [InspectorName("Locally encrypted")]
    LocallyEncrypted = 1,
    [InspectorName("RemoteConfig")]
    RemoteConfig = 2,
}
}