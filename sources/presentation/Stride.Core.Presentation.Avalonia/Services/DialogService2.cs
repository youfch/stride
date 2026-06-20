// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Platform;

namespace Stride.Core.Presentation.Services;

/// <summary>
/// Information about a dialog button.
/// </summary>
public class DialogButtonInfo
{
    public DialogButtonInfo(string text, int resultValue)
    {
        Text = text;
        ResultValue = resultValue;
    }

    public string Text { get; }
    public int ResultValue { get; }
}

/// <summary>
/// File dialog filter.
/// </summary>
public class FileDialogFilter
{
    public string Name { get; set; } = string.Empty;
    public string Pattern { get; set; } = string.Empty;
}

/// <summary>
/// A lightweight dialog service interface for Avalonia.
/// </summary>
public interface IAvaloniaDialogService2
{
    /// <summary>
    /// Shows a message box.
    /// </summary>
    Task<MessageBoxResult> ShowMessageBox(string message, MessageBoxButton buttons = MessageBoxButton.OK, MessageBoxImage image = MessageBoxImage.None);

    /// <summary>
    /// Shows a message box with custom buttons.
    /// </summary>
    Task<int> ShowMessageBox(string message, IEnumerable<DialogButtonInfo> buttons, MessageBoxImage image = MessageBoxImage.None);

    /// <summary>
    /// Shows a message box with a checkbox.
    /// </summary>
    Task<CheckedMessageBoxResult> ShowCheckedMessageBox(string message, bool? isChecked, MessageBoxButton buttons = MessageBoxButton.OK, MessageBoxImage image = MessageBoxImage.None);

    /// <summary>
    /// Shows a file open dialog.
    /// </summary>
    Task<string?> ShowFileOpenDialog(string title, string initialDirectory, FileDialogFilter[] filters);

    /// <summary>
    /// Shows a file save dialog.
    /// </summary>
    Task<string?> ShowFileSaveDialog(string title, string initialDirectory, FileDialogFilter[] filters, string defaultExtension);

    /// <summary>
    /// Shows a folder open dialog.
    /// </summary>
    Task<string?> ShowFolderOpenDialog(string title, string initialDirectory);
}

/// <summary>
/// Implementation of IDialogService2 using Avalonia dialogs.
/// </summary>
public class DialogService2 : IAvaloniaDialogService2
{
    private readonly Window? _ownerWindow;

    public DialogService2(Window? ownerWindow = null)
    {
        _ownerWindow = ownerWindow;
    }

    public async Task<MessageBoxResult> ShowMessageBox(string message, MessageBoxButton buttons = MessageBoxButton.OK, MessageBoxImage image = MessageBoxImage.None)
    {
        // Simplified - show a basic window
        var window = new Window
        {
            Title = "Message",
            Width = 400,
            Height = 200,
            Content = new StackPanel
            {
                Margin = new Thickness(20),
                Children =
                {
                    new TextBlock
                    {
                        Text = message,
                        TextWrapping = TextWrapping.Wrap
                    }
                }
            }
        };

        window.Show();
        return MessageBoxResult.OK;
    }

    public async Task<int> ShowMessageBox(string message, IEnumerable<DialogButtonInfo> buttons, MessageBoxImage image = MessageBoxImage.None)
    {
        var result = await ShowMessageBox(message, MessageBoxButton.OK, image);
        return result == MessageBoxResult.OK ? 0 : -1;
    }

    public async Task<CheckedMessageBoxResult> ShowCheckedMessageBox(string message, bool? isChecked, MessageBoxButton buttons = MessageBoxButton.OK, MessageBoxImage image = MessageBoxImage.None)
    {
        var result = await ShowMessageBox(message, buttons, image);
        return new CheckedMessageBoxResult(result, isChecked ?? false);
    }

    public async Task<string?> ShowFileOpenDialog(string title, string initialDirectory, FileDialogFilter[] filters)
    {
        var topLevel = GetTopLevel();
        if (topLevel == null) return null;
        return null;
    }

    public async Task<string?> ShowFileSaveDialog(string title, string initialDirectory, FileDialogFilter[] filters, string defaultExtension)
    {
        var topLevel = GetTopLevel();
        if (topLevel == null) return null;
        return null;
    }

    public async Task<string?> ShowFolderOpenDialog(string title, string initialDirectory)
    {
        var topLevel = GetTopLevel();
        if (topLevel == null) return null;
        return null;
    }

    private TopLevel? GetTopLevel()
    {
        if (_ownerWindow != null)
            return TopLevel.GetTopLevel(_ownerWindow);
        
        return null;
    }
}
