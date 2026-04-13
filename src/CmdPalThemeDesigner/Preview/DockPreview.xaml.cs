// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.

using CmdPalThemeDesigner.Core.Services;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;

namespace CmdPalThemeDesigner.Preview;

public sealed partial class DockPreview : UserControl
{
    public DockPreview()
    {
        InitializeComponent();
    }

    private void DockItem_PointerEntered(object sender, PointerRoutedEventArgs e)
    {
        if (sender is Border border)
        {
            border.Background = FindBrush("Dock.ItemBackgroundPointerOver");
            border.BorderBrush = FindBrush("Dock.ItemBorderBrushPointerOver");
        }
    }

    private void DockItem_PointerExited(object sender, PointerRoutedEventArgs e)
    {
        if (sender is Border border)
        {
            border.Background = FindBrush("Dock.ItemBackground");
            border.BorderBrush = FindBrush("Dock.ItemBorderBrush");
        }
    }

    private Brush? FindBrush(string key)
    {
        var engine = App.ThemeEngine;
        if (engine.Current != null)
        {
            // Expand palette and look up the XAML key
            var expanded = ThemeEngine.ExpandPalette(engine.Current.Palette.Dark);
            if (expanded.TryGetValue(key, out var hex) && ThemeEngine.TryParseColor(hex, out var color))
            {
                return new SolidColorBrush(color);
            }
        }

        return null;
    }
}
