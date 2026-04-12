// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.

using CmdPalThemeDesigner.Core.Models;
using CmdPalThemeDesigner.Core.Services;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;
using Windows.UI;

namespace CmdPalThemeDesigner.Editor;

public sealed partial class ThemeEditorPanel : UserControl
{
    private ThemeDefinition? _currentTheme;
    private bool _suppressEvents;

    /// <summary>
    /// Fires when the theme definition changes (color, font, or preset).
    /// </summary>
    public event EventHandler<ThemeDefinition>? ThemeChanged;

    public ThemeEditorPanel()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        // Populate font combos
        var fonts = App.FontService.GetFontFamilies();
        var weights = FontEnumerationService.GetFontWeights();

        PopulateFontCombo(PrimaryFontCombo, fonts);
        PopulateFontCombo(TitleFontCombo, fonts);
        PopulateFontCombo(CaptionFontCombo, fonts);
        PopulateFontCombo(MonoFontCombo, fonts);

        PopulateWeightCombo(PrimaryFontWeight, weights);
        PopulateWeightCombo(TitleFontWeight, weights);
        PopulateWeightCombo(CaptionFontWeight, weights);
        PopulateWeightCombo(MonoFontWeight, weights);

        // Build preset buttons programmatically
        BuildPresetButtons();

        // Set default backdrop
        BackdropStyleCombo.SelectedIndex = 0;
    }

    private void BuildPresetButtons()
    {
        PresetListPanel.Children.Clear();
        var presets = App.PresetProvider.Presets;

        foreach (var preset in presets)
        {
            var btn = new Button
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                HorizontalContentAlignment = Microsoft.UI.Xaml.HorizontalAlignment.Stretch,
                Padding = new Thickness(10, 8, 10, 8),
                CornerRadius = new CornerRadius(6),
                Tag = preset,
            };

            var stack = new StackPanel { Spacing = 2 };
            stack.Children.Add(new TextBlock
            {
                Text = preset.Name,
                FontSize = 12,
                FontWeight = Microsoft.UI.Text.FontWeights.SemiBold,
                TextTrimming = TextTrimming.CharacterEllipsis,
            });
            stack.Children.Add(new TextBlock
            {
                Text = preset.Description ?? string.Empty,
                FontSize = 10,
                Opacity = 0.6,
                TextTrimming = TextTrimming.CharacterEllipsis,
            });
            btn.Content = stack;

            btn.Click += PresetCard_Click;
            PresetListPanel.Children.Add(btn);
        }
    }

    /// <summary>
    /// Loads a theme definition into the editor, updating all controls.
    /// </summary>
    public void LoadTheme(ThemeDefinition theme)
    {
        _suppressEvents = true;
        _currentTheme = theme;

        // Update font controls
        SelectFontInCombo(PrimaryFontCombo, theme.Fonts.Primary.Family);
        PrimaryFontSize.Value = theme.Fonts.Primary.Size;
        SelectWeightInCombo(PrimaryFontWeight, theme.Fonts.Primary.Weight);

        SelectFontInCombo(TitleFontCombo, theme.Fonts.Title.Family);
        TitleFontSize.Value = theme.Fonts.Title.Size;
        SelectWeightInCombo(TitleFontWeight, theme.Fonts.Title.Weight);

        SelectFontInCombo(CaptionFontCombo, theme.Fonts.Caption.Family);
        CaptionFontSize.Value = theme.Fonts.Caption.Size;
        SelectWeightInCombo(CaptionFontWeight, theme.Fonts.Caption.Weight);

        SelectFontInCombo(MonoFontCombo, theme.Fonts.Monospace.Family);
        MonoFontSize.Value = theme.Fonts.Monospace.Size;
        SelectWeightInCombo(MonoFontWeight, theme.Fonts.Monospace.Weight);

        // Update backdrop
        for (int i = 0; i < BackdropStyleCombo.Items.Count; i++)
        {
            if (BackdropStyleCombo.Items[i] is ComboBoxItem item &&
                item.Tag?.ToString() == theme.Backdrop.Style)
            {
                BackdropStyleCombo.SelectedIndex = i;
                break;
            }
        }
        TintOpacitySlider.Value = theme.Backdrop.TintOpacity;
        LuminositySlider.Value = theme.Backdrop.LuminosityOpacity;

        // Build color editors
        BuildColorEditors();

        _suppressEvents = false;
    }

    private void BuildColorEditors()
    {
        ColorEditorsPanel.Children.Clear();
        if (_currentTheme == null)
            return;

        var grouped = ThemeResourceKeys.GetGroupedKeys();
        var darkColors = _currentTheme.Resources.Dark;

        foreach (var (group, keys) in grouped)
        {
            // Group header
            var header = new TextBlock
            {
                Text = group,
                FontSize = 12,
                FontWeight = Microsoft.UI.Text.FontWeights.SemiBold,
                Opacity = 0.7,
                Margin = new Thickness(0, 12, 0, 4),
            };
            ColorEditorsPanel.Children.Add(header);

            foreach (var key in keys)
            {
                var colorHex = darkColors.GetValueOrDefault(key, "#00000000");
                ThemeEngine.TryParseColor(colorHex, out var color);

                var row = CreateColorRow(key, color);
                ColorEditorsPanel.Children.Add(row);
            }
        }
    }

    private UIElement CreateColorRow(string key, Color color)
    {
        var grid = new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                new ColumnDefinition { Width = GridLength.Auto },
                new ColumnDefinition { Width = GridLength.Auto },
            },
            Margin = new Thickness(0, 2, 0, 2),
        };

        // Label (shortened key name)
        var shortName = key.Replace("CmdPal.", "").Replace("Dock.", "D:");
        var label = new TextBlock
        {
            Text = shortName,
            FontSize = 11,
            VerticalAlignment = VerticalAlignment.Center,
            TextTrimming = TextTrimming.CharacterEllipsis,
            Opacity = 0.8,
        };
        Grid.SetColumn(label, 0);
        grid.Children.Add(label);

        // Hex text input
        var hexInput = new TextBox
        {
            Text = ThemeEngine.ColorToHex(color),
            FontSize = 11,
            FontFamily = new FontFamily("Cascadia Code"),
            Width = 90,
            Padding = new Thickness(4, 2, 4, 2),
            Margin = new Thickness(4, 0, 4, 0),
            Tag = key,
        };
        hexInput.LostFocus += HexInput_LostFocus;
        Grid.SetColumn(hexInput, 1);
        grid.Children.Add(hexInput);

        // Color swatch (clickable)
        var swatch = new Border
        {
            Width = 24,
            Height = 24,
            CornerRadius = new CornerRadius(4),
            Background = new SolidColorBrush(color),
            BorderThickness = new Thickness(1),
            BorderBrush = new SolidColorBrush(Color.FromArgb(60, 128, 128, 128)),
            Tag = key,
        };

        // Add a ColorPicker in a flyout
        var colorPicker = new ColorPicker
        {
            Color = color,
            IsAlphaEnabled = true,
            IsAlphaSliderVisible = true,
            IsHexInputVisible = true,
            Tag = key,
        };
        colorPicker.ColorChanged += ColorPicker_ColorChanged;

        var flyout = new Flyout
        {
            Content = colorPicker,
        };
        swatch.Tapped += (s, e) => flyout.ShowAt(swatch);

        Grid.SetColumn(swatch, 2);
        grid.Children.Add(swatch);

        return grid;
    }

    private void ColorPicker_ColorChanged(ColorPicker sender, ColorChangedEventArgs args)
    {
        if (_suppressEvents || _currentTheme == null || sender.Tag is not string key)
            return;

        var hex = ThemeEngine.ColorToHex(args.NewColor);
        _currentTheme.Resources.Dark[key] = hex;
        _currentTheme.Resources.Light[key] = hex; // Sync for now

        NotifyThemeChanged();
    }

    private void HexInput_LostFocus(object sender, RoutedEventArgs e)
    {
        if (_suppressEvents || _currentTheme == null)
            return;

        if (sender is TextBox textBox && textBox.Tag is string key)
        {
            var hex = textBox.Text;
            if (ThemeEngine.TryParseColor(hex, out _))
            {
                _currentTheme.Resources.Dark[key] = hex;
                _currentTheme.Resources.Light[key] = hex;
                NotifyThemeChanged();
            }
        }
    }

    private void PresetCard_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is ThemeDefinition preset)
        {
            var clone = preset.Clone();
            LoadTheme(clone);
            NotifyThemeChanged();
        }
    }

    private void FontCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (_suppressEvents || _currentTheme == null)
            return;

        UpdateFontsFromControls();
        NotifyThemeChanged();
    }

    private void FontSize_ValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
    {
        if (_suppressEvents || _currentTheme == null)
            return;

        UpdateFontsFromControls();
        NotifyThemeChanged();
    }

    private void FontWeight_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (_suppressEvents || _currentTheme == null)
            return;

        UpdateFontsFromControls();
        NotifyThemeChanged();
    }

    private void BackdropStyle_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (_suppressEvents || _currentTheme == null)
            return;

        if (BackdropStyleCombo.SelectedItem is ComboBoxItem item)
        {
            _currentTheme.Backdrop.Style = item.Tag?.ToString() ?? "Acrylic";
        }
        NotifyThemeChanged();
    }

    private void BackdropSlider_ValueChanged(object sender, Microsoft.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
    {
        if (_suppressEvents || _currentTheme == null)
            return;

        _currentTheme.Backdrop.TintOpacity = TintOpacitySlider.Value;
        _currentTheme.Backdrop.LuminosityOpacity = LuminositySlider.Value;
        NotifyThemeChanged();
    }

    private void UpdateFontsFromControls()
    {
        if (_currentTheme == null)
            return;

        _currentTheme.Fonts.Primary.Family = GetSelectedFont(PrimaryFontCombo);
        _currentTheme.Fonts.Primary.Size = PrimaryFontSize.Value;
        _currentTheme.Fonts.Primary.Weight = GetSelectedWeight(PrimaryFontWeight);

        _currentTheme.Fonts.Title.Family = GetSelectedFont(TitleFontCombo);
        _currentTheme.Fonts.Title.Size = TitleFontSize.Value;
        _currentTheme.Fonts.Title.Weight = GetSelectedWeight(TitleFontWeight);

        _currentTheme.Fonts.Caption.Family = GetSelectedFont(CaptionFontCombo);
        _currentTheme.Fonts.Caption.Size = CaptionFontSize.Value;
        _currentTheme.Fonts.Caption.Weight = GetSelectedWeight(CaptionFontWeight);

        _currentTheme.Fonts.Monospace.Family = GetSelectedFont(MonoFontCombo);
        _currentTheme.Fonts.Monospace.Size = MonoFontSize.Value;
        _currentTheme.Fonts.Monospace.Weight = GetSelectedWeight(MonoFontWeight);
    }

    private void NotifyThemeChanged()
    {
        if (_currentTheme != null)
        {
            ThemeChanged?.Invoke(this, _currentTheme);
        }
    }

    private static void PopulateFontCombo(ComboBox combo, IReadOnlyList<string> fonts)
    {
        combo.Items.Clear();
        foreach (var font in fonts)
        {
            combo.Items.Add(font);
        }
    }

    private static void PopulateWeightCombo(ComboBox combo, IReadOnlyList<string> weights)
    {
        combo.Items.Clear();
        foreach (var w in weights)
        {
            combo.Items.Add(w);
        }
    }

    private static void SelectFontInCombo(ComboBox combo, string fontFamily)
    {
        for (int i = 0; i < combo.Items.Count; i++)
        {
            if (combo.Items[i] is string item &&
                item.Equals(fontFamily, StringComparison.OrdinalIgnoreCase))
            {
                combo.SelectedIndex = i;
                return;
            }
        }
        // If not found, add it and select
        combo.Items.Add(fontFamily);
        combo.SelectedIndex = combo.Items.Count - 1;
    }

    private static void SelectWeightInCombo(ComboBox combo, string weight)
    {
        for (int i = 0; i < combo.Items.Count; i++)
        {
            if (combo.Items[i] is string item &&
                item.Equals(weight, StringComparison.OrdinalIgnoreCase))
            {
                combo.SelectedIndex = i;
                return;
            }
        }
        combo.SelectedIndex = 4; // Default to "Normal"
    }

    private static string GetSelectedFont(ComboBox combo)
    {
        return combo.SelectedItem as string ?? "Segoe UI Variable";
    }

    private static string GetSelectedWeight(ComboBox combo)
    {
        return combo.SelectedItem as string ?? "Normal";
    }
}
