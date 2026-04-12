// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.

using CmdPalThemeDesigner.Core.Models;

namespace CmdPalThemeDesigner.Core.Services;

/// <summary>
/// Simple undo/redo stack for ThemeDefinition snapshots.
/// </summary>
public sealed class ThemeUndoRedoService
{
    private readonly Stack<string> _undoStack = new();
    private readonly Stack<string> _redoStack = new();
    private readonly ThemeFileService _fileService = new();
    private string? _currentJson;

    /// <summary>
    /// Fires when undo/redo availability changes.
    /// </summary>
    public event EventHandler? StateChanged;

    public bool CanUndo => _undoStack.Count > 0;
    public bool CanRedo => _redoStack.Count > 0;

    /// <summary>
    /// Pushes the current state onto the undo stack before a change.
    /// </summary>
    public void PushState(ThemeDefinition theme)
    {
        if (_currentJson != null)
        {
            _undoStack.Push(_currentJson);

            // Limit stack size to prevent memory issues
            if (_undoStack.Count > 50)
            {
                var items = _undoStack.ToArray();
                _undoStack.Clear();
                for (int i = Math.Min(items.Length - 1, 49); i >= 0; i--)
                {
                    _undoStack.Push(items[i]);
                }
            }
        }

        _currentJson = _fileService.Serialize(theme);
        _redoStack.Clear();
        StateChanged?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Undoes the last change and returns the previous theme state.
    /// </summary>
    public ThemeDefinition? Undo()
    {
        if (!CanUndo)
            return null;

        if (_currentJson != null)
            _redoStack.Push(_currentJson);

        _currentJson = _undoStack.Pop();
        StateChanged?.Invoke(this, EventArgs.Empty);
        return _fileService.Deserialize(_currentJson);
    }

    /// <summary>
    /// Redoes the last undone change and returns the restored theme state.
    /// </summary>
    public ThemeDefinition? Redo()
    {
        if (!CanRedo)
            return null;

        if (_currentJson != null)
            _undoStack.Push(_currentJson);

        _currentJson = _redoStack.Pop();
        StateChanged?.Invoke(this, EventArgs.Empty);
        return _fileService.Deserialize(_currentJson);
    }

    /// <summary>
    /// Clears all undo/redo history.
    /// </summary>
    public void Clear()
    {
        _undoStack.Clear();
        _redoStack.Clear();
        _currentJson = null;
        StateChanged?.Invoke(this, EventArgs.Empty);
    }
}
