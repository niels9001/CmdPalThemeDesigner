// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.

using CmdPalThemeDesigner.Core.Services;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.Storage.Pickers;

namespace CmdPalThemeDesigner.Pages;

public sealed partial class ExportImportPage : Page
{
    private readonly ThemeFileService _fileService = new();

    public ExportImportPage()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        UpdateThemeInfo();
    }

    private void UpdateThemeInfo()
    {
        var theme = App.ThemeEngine.Current;
        if (theme != null)
        {
            ThemeNameText.Text = theme.Name;
            ThemeAuthorText.Text = $"by {theme.Author ?? "Unknown"}";
            ThemeDescText.Text = theme.Description ?? "";
            JsonPreviewText.Text = _fileService.Serialize(theme);
        }
        else
        {
            ThemeNameText.Text = "No theme loaded";
            ThemeAuthorText.Text = "";
            ThemeDescText.Text = "";
            JsonPreviewText.Text = "";
        }
    }

    private async void ExportButton_Click(object sender, RoutedEventArgs e)
    {
        var theme = App.ThemeEngine.Current;
        if (theme == null)
            return;

        var picker = new FileSavePicker();
        picker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
        picker.FileTypeChoices.Add("JSON Theme", [".json"]);
        picker.SuggestedFileName = theme.Name.Replace(" ", "-").ToLowerInvariant();

        // Initialize the picker with the window handle
        var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(App.CurrentWindow);
        WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);

        var file = await picker.PickSaveFileAsync();
        if (file != null)
        {
            var json = _fileService.Serialize(theme);
            await Windows.Storage.FileIO.WriteTextAsync(file, json);

            ExportInfoBar.IsOpen = true;
            ExportInfoBar.Severity = InfoBarSeverity.Success;
            ExportInfoBar.Title = $"Theme exported to {file.Name}";
        }
    }

    private async void ImportButton_Click(object sender, RoutedEventArgs e)
    {
        var picker = new FileOpenPicker();
        picker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
        picker.FileTypeFilter.Add(".json");

        var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(App.CurrentWindow);
        WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);

        var file = await picker.PickSingleFileAsync();
        if (file != null)
        {
            try
            {
                var json = await Windows.Storage.FileIO.ReadTextAsync(file);

                // Validate
                var errors = _fileService.Validate(json);
                if (errors.Count > 0)
                {
                    ImportInfoBar.IsOpen = true;
                    ImportInfoBar.Severity = InfoBarSeverity.Error;
                    ImportInfoBar.Title = "Invalid theme file";
                    ImportInfoBar.Message = string.Join("\n", errors);
                    return;
                }

                var theme = _fileService.Deserialize(json);
                App.ThemeEngine.Current = theme;

                ImportInfoBar.IsOpen = true;
                ImportInfoBar.Severity = InfoBarSeverity.Success;
                ImportInfoBar.Title = $"Imported: {theme.Name}";
                ImportInfoBar.Message = "";

                UpdateThemeInfo();
            }
            catch (Exception ex)
            {
                ImportInfoBar.IsOpen = true;
                ImportInfoBar.Severity = InfoBarSeverity.Error;
                ImportInfoBar.Title = "Failed to import theme";
                ImportInfoBar.Message = ex.Message;
            }
        }
    }
}
