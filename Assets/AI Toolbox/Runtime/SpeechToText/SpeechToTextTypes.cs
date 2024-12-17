using System;

namespace AiToolbox {
// https://platform.openai.com/docs/api-reference/audio/createSpeech
[Serializable]
public struct SpeechToTextResponse {
    public OpenAiApiError error;
    public string warning;
    public string text;
}
}