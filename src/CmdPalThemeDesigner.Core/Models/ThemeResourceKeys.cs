// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.

namespace CmdPalThemeDesigner.Core.Models;

/// <summary>
/// Defines all known themeable resource keys for CmdPal and Dock.
/// </summary>
public static class ThemeResourceKeys
{
    // --- CmdPal: Background & Layers ---
    public const string LayerOnAcrylicPrimaryBackground = "LayerOnAcrylicPrimaryBackgroundBrush";
    public const string LayerOnAcrylicSecondaryBackground = "LayerOnAcrylicSecondaryBackgroundBrush";

    // --- CmdPal: Borders & Dividers ---
    public const string CommandBarBorder = "CmdPal.CommandBarBorderBrush";
    public const string TopBarBorder = "CmdPal.TopBarBorderBrush";
    public const string DividerStroke = "CmdPal.DividerStrokeColorDefaultBrush";

    // --- CmdPal: Search Box ---
    public const string SearchBoxBackground = "CmdPal.SearchBoxBackground";
    public const string SearchBoxForeground = "CmdPal.SearchBoxForeground";
    public const string SearchBoxPlaceholderForeground = "CmdPal.SearchBoxPlaceholderForeground";
    public const string SearchBoxBorderBrush = "CmdPal.SearchBoxBorderBrush";

    // --- CmdPal: List Items ---
    public const string ListItemBackground = "CmdPal.ListItemBackground";
    public const string ListItemBackgroundPointerOver = "CmdPal.ListItemBackgroundPointerOver";
    public const string ListItemBackgroundSelected = "CmdPal.ListItemBackgroundSelected";
    public const string ListItemForeground = "CmdPal.ListItemForeground";
    public const string ListItemForegroundSecondary = "CmdPal.ListItemForegroundSecondary";

    // --- CmdPal: Buttons ---
    public const string ButtonBackground = "CmdPal.ButtonBackground";
    public const string ButtonForeground = "CmdPal.ButtonForeground";
    public const string ButtonBackgroundPointerOver = "CmdPal.ButtonBackgroundPointerOver";
    public const string ButtonBorderBrush = "CmdPal.ButtonBorderBrush";

    // --- CmdPal: Accent ---
    public const string AccentColor = "CmdPal.AccentColor";

    // --- CmdPal: Tags ---
    public const string TagBackground = "CmdPal.TagBackground";
    public const string TagForeground = "CmdPal.TagForeground";
    public const string TagBorderBrush = "CmdPal.TagBorderBrush";

    // --- CmdPal: Context Menu ---
    public const string ContextMenuBackground = "CmdPal.ContextMenuBackground";
    public const string ContextMenuBorderBrush = "CmdPal.ContextMenuBorderBrush";
    public const string ContextMenuItemBackgroundPointerOver = "CmdPal.ContextMenuItemBackgroundPointerOver";
    public const string ContextMenuForeground = "CmdPal.ContextMenuForeground";

    // --- CmdPal: Fonts (applied as FontFamily/Size/Weight resources) ---
    public const string PrimaryFontFamily = "CmdPal.PrimaryFontFamily";
    public const string PrimaryFontSize = "CmdPal.PrimaryFontSize";
    public const string TitleFontFamily = "CmdPal.TitleFontFamily";
    public const string TitleFontSize = "CmdPal.TitleFontSize";
    public const string CaptionFontFamily = "CmdPal.CaptionFontFamily";
    public const string CaptionFontSize = "CmdPal.CaptionFontSize";
    public const string MonospaceFontFamily = "CmdPal.MonospaceFontFamily";
    public const string MonospaceFontSize = "CmdPal.MonospaceFontSize";

    // --- Dock: Items ---
    public const string DockItemBackground = "Dock.ItemBackground";
    public const string DockItemBackgroundPointerOver = "Dock.ItemBackgroundPointerOver";
    public const string DockItemBackgroundPressed = "Dock.ItemBackgroundPressed";
    public const string DockItemBorderBrush = "Dock.ItemBorderBrush";
    public const string DockItemBorderBrushPointerOver = "Dock.ItemBorderBrushPointerOver";
    public const string DockItemForeground = "Dock.ItemForeground";
    public const string DockItemForegroundSecondary = "Dock.ItemForegroundSecondary";

    /// <summary>
    /// Returns all resource keys grouped by category for use in the editor UI.
    /// </summary>
    public static IReadOnlyDictionary<string, string[]> GetGroupedKeys() => new Dictionary<string, string[]>
    {
        ["Background & Layers"] = [LayerOnAcrylicPrimaryBackground, LayerOnAcrylicSecondaryBackground],
        ["Borders & Dividers"] = [CommandBarBorder, TopBarBorder, DividerStroke],
        ["Search Box"] = [SearchBoxBackground, SearchBoxForeground, SearchBoxPlaceholderForeground, SearchBoxBorderBrush],
        ["List Items"] = [ListItemBackground, ListItemBackgroundPointerOver, ListItemBackgroundSelected, ListItemForeground, ListItemForegroundSecondary],
        ["Buttons"] = [ButtonBackground, ButtonForeground, ButtonBackgroundPointerOver, ButtonBorderBrush],
        ["Accent"] = [AccentColor],
        ["Tags"] = [TagBackground, TagForeground, TagBorderBrush],
        ["Context Menu"] = [ContextMenuBackground, ContextMenuBorderBrush, ContextMenuItemBackgroundPointerOver, ContextMenuForeground],
        ["Dock Items"] = [DockItemBackground, DockItemBackgroundPointerOver, DockItemBackgroundPressed, DockItemBorderBrush, DockItemBorderBrushPointerOver, DockItemForeground, DockItemForegroundSecondary],
    };

    /// <summary>
    /// Returns a flat list of all color resource keys (excludes font keys).
    /// </summary>
    public static string[] GetAllColorKeys() => [
        LayerOnAcrylicPrimaryBackground, LayerOnAcrylicSecondaryBackground,
        CommandBarBorder, TopBarBorder, DividerStroke,
        SearchBoxBackground, SearchBoxForeground, SearchBoxPlaceholderForeground, SearchBoxBorderBrush,
        ListItemBackground, ListItemBackgroundPointerOver, ListItemBackgroundSelected, ListItemForeground, ListItemForegroundSecondary,
        ButtonBackground, ButtonForeground, ButtonBackgroundPointerOver, ButtonBorderBrush,
        AccentColor,
        TagBackground, TagForeground, TagBorderBrush,
        ContextMenuBackground, ContextMenuBorderBrush, ContextMenuItemBackgroundPointerOver, ContextMenuForeground,
        DockItemBackground, DockItemBackgroundPointerOver, DockItemBackgroundPressed, DockItemBorderBrush, DockItemBorderBrushPointerOver, DockItemForeground, DockItemForegroundSecondary,
    ];
}
