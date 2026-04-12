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

        // Create a separate writable dictionary for font overrides
        // (can't add local values to a Source-set ResourceDictionary)
        var fontDict = new ResourceDictionary();
        Application.Current.Resources.MergedDictionaries.Add(fontDict);

        if (previewDict != null)
        {
            App.ThemeEngine.RegisterDictionaries(previewDict, fontDict);
        }

        // When theme changes, force {ThemeResource} re-evaluation for fonts.
        // FontFamily is immutable so we can't update it in-place like brushes.
        // Toggling RequestedTheme forces all {ThemeResource} bindings to re-resolve,
        // picking up the font overrides from the fontDict (last in MergedDictionaries).
        App.ThemeEngine.ThemeChanged += (_, _) =>
        {
            ForceThemeResourceRefresh();
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
    /// Forces all {ThemeResource} bindings to re-resolve by briefly toggling
    /// the root element's RequestedTheme. This is required for font changes
    /// since FontFamily objects are immutable (unlike SolidColorBrush.Color).
    /// </summary>
    private void ForceThemeResourceRefresh()
    {
        if (App.CurrentWindow?.Content is FrameworkElement root)
        {
            var current = root.RequestedTheme;
            root.RequestedTheme = current == ElementTheme.Dark
                ? ElementTheme.Light
                : ElementTheme.Dark;
            root.RequestedTheme = current;
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
