// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.

using CmdPalThemeDesigner.Core.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;

namespace CmdPalThemeDesigner.Pages;

public sealed partial class ThemeDesignerPage : Page
{
    public ThemeDesignerPage()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        // Find the PreviewStyles.xaml dictionary for in-place brush updates
        var previewDict = FindPreviewStylesDictionary();

        // Create a separate writable dictionary for font overrides (unused for fonts now,
        // but kept for future non-ThemeDictionary resources)
        var fontDict = new ResourceDictionary();
        Application.Current.Resources.MergedDictionaries.Add(fontDict);

        if (previewDict != null)
        {
            App.ThemeEngine.RegisterDictionaries(previewDict, fontDict);
        }

        // When theme changes, directly update fonts on preview controls
        App.ThemeEngine.ThemeChanged += (_, _) =>
        {
            ApplyFontsToPreview();
        };

        // Wire editor changes to theme engine with undo support
        EditorPanel.ThemeChanged += (_, theme) =>
        {
            App.UndoRedo.PushState(theme);
            App.ThemeEngine.Current = theme;
        };

        // Apply first preset by default
        var presets = App.PresetProvider.Presets;
        if (presets.Count > 0)
        {
            var defaultTheme = presets[0].Clone();
            EditorPanel.LoadTheme(defaultTheme);
            App.ThemeEngine.Current = defaultTheme;
            App.UndoRedo.PushState(defaultTheme);
        }
    }

    /// <summary>
    /// Directly applies font families to all preview TextBlocks by walking the visual tree.
    /// This is necessary because FontFamily is immutable — {ThemeResource} can't update it in-place.
    /// </summary>
    private void ApplyFontsToPreview()
    {
        var theme = App.ThemeEngine.Current;
        if (theme == null) return;

        var primaryFont = new FontFamily(theme.Fonts.Primary.Family ?? "Segoe UI Variable");
        var titleFont = new FontFamily(theme.Fonts.Title.Family ?? "Segoe UI Variable");
        var captionFont = new FontFamily(theme.Fonts.Caption.Family ?? "Segoe UI Variable");
        var monoFont = new FontFamily(theme.Fonts.Monospace.Family ?? "Cascadia Code");

        var primarySize = theme.Fonts.Primary.Size;
        var titleSize = theme.Fonts.Title.Size;
        var captionSize = theme.Fonts.Caption.Size;
        var monoSize = theme.Fonts.Monospace.Size;

        // Walk each preview control + the font preview section
        ApplyFontsToTree(CmdPalPreviewControl, primaryFont, titleFont, captionFont, monoFont,
                         primarySize, titleSize, captionSize, monoSize);
        ApplyFontsToTree(ContextMenuPreviewControl, primaryFont, titleFont, captionFont, monoFont,
                         primarySize, titleSize, captionSize, monoSize);
        ApplyFontsToTree(DockPreviewControl, primaryFont, titleFont, captionFont, monoFont,
                         primarySize, titleSize, captionSize, monoSize);
        // Also update the font preview section on this page
        ApplyFontsToTree(FontPreviewPanel, primaryFont, titleFont, captionFont, monoFont,
                         primarySize, titleSize, captionSize, monoSize);
    }

    private static void ApplyFontsToTree(DependencyObject root,
        FontFamily primaryFont, FontFamily titleFont, FontFamily captionFont, FontFamily monoFont,
        double primarySize, double titleSize, double captionSize, double monoSize)
    {
        if (root == null) return;

        var count = VisualTreeHelper.GetChildrenCount(root);
        for (int i = 0; i < count; i++)
        {
            var child = VisualTreeHelper.GetChild(root, i);

            if (child is TextBlock tb)
            {
                var tag = tb.Tag as string;
                if (tag == "title")
                {
                    tb.FontFamily = titleFont;
                    tb.FontSize = titleSize;
                }
                else if (tag == "mono")
                {
                    tb.FontFamily = monoFont;
                    tb.FontSize = monoSize;
                }
                else if (tag == "caption")
                {
                    tb.FontFamily = captionFont;
                    tb.FontSize = captionSize;
                }
                else if (tag == "primary")
                {
                    tb.FontFamily = primaryFont;
                    tb.FontSize = primarySize;
                }
            }
            else if (child is TextBox txBox)
            {
                var tag = txBox.Tag as string;
                if (tag == "primary")
                {
                    txBox.FontFamily = primaryFont;
                    txBox.FontSize = primarySize;
                }
                else if (tag == "mono")
                {
                    txBox.FontFamily = monoFont;
                    txBox.FontSize = monoSize;
                }
            }

            // Recurse into children
            ApplyFontsToTree(child, primaryFont, titleFont, captionFont, monoFont,
                            primarySize, titleSize, captionSize, monoSize);
        }
    }

    /// <summary>
    /// Finds the PreviewStyles ResourceDictionary that's already in MergedDictionaries.
    /// </summary>
    private static ResourceDictionary? FindPreviewStylesDictionary()
    {
        foreach (var dict in Application.Current.Resources.MergedDictionaries)
        {
            if (dict.ThemeDictionaries.ContainsKey("Default"))
            {
                if (dict.ThemeDictionaries["Default"] is ResourceDictionary defaultDict &&
                    defaultDict.ContainsKey("CmdPal.SearchBoxBackground"))
                {
                    return dict;
                }
            }
        }

        return null;
    }
}
