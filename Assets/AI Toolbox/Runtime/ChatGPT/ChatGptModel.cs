using UnityEngine;

// ReSharper disable InconsistentNaming

namespace AiToolbox {
/// <summary>
/// The ChatGPT model to use.
/// Models are described here: https://platform.openai.com/docs/models/overview
/// </summary>
public enum ChatGptModel {
    [InspectorName("gpt-3.5-turbo")]
    Gpt35Turbo = 0,
    [InspectorName("gpt-4")]
    Gpt4 = 1,
    [InspectorName("gpt-4-turbo")]
    Gpt4Turbo = 2,
    // ReSharper disable once InconsistentNaming
    [InspectorName("gpt-4o")]
    Gpt4o = 3,
    // ReSharper disable once InconsistentNaming
    [InspectorName("gpt-4o-mini")]
    Gpt4oMini = 4,
    [InspectorName("o1-mini")]
    o1Mini = 5,
    [InspectorName("o1-preview")]
    o1Preview = 6,
}
}