# CmdPal Theme Designer

A standalone WinUI 3 app for designing and previewing themes for [PowerToys](https://github.com/microsoft/PowerToys) **Command Palette** and **Dock**.

![WinUI 3](https://img.shields.io/badge/WinUI_3-blue) ![.NET](https://img.shields.io/badge/.NET_10-purple) ![License](https://img.shields.io/badge/License-MIT-green)

## Features

- **Live Preview** — See Command Palette and Dock controls update in real-time as you edit colors and fonts
- **14 Built-in Presets** — Dracula, Tokyo Night, Nord, Catppuccin Mocha, Rosé Pine, One Dark, Gruvbox, GitHub Dark, Solarized Dark, Monokai, Synthwave, Everforest, and more
- **Color Editor** — Click any color swatch to customize individual theme resources
- **Font Editor** — Change font family, size, and weight for primary, monospace, title, and caption text
- **Backdrop Selection** — Switch between Mica, Mica Alt, and Acrylic backdrops
- **JSON Export/Import** — Save themes as `.json` files to share with others
- **Grouped Resources** — Colors organized by category: Search Box, List Items, Dock, Tags, and General

## Architecture

```
CmdPalThemeDesigner/
├── src/
│   ├── CmdPalThemeDesigner/          # WinUI 3 app (UI layer)
│   │   ├── Pages/                    # Main ThemeDesignerPage
│   │   ├── Preview/                  # CmdPal & Dock preview controls
│   │   ├── Editor/                   # Theme editor panel (colors, fonts, presets)
│   │   └── Styles/                   # PreviewStyles.xaml (theme resources)
│   └── CmdPalThemeDesigner.Core/     # Core library (no UI dependencies)
│       ├── Models/                   # ThemeDefinition, ThemeResourceKeys
│       ├── Services/                 # ThemeEngine, PresetThemeProvider, ThemeExporter
│       └── Presets/                  # 14 embedded JSON preset files
└── CmdPalThemeDesigner.sln
```

## Theme Resource Keys

The designer supports 33 color resources and 8 font resources that map to CmdPal and Dock UI elements:

| Category | Keys |
|----------|------|
| **Search Box** | Background, Border, Text, PlaceholderText, Glyph |
| **List Items** | Background, BackgroundPointerOver, BackgroundSelected, Text, TextSecondary, Separator |
| **Dock** | Background, ItemBackground, ItemBackgroundPointerOver, ItemBackgroundSelected, ItemText, Separator |
| **Tags** | Background, Text, Border |
| **General** | Background, AccentFill, AccentText, TextPrimary, TextSecondary, TextDisabled, Border, Overlay, Shadow, ScrollBarThumb |
| **Fonts** | Primary, Monospace, Title, Caption (family, size, weight) |

## Building

### Prerequisites

- Visual Studio 2022 17.x+ (or VS 18 Preview) with:
  - .NET Desktop Development workload
  - Windows App SDK
- .NET 10 SDK
- Windows 10 SDK (10.0.22621.0)

### Build

```powershell
# From the repo root
msbuild CmdPalThemeDesigner.sln /p:Configuration=Debug /p:Platform=x64
```

Or open `CmdPalThemeDesigner.sln` in Visual Studio and press F5.

## Theme JSON Format

Themes are stored as JSON files with this structure:

```json
{
  "name": "My Theme",
  "author": "Your Name",
  "version": "1.0",
  "description": "A cool theme",
  "backdrop": "MicaAlt",
  "fonts": {
    "primary": { "family": "Segoe UI Variable", "size": 14, "weight": "Normal" },
    "monospace": { "family": "Cascadia Code", "size": 13, "weight": "Normal" },
    "title": { "family": "Segoe UI Variable", "size": 16, "weight": "SemiBold" },
    "caption": { "family": "Segoe UI Variable", "size": 12, "weight": "Normal" }
  },
  "resources": {
    "dark": {
      "CmdPal.SearchBoxBackground": "#2D2D30",
      "CmdPal.ListItemText": "#CCCCCC"
    }
  }
}
```

## Contributing

Contributions are welcome! This project is a design tool for PowerToys Command Palette themes. Feel free to:

- Add new preset themes
- Improve the preview controls to better match CmdPal/Dock
- Add new theme resource keys
- Fix bugs or improve the UI

## License

[MIT](LICENSE)
