using System;

namespace AiToolbox {
[Serializable]
public class OpenAiApiError {
    public string message;
    public string type;
    public object param;
    public object code;
}
}