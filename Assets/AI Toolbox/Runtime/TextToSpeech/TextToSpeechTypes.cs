using System;

// ReSharper disable InconsistentNaming

namespace AiToolbox {
// https://platform.openai.com/docs/guides/text-to-speech

[Serializable]
public struct TextToSpeechRequest {
    public string model;
    public string input;
    public string voice;
    public float speed; // 0.25 to 4.0
}
}