// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

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

        // Create a separate writable dictionary for font style overrides
        var fontDict = new ResourceDictionary();
        Application.Current.Resources.MergedDictionaries.Add(fontDict);

        if (previewDict != null)
        {
            App.ThemeEngine.RegisterDictionaries(previewDict, fontDict);
        }

        // When theme changes, toggle RequestedTheme to force Style re-resolution
        App.ThemeEngine.ThemeChanged += (_, _) =>
        {
            ForceThemeRefresh();
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
    /// Forces WinUI to re-resolve {ThemeResource} bindings (including Style objects)
    /// by cycling RequestedTheme. Uses a nested helper panel to avoid visible flash.
    /// </summary>
    private void ForceThemeRefresh()
    {
        var previewPanel = PreviewScrollViewer;
        if (previewPanel == null) return;

        // Toggle to Light, then immediately back to Dark.
        // Even synchronously, WinUI re-resolves ThemeResource when the value changes.
        previewPanel.RequestedTheme = ElementTheme.Light;
        previewPanel.RequestedTheme = ElementTheme.Dark;
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
