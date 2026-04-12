// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.

using CmdPalThemeDesigner.Core.Models;
using Microsoft.UI;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using Windows.UI;
using Windows.UI.Text;

namespace CmdPalThemeDesigner.Core.Services;

/// <summary>
/// Converts ThemeDefinition models into WinUI ResourceDictionaries and manages
/// live theme swapping at runtime.
/// </summary>
public sealed class ThemeEngine
{
    private ThemeDefinition? _current;
    private string _variant = "dark";
    private ResourceDictionary? _brushDictionary;  // PreviewStyles.xaml — update brushes in-place
    private ResourceDictionary? _fontDictionary;    // Separate writable dict for font overrides

    /// <summary>
    /// Fires whenever the theme resources are updated.
    /// </summary>
    public event EventHandler? ThemeChanged;

    /// <summary>
    /// Gets or sets the current theme definition.
    /// </summary>
    public ThemeDefinition? Current
    {
        get => _current;
        set
        {
            _current = value;
            ApplyTheme();
        }
    }

    /// <summary>
    /// Sets the current theme variant ("dark" or "light") and re-applies.
    /// </summary>
    public void SetThemeVariant(string variant)
    {
        _variant = variant;
        ApplyTheme();
    }

    /// <summary>
    /// Registers the brush dictionary (PreviewStyles.xaml) for in-place color updates,
    /// and a separate writable dictionary for font overrides.
    /// </summary>
    public void RegisterDictionaries(ResourceDictionary brushDict, ResourceDictionary fontDict)
    {
        _brushDictionary = brushDict;
        _fontDictionary = fontDict;
    }

    /// <summary>
    /// Builds a ResourceDictionary from a ThemeDefinition for a specific variant.
    /// </summary>
    public ResourceDictionary BuildDictionary(ThemeDefinition theme, string variant)
    {
        var dict = new ResourceDictionary();
        var colors = variant == "light" ? theme.Resources.Light : theme.Resources.Dark;

        foreach (var (key, value) in colors)
        {
            if (TryParseColor(value, out var color))
            {
                dict[key] = new SolidColorBrush(color);
            }
        }

        ApplyFontFamilyResources(dict, theme.Fonts);

        return dict;
    }

    /// <summary>
    /// Builds a ResourceDictionary using ThemeDictionaries for both Dark and Light variants.
    /// </summary>
    public ResourceDictionary BuildThemedDictionary(ThemeDefinition theme)
    {
        var root = new ResourceDictionary();

        var darkDict = BuildVariantDictionary(theme.Resources.Dark);
        var darkDictCopy = BuildVariantDictionary(theme.Resources.Dark);
        var lightDict = BuildVariantDictionary(theme.Resources.Light);

        root.ThemeDictionaries["Default"] = darkDict;
        root.ThemeDictionaries["Dark"] = darkDictCopy;
        root.ThemeDictionaries["Light"] = lightDict;

        ApplyFontFamilyResources(root, theme.Fonts);

        return root;
    }

    private static ResourceDictionary BuildVariantDictionary(Dictionary<string, string> colors)
    {
        var dict = new ResourceDictionary();
        foreach (var (key, value) in colors)
        {
            if (TryParseColor(value, out var color))
            {
                dict[key] = new SolidColorBrush(color);
            }
        }
        return dict;
    }

    private void ApplyTheme()
    {
        if (_current == null || _brushDictionary == null)
            return;

        // Get the theme dictionaries from PreviewStyles.xaml (already bound by XAML elements)
        ResourceDictionary? defaultDict = null;
        ResourceDictionary? lightDict = null;

        if (_brushDictionary.ThemeDictionaries.ContainsKey("Default"))
            defaultDict = _brushDictionary.ThemeDictionaries["Default"] as ResourceDictionary;
        if (_brushDictionary.ThemeDictionaries.ContainsKey("Light"))
            lightDict = _brushDictionary.ThemeDictionaries["Light"] as ResourceDictionary;

        // Update dark/default theme brushes in-place
        foreach (var (key, value) in _current.Resources.Dark)
        {
            if (TryParseColor(value, out var color))
            {
                UpdateBrushInDict(defaultDict, key, color);
            }
        }

        // Update light theme brushes in-place
        foreach (var (key, value) in _current.Resources.Light)
        {
            if (TryParseColor(value, out var color))
            {
                UpdateBrushInDict(lightDict, key, color);
            }
        }

        // Font families go into the separate writable dictionary
        if (_fontDictionary != null)
        {
            if (!string.IsNullOrEmpty(_current.Fonts.Primary.Family))
                _fontDictionary[ThemeResourceKeys.PrimaryFontFamily] = new FontFamily(_current.Fonts.Primary.Family);
            if (!string.IsNullOrEmpty(_current.Fonts.Title.Family))
                _fontDictionary[ThemeResourceKeys.TitleFontFamily] = new FontFamily(_current.Fonts.Title.Family);
            if (!string.IsNullOrEmpty(_current.Fonts.Caption.Family))
                _fontDictionary[ThemeResourceKeys.CaptionFontFamily] = new FontFamily(_current.Fonts.Caption.Family);
            if (!string.IsNullOrEmpty(_current.Fonts.Monospace.Family))
                _fontDictionary[ThemeResourceKeys.MonospaceFontFamily] = new FontFamily(_current.Fonts.Monospace.Family);
        }

        ThemeChanged?.Invoke(this, EventArgs.Empty);
    }

    private static void UpdateBrushInDict(ResourceDictionary? dict, string key, Color color)
    {
        if (dict == null)
            return;

        if (dict.ContainsKey(key) && dict[key] is SolidColorBrush existingBrush)
        {
            // Update Color in-place — triggers UI refresh since Color is a DependencyProperty
            existingBrush.Color = color;
        }
    }

    private static void ApplyFontFamilyResources(ResourceDictionary dict, FontSettings fonts)
    {
        if (!string.IsNullOrEmpty(fonts.Primary.Family))
            dict[ThemeResourceKeys.PrimaryFontFamily] = new FontFamily(fonts.Primary.Family);
        if (!string.IsNullOrEmpty(fonts.Title.Family))
            dict[ThemeResourceKeys.TitleFontFamily] = new FontFamily(fonts.Title.Family);
        if (!string.IsNullOrEmpty(fonts.Caption.Family))
            dict[ThemeResourceKeys.CaptionFontFamily] = new FontFamily(fonts.Caption.Family);
        if (!string.IsNullOrEmpty(fonts.Monospace.Family))
            dict[ThemeResourceKeys.MonospaceFontFamily] = new FontFamily(fonts.Monospace.Family);
    }

    /// <summary>
    /// Parses a hex color string (#RRGGBB or #AARRGGBB) into a Color.
    /// Also supports "Transparent".
    /// </summary>
    public static bool TryParseColor(string value, out Color color)
    {
        color = default;

        if (string.IsNullOrWhiteSpace(value))
            return false;

        if (value.Equals("Transparent", StringComparison.OrdinalIgnoreCase))
        {
            color = Colors.Transparent;
            return true;
        }

        if (!value.StartsWith('#'))
            return false;

        var hex = value[1..];

        try
        {
            switch (hex.Length)
            {
                case 6: // #RRGGBB
                    color = Color.FromArgb(
                        255,
                        Convert.ToByte(hex[0..2], 16),
                        Convert.ToByte(hex[2..4], 16),
                        Convert.ToByte(hex[4..6], 16));
                    return true;

                case 8: // #AARRGGBB
                    color = Color.FromArgb(
                        Convert.ToByte(hex[0..2], 16),
                        Convert.ToByte(hex[2..4], 16),
                        Convert.ToByte(hex[4..6], 16),
                        Convert.ToByte(hex[6..8], 16));
                    return true;

                default:
                    return false;
            }
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Converts a Color to a hex string (#AARRGGBB or #RRGGBB).
    /// </summary>
    public static string ColorToHex(Color color)
    {
        return color.A == 255
            ? $"#{color.R:X2}{color.G:X2}{color.B:X2}"
            : $"#{color.A:X2}{color.R:X2}{color.G:X2}{color.B:X2}";
    }

    /// <summary>
    /// Parses a font weight string (e.g., "Normal", "Bold", "SemiBold") to a FontWeight.
    /// </summary>
    public static FontWeight ParseFontWeight(string weight)
    {
        return weight?.ToLowerInvariant() switch
        {
            "thin" => FontWeights.Thin,
            "extralight" or "extra-light" => FontWeights.ExtraLight,
            "light" => FontWeights.Light,
            "semilight" or "semi-light" => FontWeights.SemiLight,
            "normal" or "regular" => FontWeights.Normal,
            "medium" => FontWeights.Medium,
            "semibold" or "semi-bold" => FontWeights.SemiBold,
            "bold" => FontWeights.Bold,
            "extrabold" or "extra-bold" => FontWeights.ExtraBold,
            "black" or "heavy" => FontWeights.Black,
            "extrablack" or "extra-black" => FontWeights.ExtraBlack,
            _ => FontWeights.Normal,
        };
    }
}
