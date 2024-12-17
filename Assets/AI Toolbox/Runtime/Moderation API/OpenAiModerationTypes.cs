using System;
using Newtonsoft.Json;
using UnityEngine;

namespace AiToolbox {
// ReSharper disable InconsistentNaming
// ReSharper disable NotAccessedField.Local
#pragma warning disable 0649

[Serializable]
public struct ModerationResponse {
    public string id;
    public string model;
    public ModerationResult[] results;

    public float GetHighestScore() {
        var highestScore = 0f;
        foreach (var result in results) {
            if (result.category_scores.hate > highestScore) highestScore = result.category_scores.hate;
            if (result.category_scores.hateThreatening > highestScore)
                highestScore = result.category_scores.hateThreatening;
            if (result.category_scores.harassment > highestScore) highestScore = result.category_scores.harassment;
            if (result.category_scores.harassmentThreatening > highestScore)
                highestScore = result.category_scores.harassmentThreatening;
            if (result.category_scores.selfHarm > highestScore) highestScore = result.category_scores.selfHarm;
            if (result.category_scores.selfHarmIntent > highestScore)
                highestScore = result.category_scores.selfHarmIntent;
            if (result.category_scores.selfHarmInstructions > highestScore)
                highestScore = result.category_scores.selfHarmInstructions;
            if (result.category_scores.sexual > highestScore) highestScore = result.category_scores.sexual;
            if (result.category_scores.sexualMinors > highestScore) highestScore = result.category_scores.sexualMinors;
            if (result.category_scores.violence > highestScore) highestScore = result.category_scores.violence;
            if (result.category_scores.violenceGraphic > highestScore)
                highestScore = result.category_scores.violenceGraphic;
        }

        return highestScore;
    }

    public override string ToString() {
        return $"{string.Join("\n", results)}";
    }
}

// https://platform.openai.com/docs/api-reference/moderations/object
[Serializable]
public struct ModerationResult {
    public bool flagged;
    public ModerationCategories categories;
    public ModerationCategoryScores category_scores;

    public override string ToString() {
        // @formatter:off
        var s = $"<b>{(flagged ? "<color=red>Flagged</color>" : "<color=green>Safe</color>")}</b>\n";
        s += $"\thate: {categories.hate} ({category_scores.hate:0.000})\n";
        s += $"\thate/threatening: {categories.hateThreatening} ({category_scores.hateThreatening:0.000})\n";
        s += $"\tharassment: {categories.harassment} ({category_scores.harassment:0.000})\n";
        s += $"\tharassment/threatening: {categories.harassmentThreatening} ({category_scores.harassmentThreatening:0.000})\n";
        s += $"\tself-harm: {categories.selfHarm} ({category_scores.selfHarm:0.000})\n";
        s += $"\tself-harm/intent: {categories.selfHarmIntent} ({category_scores.selfHarmIntent:0.000})\n";
        s += $"\tself-harm/instructions: {categories.selfHarmInstructions} ({category_scores.selfHarmInstructions:0.000})\n";
        s += $"\tsexual: {categories.sexual} ({category_scores.sexual:0.000})\n";
        s += $"\tsexual/minors: {categories.sexualMinors} ({category_scores.sexualMinors:0.000})\n";
        s += $"\tviolence: {categories.violence} ({category_scores.violence:0.000})\n";
        s += $"\tviolence/graphic: {categories.violenceGraphic} ({category_scores.violenceGraphic:0.000})";
        // @formatter:on
        return s;
    }

    public string ToString(Color safeColor, Color flaggedColor) {
        var safeColorString = $"#{ColorUtility.ToHtmlStringRGB(safeColor)}";
        var flaggedColorString = $"#{ColorUtility.ToHtmlStringRGB(flaggedColor)}";
        // @formatter:off
        var s = $"<b>{(flagged?$"<color={flaggedColorString}>Flagged</color>" : $"<color={safeColorString}>Safe</color>")}</b>\n";
        s += $"<color={(categories.hate?$"{flaggedColorString}":$"{safeColorString}")}>hate</color>: {categories.hate} ({category_scores.hate:0.000})\n";
        s += $"<color={(categories.hateThreatening?$"{flaggedColorString}":$"{safeColorString}")}>hate/threatening</color>: {categories.hateThreatening} ({category_scores.hateThreatening:0.000})\n";
        s += $"<color={(categories.harassment?$"{flaggedColorString}":$"{safeColorString}")}>harassment</color>: {categories.harassment} ({category_scores.harassment:0.000})\n";
        s += $"<color={(categories.harassmentThreatening?$"{flaggedColorString}":$"{safeColorString}")}>harassment/threatening</color>: {categories.harassmentThreatening} ({category_scores.harassmentThreatening:0.000})\n";
        s += $"<color={(categories.selfHarm?$"{flaggedColorString}":$"{safeColorString}")}>self-harm</color>: {categories.selfHarm} ({category_scores.selfHarm:0.000})\n";
        s += $"<color={(categories.selfHarmIntent?$"{flaggedColorString}":$"{safeColorString}")}>self-harm/intent</color>: {categories.selfHarmIntent} ({category_scores.selfHarmIntent:0.000})\n";
        s += $"<color={(categories.selfHarmInstructions?$"{flaggedColorString}":$"{safeColorString}")}>self-harm/instructions</color>: {categories.selfHarmInstructions} ({category_scores.selfHarmInstructions:0.000})\n";
        s += $"<color={(categories.sexual?$"{flaggedColorString}":$"{safeColorString}")}>sexual</color>: {categories.sexual} ({category_scores.sexual:0.000})\n";
        s += $"<color={(categories.sexualMinors?$"{flaggedColorString}":$"{safeColorString}")}>sexual/minors</color>: {categories.sexualMinors} ({category_scores.sexualMinors:0.000})\n";
        s += $"<color={(categories.violence?$"{flaggedColorString}":$"{safeColorString}")}>violence</color>: {categories.violence} ({category_scores.violence:0.000})\n";
        s += $"<color={(categories.violenceGraphic?$"{flaggedColorString}":$"{safeColorString}")}>violence/graphic</color>: {categories.violenceGraphic} ({category_scores.violenceGraphic:0.000})";
        // @formatter:on
        return s;
    }
}

[Serializable]
public struct ModerationCategories {
    public bool hate;

    [JsonProperty("hate/threatening")]
    public bool hateThreatening;

    public bool harassment;

    [JsonProperty("harassment/threatening")]
    public bool harassmentThreatening;

    [JsonProperty("self-harm")]
    public bool selfHarm;

    [JsonProperty("self-harm/intent")]
    public bool selfHarmIntent;

    [JsonProperty("self-harm/instructions")]
    public bool selfHarmInstructions;

    public bool sexual;

    [JsonProperty("sexual/minors")]
    public bool sexualMinors;

    public bool violence;

    [JsonProperty("violence/graphic")]
    public bool violenceGraphic;
}

[Serializable]
public struct ModerationCategoryScores {
    public float hate;

    [JsonProperty("hate/threatening")]
    public float hateThreatening;

    public float harassment;

    [JsonProperty("harassment/threatening")]
    public float harassmentThreatening;

    [JsonProperty("self-harm")]
    public float selfHarm;

    [JsonProperty("self-harm/intent")]
    public float selfHarmIntent;

    [JsonProperty("self-harm/instructions")]
    public float selfHarmInstructions;

    public float sexual;

    [JsonProperty("sexual/minors")]
    public float sexualMinors;

    public float violence;

    [JsonProperty("violence/graphic")]
    public float violenceGraphic;
}
}