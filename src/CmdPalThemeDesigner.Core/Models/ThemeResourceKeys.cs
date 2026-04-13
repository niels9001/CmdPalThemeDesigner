// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.

namespace CmdPalThemeDesigner.Core.Models;

/// <summary>
/// Defines the semantic palette tokens that users edit, and the derivation
/// mappings that expand them into full XAML resource keys.
/// </summary>
public static class ThemeResourceKeys
{
    // ─── Palette tokens (user-editable, stored in JSON) ───

    public const string ForegroundPrimary = "foreground.primary";
    public const string ForegroundSecondary = "foreground.secondary";
    public const string ForegroundTertiary = "foreground.tertiary";
    public const string ForegroundDisabled = "foreground.disabled";
    public const string BackgroundPrimary = "background.primary";
    public const string BackgroundSecondary = "background.secondary";
    public const string BackgroundTertiary = "background.tertiary";
    public const string Accent = "accent";
    public const string AccentText = "accent.text";
    public const string BorderDefault = "border.default";
    public const string BorderCard = "border.card";

    // ─── XAML resource keys (what preview controls consume) ───

    // These are the actual keys in PreviewStyles.xaml / CmdPal UI
    public const string Xaml_LayerPrimary = "LayerOnAcrylicPrimaryBackgroundBrush";
    public const string Xaml_LayerSecondary = "LayerOnAcrylicSecondaryBackgroundBrush";
    public const string Xaml_CommandBarBorder = "CmdPal.CommandBarBorderBrush";
    public const string Xaml_TopBarBorder = "CmdPal.TopBarBorderBrush";
    public const string Xaml_DividerStroke = "CmdPal.DividerStrokeColorDefaultBrush";
    public const string Xaml_SearchBoxBg = "CmdPal.SearchBoxBackground";
    public const string Xaml_SearchBoxFg = "CmdPal.SearchBoxForeground";
    public const string Xaml_SearchBoxPlaceholder = "CmdPal.SearchBoxPlaceholderForeground";
    public const string Xaml_SearchBoxBorder = "CmdPal.SearchBoxBorderBrush";
    public const string Xaml_ListItemBg = "CmdPal.ListItemBackground";
    public const string Xaml_ListItemBgHover = "CmdPal.ListItemBackgroundPointerOver";
    public const string Xaml_ListItemBgSelected = "CmdPal.ListItemBackgroundSelected";
    public const string Xaml_ListItemFg = "CmdPal.ListItemForeground";
    public const string Xaml_ListItemFgSecondary = "CmdPal.ListItemForegroundSecondary";
    public const string Xaml_ButtonBg = "CmdPal.ButtonBackground";
    public const string Xaml_ButtonFg = "CmdPal.ButtonForeground";
    public const string Xaml_ButtonBgHover = "CmdPal.ButtonBackgroundPointerOver";
    public const string Xaml_ButtonBorder = "CmdPal.ButtonBorderBrush";
    public const string Xaml_Accent = "CmdPal.AccentColor";
    public const string Xaml_TagBg = "CmdPal.TagBackground";
    public const string Xaml_TagFg = "CmdPal.TagForeground";
    public const string Xaml_TagBorder = "CmdPal.TagBorderBrush";
    public const string Xaml_ContextMenuBg = "CmdPal.ContextMenuBackground";
    public const string Xaml_ContextMenuBorder = "CmdPal.ContextMenuBorderBrush";
    public const string Xaml_ContextMenuItemHover = "CmdPal.ContextMenuItemBackgroundPointerOver";
    public const string Xaml_ContextMenuFg = "CmdPal.ContextMenuForeground";
    public const string Xaml_DockItemBg = "Dock.ItemBackground";
    public const string Xaml_DockItemBgHover = "Dock.ItemBackgroundPointerOver";
    public const string Xaml_DockItemBgPressed = "Dock.ItemBackgroundPressed";
    public const string Xaml_DockItemBorder = "Dock.ItemBorderBrush";
    public const string Xaml_DockItemBorderHover = "Dock.ItemBorderBrushPointerOver";
    public const string Xaml_DockItemFg = "Dock.ItemForeground";
    public const string Xaml_DockItemFgSecondary = "Dock.ItemForegroundSecondary";

    // ─── Font resource keys ───

    public const string PrimaryFontFamily = "CmdPal.PrimaryFontFamily";
    public const string PrimaryFontSize = "CmdPal.PrimaryFontSize";
    public const string TitleFontFamily = "CmdPal.TitleFontFamily";
    public const string TitleFontSize = "CmdPal.TitleFontSize";
    public const string CaptionFontFamily = "CmdPal.CaptionFontFamily";
    public const string CaptionFontSize = "CmdPal.CaptionFontSize";
    public const string MonospaceFontFamily = "CmdPal.MonospaceFontFamily";
    public const string MonospaceFontSize = "CmdPal.MonospaceFontSize";

    /// <summary>
    /// All palette tokens, ordered for display in the editor.
    /// </summary>
    public static string[] GetAllPaletteTokens() =>
    [
        ForegroundPrimary, ForegroundSecondary, ForegroundTertiary, ForegroundDisabled,
        BackgroundPrimary, BackgroundSecondary, BackgroundTertiary,
        Accent, AccentText,
        BorderDefault, BorderCard,
    ];

    /// <summary>
    /// Palette tokens grouped by category for the editor UI.
    /// </summary>
    public static IReadOnlyDictionary<string, string[]> GetGroupedPaletteTokens() => new Dictionary<string, string[]>
    {
        ["Foreground"] = [ForegroundPrimary, ForegroundSecondary, ForegroundTertiary, ForegroundDisabled],
        ["Background"] = [BackgroundPrimary, BackgroundSecondary, BackgroundTertiary],
        ["Accent"] = [Accent, AccentText],
        ["Borders"] = [BorderDefault, BorderCard],
    };

    /// <summary>
    /// Friendly display names for palette tokens.
    /// </summary>
    public static IReadOnlyDictionary<string, string> GetDisplayNames() => new Dictionary<string, string>
    {
        [ForegroundPrimary] = "Primary Text",
        [ForegroundSecondary] = "Secondary Text",
        [ForegroundTertiary] = "Tertiary Text",
        [ForegroundDisabled] = "Disabled Text",
        [BackgroundPrimary] = "Primary Background",
        [BackgroundSecondary] = "Secondary Background",
        [BackgroundTertiary] = "Tertiary / Controls",
        [Accent] = "Accent",
        [AccentText] = "Accent Text",
        [BorderDefault] = "Dividers & Borders",
        [BorderCard] = "Card Borders",
    };

    /// <summary>
    /// Maps each palette token → the XAML resource keys it controls.
    /// The ThemeEngine uses this to expand ~11 tokens into ~33 XAML resources.
    /// </summary>
    public static IReadOnlyDictionary<string, string[]> GetDerivedMappings() => new Dictionary<string, string[]>
    {
        [ForegroundPrimary] = [
            Xaml_SearchBoxFg, Xaml_ListItemFg, Xaml_ButtonFg,
            Xaml_ContextMenuFg, Xaml_DockItemFg,
        ],
        [ForegroundSecondary] = [
            Xaml_ListItemFgSecondary, Xaml_DockItemFgSecondary,
        ],
        [ForegroundTertiary] = [
            Xaml_SearchBoxPlaceholder, Xaml_TagFg,
        ],
        [BackgroundPrimary] = [
            Xaml_LayerPrimary,
        ],
        [BackgroundSecondary] = [
            Xaml_LayerSecondary, Xaml_ContextMenuBg,
        ],
        [BackgroundTertiary] = [
            Xaml_SearchBoxBg, Xaml_ButtonBg, Xaml_TagBg,
        ],
        [Accent] = [
            Xaml_Accent,
        ],
        [BorderDefault] = [
            Xaml_CommandBarBorder, Xaml_TopBarBorder, Xaml_DividerStroke,
            Xaml_SearchBoxBorder, Xaml_ButtonBorder,
        ],
        [BorderCard] = [
            Xaml_TagBorder, Xaml_ContextMenuBorder,
            Xaml_DockItemBorder,
        ],
    };

    /// <summary>
    /// Returns all XAML resource keys that are present in PreviewStyles.xaml.
    /// </summary>
    public static string[] GetAllXamlKeys() =>
    [
        Xaml_LayerPrimary, Xaml_LayerSecondary,
        Xaml_CommandBarBorder, Xaml_TopBarBorder, Xaml_DividerStroke,
        Xaml_SearchBoxBg, Xaml_SearchBoxFg, Xaml_SearchBoxPlaceholder, Xaml_SearchBoxBorder,
        Xaml_ListItemBg, Xaml_ListItemBgHover, Xaml_ListItemBgSelected, Xaml_ListItemFg, Xaml_ListItemFgSecondary,
        Xaml_ButtonBg, Xaml_ButtonFg, Xaml_ButtonBgHover, Xaml_ButtonBorder,
        Xaml_Accent,
        Xaml_TagBg, Xaml_TagFg, Xaml_TagBorder,
        Xaml_ContextMenuBg, Xaml_ContextMenuBorder, Xaml_ContextMenuItemHover, Xaml_ContextMenuFg,
        Xaml_DockItemBg, Xaml_DockItemBgHover, Xaml_DockItemBgPressed,
        Xaml_DockItemBorder, Xaml_DockItemBorderHover, Xaml_DockItemFg, Xaml_DockItemFgSecondary,
    ];
}
