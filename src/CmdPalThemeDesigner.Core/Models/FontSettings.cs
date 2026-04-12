// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.

using System.Text.Json.Serialization;

namespace CmdPalThemeDesigner.Core.Models;

/// <summary>
/// Font configuration for a theme.
/// </summary>
public sealed class FontSettings
{
    [JsonPropertyName("primary")]
    public FontSpec Primary { get; set; } = new() { Family = "Segoe UI Variable", Size = 14, Weight = "Normal" };

    [JsonPropertyName("monospace")]
    public FontSpec Monospace { get; set; } = new() { Family = "Cascadia Code", Size = 13, Weight = "Normal" };

    [JsonPropertyName("title")]
    public FontSpec Title { get; set; } = new() { Family = "Segoe UI Variable", Size = 16, Weight = "SemiBold" };

    [JsonPropertyName("caption")]
    public FontSpec Caption { get; set; } = new() { Family = "Segoe UI Variable", Size = 12, Weight = "Normal" };

    public FontSettings Clone() => new()
    {
        Primary = Primary.Clone(),
        Monospace = Monospace.Clone(),
        Title = Title.Clone(),
        Caption = Caption.Clone(),
    };
}

/// <summary>
/// Specification for a single font slot.
/// </summary>
public sealed class FontSpec
{
    [JsonPropertyName("family")]
    public string Family { get; set; } = "Segoe UI Variable";

    [JsonPropertyName("size")]
    public double Size { get; set; } = 14;

    [JsonPropertyName("weight")]
    public string Weight { get; set; } = "Normal";

    public FontSpec Clone() => new() { Family = Family, Size = Size, Weight = Weight };
}
