using System;
using UnityEngine;

namespace AiToolbox {
/// <summary>
/// The Gemini model to use.
/// Models are described here: https://ai.google.dev/gemini-api/docs/models/gemini#model-variations
/// </summary>
public enum GeminiModel {
    [InspectorName("Gemini 1.0 Pro")]
    Gemini10Pro = 0,
    [InspectorName("Gemini 1.5 Pro")]
    Gemini15Pro = 1,
    [InspectorName("Gemini 1.5 Flash")]
    Gemini15Flash = 2,
}

internal static class GeminiExtensions {
    public static string GetModelCode(this GeminiModel model) {
        var id = model switch {
            GeminiModel.Gemini10Pro => "gemini-1.0-pro",
            GeminiModel.Gemini15Pro => "gemini-1.5-pro-latest",
            GeminiModel.Gemini15Flash => "gemini-1.5-flash-latest",
            _ => throw new ArgumentOutOfRangeException()
        };
        return id;
    }
}
}