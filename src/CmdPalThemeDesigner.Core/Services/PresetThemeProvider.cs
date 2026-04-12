// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.

using System.Reflection;
using System.Text.Json;
using CmdPalThemeDesigner.Core.Models;

namespace CmdPalThemeDesigner.Core.Services;

/// <summary>
/// Provides access to built-in preset themes embedded as resources.
/// </summary>
public sealed class PresetThemeProvider
{
    private readonly List<ThemeDefinition> _presets = new();
    private bool _loaded;

    /// <summary>
    /// Gets all available preset themes.
    /// </summary>
    public IReadOnlyList<ThemeDefinition> Presets
    {
        get
        {
            EnsureLoaded();
            return _presets;
        }
    }

    private void EnsureLoaded()
    {
        if (_loaded)
            return;

        _loaded = true;
        var assembly = Assembly.GetExecutingAssembly();
        var resourceNames = assembly.GetManifestResourceNames()
            .Where(n => n.Contains("Presets") && n.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
            .OrderBy(n => n);

        var fileService = new ThemeFileService();

        foreach (var name in resourceNames)
        {
            try
            {
                using var stream = assembly.GetManifestResourceStream(name);
                if (stream == null)
                    continue;

                using var reader = new StreamReader(stream);
                var json = reader.ReadToEnd();
                var theme = fileService.Deserialize(json);
                _presets.Add(theme);
            }
            catch
            {
                // Skip invalid preset files
            }
        }
    }

    /// <summary>
    /// Gets a preset by name (case-insensitive).
    /// </summary>
    public ThemeDefinition? GetByName(string name)
    {
        EnsureLoaded();
        return _presets.FirstOrDefault(p =>
            p.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
    }
}
