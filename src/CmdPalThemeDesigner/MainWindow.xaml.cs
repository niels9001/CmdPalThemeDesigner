// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.

using CmdPalThemeDesigner.Pages;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace CmdPalThemeDesigner;

public sealed partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        ExtendsContentIntoTitleBar = true;
        SetTitleBar(AppTitleBar);

        // Set minimum window size
        var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
        var windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hwnd);
        var appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId);
        appWindow.Resize(new Windows.Graphics.SizeInt32(1280, 800));

        // Wire undo/redo state
        App.UndoRedo.StateChanged += (_, _) =>
        {
            UndoButton.IsEnabled = App.UndoRedo.CanUndo;
            RedoButton.IsEnabled = App.UndoRedo.CanRedo;
        };

        // Select the first nav item
        NavView.SelectedItem = NavView.MenuItems[0];
    }

    private void NavView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
    {
        if (args.SelectedItemContainer is NavigationViewItem item)
        {
            var tag = item.Tag?.ToString();
            switch (tag)
            {
                case "designer":
                    ContentFrame.Navigate(typeof(ThemeDesignerPage));
                    break;
                case "exportimport":
                    ContentFrame.Navigate(typeof(ExportImportPage));
                    break;
            }
        }
    }

    private void ThemeToggle_Toggled(object sender, RoutedEventArgs e)
    {
        if (Content is FrameworkElement root)
        {
            root.RequestedTheme = ThemeToggle.IsOn ? ElementTheme.Dark : ElementTheme.Light;
        }

        App.ThemeEngine.Current = App.ThemeEngine.Current; // Re-apply theme for variant
    }

    private void UndoButton_Click(object sender, RoutedEventArgs e)
    {
        var theme = App.UndoRedo.Undo();
        if (theme != null)
        {
            App.ThemeEngine.Current = theme;
        }
    }

    private void RedoButton_Click(object sender, RoutedEventArgs e)
    {
        var theme = App.UndoRedo.Redo();
        if (theme != null)
        {
            App.ThemeEngine.Current = theme;
        }
    }
}
