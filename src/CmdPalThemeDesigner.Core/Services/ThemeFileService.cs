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
        return theme ?? throw new InvalidOperationException("Failed to deserialize theme definition.");
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

            if (!root.TryGetProperty("resources", out var resources))
            {
                errors.Add("Missing 'resources' section.");
            }
            else
            {
                if (!resources.TryGetProperty("dark", out _))
                    errors.Add("Missing 'resources.dark' section.");
                if (!resources.TryGetProperty("light", out _))
                    errors.Add("Missing 'resources.light' section.");
            }
        }
        catch (JsonException ex)
        {
            errors.Add($"Invalid JSON: {ex.Message}");
        }

        return errors;
    }
}
