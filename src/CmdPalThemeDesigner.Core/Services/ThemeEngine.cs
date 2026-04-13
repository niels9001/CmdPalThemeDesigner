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
/// Converts ThemeDefinition palette tokens into XAML resources.
/// Expands ~11 semantic palette entries into ~33 XAML resource keys,
/// plus auto-derived interaction states (hover, selected, pressed).
/// </summary>
public sealed class ThemeEngine
{
    private ThemeDefinition? _current;
    private ResourceDictionary? _brushDictionary;  // PreviewStyles.xaml — update brushes in-place
    private ResourceDictionary? _fontDictionary;    // Separate writable dict for font overrides

    public event EventHandler? ThemeChanged;

    public ThemeDefinition? Current
    {
        get => _current;
        set
        {
            _current = value;
            ApplyTheme();
        }
    }

    public void RegisterDictionaries(ResourceDictionary brushDict, ResourceDictionary fontDict)
    {
        _brushDictionary = brushDict;
        _fontDictionary = fontDict;
    }

    /// <summary>
    /// Expands palette tokens into the full set of XAML resource key→color mappings.
    /// Auto-derives hover/selected/pressed tints.
    /// </summary>
    public static Dictionary<string, string> ExpandPalette(Dictionary<string, string> palette)
    {
        var result = new Dictionary<string, string>();
        var mappings = ThemeResourceKeys.GetDerivedMappings();

        // Apply direct mappings: palette token → multiple XAML keys
        foreach (var (token, xamlKeys) in mappings)
        {
            if (palette.TryGetValue(token, out var colorHex))
            {
                foreach (var xamlKey in xamlKeys)
                {
                    result[xamlKey] = colorHex;
                }
            }
        }

        // Auto-derived: ForegroundDisabled → no direct mapping, but keep in palette
        // AccentText → no direct mapping yet

        // Auto-derive interaction states from palette values
        var isDark = IsDarkPalette(palette);

        // ListItem backgrounds: transparent for normal
        result[ThemeResourceKeys.Xaml_ListItemBg] = "Transparent";
        result[ThemeResourceKeys.Xaml_DockItemBg] = "Transparent";

        // Hover tint: subtle white overlay (dark) or black overlay (light)
        var hoverTint = isDark ? "#14FFFFFF" : "#09000000";
        result[ThemeResourceKeys.Xaml_ListItemBgHover] = hoverTint;
        result[ThemeResourceKeys.Xaml_ButtonBgHover] = hoverTint;
        result[ThemeResourceKeys.Xaml_ContextMenuItemHover] = hoverTint;
        result[ThemeResourceKeys.Xaml_DockItemBgHover] = hoverTint;

        // Selected tint: accent at ~20% opacity
        if (palette.TryGetValue(ThemeResourceKeys.Accent, out var accentHex) &&
            TryParseColor(accentHex, out var accentColor))
        {
            var selectedTint = ColorToHex(Color.FromArgb(48, accentColor.R, accentColor.G, accentColor.B));
            result[ThemeResourceKeys.Xaml_ListItemBgSelected] = selectedTint;
        }
        else
        {
            result[ThemeResourceKeys.Xaml_ListItemBgSelected] = isDark ? "#1AFFFFFF" : "#0F000000";
        }

        // Pressed tint
        var pressedTint = isDark ? "#0AFFFFFF" : "#12000000";
        result[ThemeResourceKeys.Xaml_DockItemBgPressed] = pressedTint;

        // Dock border hover — use border.default if available
        if (palette.TryGetValue(ThemeResourceKeys.BorderDefault, out var borderHex))
        {
            result[ThemeResourceKeys.Xaml_DockItemBorderHover] = borderHex;
        }

        return result;
    }

    private static bool IsDarkPalette(Dictionary<string, string> palette)
    {
        // Heuristic: if primary foreground is light, it's a dark theme
        if (palette.TryGetValue(ThemeResourceKeys.ForegroundPrimary, out var fgHex) &&
            TryParseColor(fgHex, out var fg))
        {
            return (fg.R + fg.G + fg.B) / 3.0 > 128;
        }
        return true; // default to dark
    }

    private void ApplyTheme()
    {
        if (_current == null || _brushDictionary == null)
            return;

        ResourceDictionary? defaultDict = null;
        ResourceDictionary? lightDict = null;

        if (_brushDictionary.ThemeDictionaries.ContainsKey("Default"))
            defaultDict = _brushDictionary.ThemeDictionaries["Default"] as ResourceDictionary;
        if (_brushDictionary.ThemeDictionaries.ContainsKey("Light"))
            lightDict = _brushDictionary.ThemeDictionaries["Light"] as ResourceDictionary;

        // Expand palette → full XAML resources
        var darkExpanded = ExpandPalette(_current.Palette.Dark);
        var lightExpanded = ExpandPalette(_current.Palette.Light);

        // Update dark/default theme brushes in-place
        foreach (var (key, value) in darkExpanded)
        {
            if (TryParseColor(value, out var color))
            {
                UpdateBrushInDict(defaultDict, key, color);
            }
        }

        // Update light theme brushes in-place
        foreach (var (key, value) in lightExpanded)
        {
            if (TryParseColor(value, out var color))
            {
                UpdateBrushInDict(lightDict, key, color);
            }
        }

        // Font families go into ThemeDictionaries of the font dict.
        // FontFamily is immutable, so we must replace the entire inner dict
        // to get {ThemeResource} to pick up new values on theme toggle.
        if (_fontDictionary != null)
        {
            var fontEntries = new Dictionary<string, FontFamily>();
            if (!string.IsNullOrEmpty(_current.Fonts.Primary.Family))
                fontEntries[ThemeResourceKeys.PrimaryFontFamily] = new FontFamily(_current.Fonts.Primary.Family);
            if (!string.IsNullOrEmpty(_current.Fonts.Title.Family))
                fontEntries[ThemeResourceKeys.TitleFontFamily] = new FontFamily(_current.Fonts.Title.Family);
            if (!string.IsNullOrEmpty(_current.Fonts.Caption.Family))
                fontEntries[ThemeResourceKeys.CaptionFontFamily] = new FontFamily(_current.Fonts.Caption.Family);
            if (!string.IsNullOrEmpty(_current.Fonts.Monospace.Family))
                fontEntries[ThemeResourceKeys.MonospaceFontFamily] = new FontFamily(_current.Fonts.Monospace.Family);

            // Build fresh ThemeDictionaries for Default/Dark/Light so
            // {ThemeResource} re-resolves on RequestedTheme toggle
            var darkFontDict = new ResourceDictionary();
            var darkFontDict2 = new ResourceDictionary();
            var lightFontDict = new ResourceDictionary();
            foreach (var (key, font) in fontEntries)
            {
                darkFontDict[key] = font;
                darkFontDict2[key] = new FontFamily(font.Source);
                lightFontDict[key] = new FontFamily(font.Source);
            }

            _fontDictionary.ThemeDictionaries["Default"] = darkFontDict;
            _fontDictionary.ThemeDictionaries["Dark"] = darkFontDict2;
            _fontDictionary.ThemeDictionaries["Light"] = lightFontDict;
        }

        ThemeChanged?.Invoke(this, EventArgs.Empty);
    }

    private static void UpdateBrushInDict(ResourceDictionary? dict, string key, Color color)
    {
        if (dict == null)
            return;

        if (dict.ContainsKey(key) && dict[key] is SolidColorBrush existingBrush)
        {
            existingBrush.Color = color;
        }
    }

    // ─── Static utilities ───

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
                case 6:
                    color = Color.FromArgb(
                        255,
                        Convert.ToByte(hex[0..2], 16),
                        Convert.ToByte(hex[2..4], 16),
                        Convert.ToByte(hex[4..6], 16));
                    return true;

                case 8:
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

    public static string ColorToHex(Color color)
    {
        return color.A == 255
            ? $"#{color.R:X2}{color.G:X2}{color.B:X2}"
            : $"#{color.A:X2}{color.R:X2}{color.G:X2}{color.B:X2}";
    }

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
