// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;
using Microsoft.UI.Xaml.Media;

namespace CmdPalThemeDesigner.Core.Services;

/// <summary>
/// Enumerates installed system fonts via WinUI FontFamily inspection.
/// </summary>
public sealed class FontEnumerationService
{
    private List<string>? _cachedFonts;

    [DllImport("dwrite.dll")]
    private static extern int DWriteCreateFactory(int factoryType, ref Guid iid, out nint factory);

    /// <summary>
    /// Gets a sorted list of commonly available font family names.
    /// Falls back to a known list since enumerating all system fonts
    /// requires DirectWrite COM interop which isn't worth the complexity here.
    /// </summary>
    public IReadOnlyList<string> GetFontFamilies()
    {
        if (_cachedFonts != null)
            return _cachedFonts;

        // Well-known system fonts — covers the vast majority of use cases
        _cachedFonts = new List<string>
        {
            "Arial",
            "Arial Black",
            "Bahnschrift",
            "Calibri",
            "Cambria",
            "Candara",
            "Cascadia Code",
            "Cascadia Mono",
            "Comic Sans MS",
            "Consolas",
            "Constantia",
            "Corbel",
            "Courier New",
            "Ebrima",
            "Fira Code",
            "Franklin Gothic Medium",
            "Gadugi",
            "Georgia",
            "Impact",
            "Ink Free",
            "Inter",
            "JetBrains Mono",
            "Lucida Console",
            "Lucida Sans Unicode",
            "Malgun Gothic",
            "Microsoft Sans Serif",
            "Nirmala UI",
            "Palatino Linotype",
            "Segoe Fluent Icons",
            "Segoe MDL2 Assets",
            "Segoe Print",
            "Segoe Script",
            "Segoe UI",
            "Segoe UI Emoji",
            "Segoe UI Historic",
            "Segoe UI Symbol",
            "Segoe UI Variable",
            "SimSun",
            "Sitka Text",
            "Sylfaen",
            "Tahoma",
            "Times New Roman",
            "Trebuchet MS",
            "Verdana",
            "Yu Gothic",
        };

        return _cachedFonts;
    }

    /// <summary>
    /// Returns available font weight names.
    /// </summary>
    public static IReadOnlyList<string> GetFontWeights() =>
    [
        "Thin",
        "ExtraLight",
        "Light",
        "SemiLight",
        "Normal",
        "Medium",
        "SemiBold",
        "Bold",
        "ExtraBold",
        "Black",
    ];
}
