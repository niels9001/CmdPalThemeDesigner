// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.

using System.Text.Json;
using CmdPalThemeDesigner.Core.Models;

namespace CmdPalThemeDesigner.Core.Services;

/// <summary>
/// Service for loading and saving .json theme files.
/// </summary>
public sealed class ThemeFileService
{
    private static readonly JsonSerializerOptions s_options = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true,
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
    };

    /// <summary>
    /// Loads a theme definition from a JSON file path.
    /// </summary>
    public async Task<ThemeDefinition> LoadAsync(string filePath)
    {
        var json = await File.ReadAllTextAsync(filePath);
        return Deserialize(json);
    }

    /// <summary>
    /// Saves a theme definition to a JSON file path.
    /// </summary>
    public async Task SaveAsync(ThemeDefinition theme, string filePath)
    {
        var json = Serialize(theme);
        await File.WriteAllTextAsync(filePath, json);
    }

    /// <summary>
    /// Deserializes a theme definition from a JSON string.
    /// </summary>
    public ThemeDefinition Deserialize(string json)
    {
        var theme = JsonSerializer.Deserialize<ThemeDefinition>(json, s_options);
        if (theme == null)
            throw new InvalidOperationException("Failed to deserialize theme definition.");

        // Migrate v1 → v2: if legacy resources exist but palette is empty, auto-convert
        if (theme.LegacyResources != null && theme.Palette.Dark.Count == 0 && theme.LegacyResources.Dark.Count > 0)
        {
            theme.Palette = ConvertLegacyToPalette(theme.LegacyResources);
            theme.Version = 2;
        }

        return theme;
    }

    /// <summary>
    /// Converts v1 resources (33 XAML keys) to v2 palette (~11 semantic tokens).
    /// Uses reverse mapping to pick the best palette value from XAML keys.
    /// </summary>
    private static ThemeResources ConvertLegacyToPalette(ThemeResources legacy)
    {
        var result = new ThemeResources();
        result.Dark = ExtractPaletteFromLegacy(legacy.Dark);
        result.Light = ExtractPaletteFromLegacy(legacy.Light);
        return result;
    }

    private static Dictionary<string, string> ExtractPaletteFromLegacy(Dictionary<string, string> legacyColors)
    {
        var palette = new Dictionary<string, string>();

        // Build reverse mapping: XAML key → palette token (first match wins)
        var derivedMappings = ThemeResourceKeys.GetDerivedMappings();
        var reverseMap = new Dictionary<string, string>();
        foreach (var (token, xamlKeys) in derivedMappings)
        {
            foreach (var xamlKey in xamlKeys)
            {
                reverseMap.TryAdd(xamlKey, token);
            }
        }

        // For each XAML key in the legacy data, find its palette token
        foreach (var (xamlKey, colorHex) in legacyColors)
        {
            if (reverseMap.TryGetValue(xamlKey, out var token) && !palette.ContainsKey(token))
            {
                palette[token] = colorHex;
            }
        }

        return palette;
    }

    /// <summary>
    /// Serializes a theme definition to a formatted JSON string.
    /// </summary>
    public string Serialize(ThemeDefinition theme)
    {
        return JsonSerializer.Serialize(theme, s_options);
    }

    /// <summary>
    /// Validates a JSON string against the expected theme schema structure.
    /// Returns a list of validation errors (empty if valid).
    /// </summary>
    public IReadOnlyList<string> Validate(string json)
    {
        var errors = new List<string>();

        try
        {
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            if (!root.TryGetProperty("version", out var version) || version.GetInt32() < 1)
            {
                errors.Add("Missing or invalid 'version' field.");
            }

            if (!root.TryGetProperty("name", out _))
            {
                errors.Add("Missing 'name' field.");
            }

            if (!root.TryGetProperty("palette", out var palette) &&
                !root.TryGetProperty("resources", out _))
            {
                errors.Add("Missing 'palette' (v2) or 'resources' (v1) section.");
            }
            else if (palette.ValueKind != default)
            {
                if (!palette.TryGetProperty("dark", out _))
                    errors.Add("Missing 'palette.dark' section.");
            }
            else if (root.TryGetProperty("resources", out var resources))
            {
                if (!resources.TryGetProperty("dark", out _))
                    errors.Add("Missing 'resources.dark' section.");
            }
        }
        catch (JsonException ex)
        {
            errors.Add($"Invalid JSON: {ex.Message}");
        }

        return errors;
    }
}
