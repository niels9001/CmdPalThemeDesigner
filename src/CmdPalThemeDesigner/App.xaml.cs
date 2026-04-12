// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.

using CmdPalThemeDesigner.Core.Services;
using Microsoft.UI.Xaml;

namespace CmdPalThemeDesigner;

public partial class App : Application
{
    public static ThemeEngine ThemeEngine { get; } = new();
    public static PresetThemeProvider PresetProvider { get; } = new();
    public static FontEnumerationService FontService { get; } = new();
    public static ThemeUndoRedoService UndoRedo { get; } = new();

    private Window? _window;

    public App()
    {
        InitializeComponent();
    }

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        _window = new MainWindow();
        _window.Activate();
    }

    public static Window? CurrentWindow => ((App)Current)._window;
}
