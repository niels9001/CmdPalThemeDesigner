// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.

using System.Text.Json.Serialization;

namespace CmdPalThemeDesigner.Core.Models;

/// <summary>
/// Root model for a CmdPal theme definition. Serialized to/from JSON.
/// </summary>
public sealed class ThemeDefinition
{
    [JsonPropertyName("$schema")]
    public string? Schema { get; set; }

    [JsonPropertyName("version")]
    public int Version { get; set; } = 1;

    [JsonPropertyName("name")]
    public string Name { get; set; } = "Untitled Theme";

    [JsonPropertyName("author")]
    public string? Author { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("fonts")]
    public FontSettings Fonts { get; set; } = new();

    [JsonPropertyName("resources")]
    public ThemeResources Resources { get; set; } = new();

    [JsonPropertyName("backdrop")]
    public BackdropSettings Backdrop { get; set; } = new();

    /// <summary>
    /// Creates a deep clone of this theme definition.
    /// </summary>
    public ThemeDefinition Clone()
    {
        return new ThemeDefinition
        {
            Schema = Schema,
            Version = Version,
            Name = Name,
            Author = Author,
            Description = Description,
            Fonts = Fonts.Clone(),
            Resources = Resources.Clone(),
            Backdrop = Backdrop.Clone(),
        };
    }
}
