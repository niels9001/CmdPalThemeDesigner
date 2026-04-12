// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.

using System.Text.Json.Serialization;

namespace CmdPalThemeDesigner.Core.Models;

/// <summary>
/// Backdrop/transparency settings for a theme.
/// </summary>
public sealed class BackdropSettings
{
    [JsonPropertyName("style")]
    public string Style { get; set; } = "Acrylic";

    [JsonPropertyName("tintOpacity")]
    public double TintOpacity { get; set; } = 0.85;

    [JsonPropertyName("luminosityOpacity")]
    public double LuminosityOpacity { get; set; } = 0.9;

    public BackdropSettings Clone() => new()
    {
        Style = Style,
        TintOpacity = TintOpacity,
        LuminosityOpacity = LuminosityOpacity,
    };
}
