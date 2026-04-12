// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.

using System.Text.Json.Serialization;

namespace CmdPalThemeDesigner.Core.Models;

/// <summary>
/// Theme resources organized by theme variant (dark/light).
/// Each variant is a dictionary of resource key → color hex string.
/// </summary>
public sealed class ThemeResources
{
    [JsonPropertyName("dark")]
    public Dictionary<string, string> Dark { get; set; } = new();

    [JsonPropertyName("light")]
    public Dictionary<string, string> Light { get; set; } = new();

    public ThemeResources Clone() => new()
    {
        Dark = new Dictionary<string, string>(Dark),
        Light = new Dictionary<string, string>(Light),
    };

    /// <summary>
    /// Gets all known resource keys across both variants.
    /// </summary>
    public IReadOnlySet<string> AllKeys
    {
        get
        {
            var keys = new HashSet<string>(Dark.Keys);
            keys.UnionWith(Light.Keys);
            return keys;
        }
    }
}
