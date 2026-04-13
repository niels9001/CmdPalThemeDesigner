// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.

using System.Text.Json.Serialization;

namespace CmdPalThemeDesigner.Core.Models;

/// <summary>
/// Root model for a CmdPal theme definition. Serialized to/from JSON.
/// v2 uses "palette" with ~11 semantic tokens; v1 "resources" with ~33 keys is still loadable.
/// </summary>
public sealed class ThemeDefinition
{
    [JsonPropertyName("$schema")]
    public string? Schema { get; set; }

    [JsonPropertyName("version")]
    public int Version { get; set; } = 2;

    [JsonPropertyName("name")]
    public string Name { get; set; } = "Untitled Theme";

    [JsonPropertyName("author")]
    public string? Author { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("fonts")]
    public FontSettings Fonts { get; set; } = new();

    /// <summary>
    /// v2 palette — ~11 semantic tokens that derive into full XAML resources.
    /// </summary>
    [JsonPropertyName("palette")]
    public ThemeResources Palette { get; set; } = new();

    /// <summary>
    /// Legacy v1 resources — ~33 XAML resource keys directly. Still loadable for backward compat.
    /// </summary>
    [JsonPropertyName("resources")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public ThemeResources? LegacyResources { get; set; }

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
            Palette = Palette.Clone(),
            LegacyResources = LegacyResources?.Clone(),
            Backdrop = Backdrop.Clone(),
        };
    }
}
