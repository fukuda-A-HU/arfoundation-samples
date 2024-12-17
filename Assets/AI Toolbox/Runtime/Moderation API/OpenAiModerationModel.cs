using UnityEngine;

namespace AiToolbox {
/// <summary>
/// The OpenAI content moderation model to use.
/// Models are described here: https://platform.openai.com/docs/api-reference/moderations/create#moderations-create-model
/// </summary>
public enum OpenAiModerationModel {
    [InspectorName("Latest")]
    Latest = 0,
    [InspectorName("Stable")]
    Stable = 1,
}
}