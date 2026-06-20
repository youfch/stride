using System;
using System.Threading.Tasks;

namespace Stride.Core.Presentation.Interop;

/// <summary>
/// Shared services that need to be bridged between WPF and Avalonia.
/// </summary>
public interface IInteropServices
{
    /// <summary>
    /// Gets or sets the clipboard text.
    /// </summary>
    string ClipboardText { get; set; }

    /// <summary>
    /// Shows an open file dialog.
    /// </summary>
    /// <param name="title">Dialog title.</param>
    /// <param name="filter">File filter.</param>
    /// <param name="initialDirectory">Initial directory.</param>
    /// <returns>The selected file path, or null if cancelled.</returns>
    Task<string?> ShowOpenFileDialogAsync(
        string title,
        string filter,
        string? initialDirectory = null);

    /// <summary>
    /// Shows a save file dialog.
    /// </summary>
    /// <param name="title">Dialog title.</param>
    /// <param name="filter">File filter.</param>
    /// <param name="initialDirectory">Initial directory.</param>
    /// <returns>The selected file path, or null if cancelled.</returns>
    Task<string?> ShowSaveFileDialogAsync(
        string title,
        string filter,
        string? initialDirectory = null);

    /// <summary>
    /// Shows a folder browser dialog.
    /// </summary>
    /// <param name="title">Dialog title.</param>
    /// <param name="initialDirectory">Initial directory.</param>
    /// <returns>The selected folder path, or null if cancelled.</returns>
    Task<string?> ShowFolderBrowserDialogAsync(
        string title,
        string? initialDirectory = null);

    /// <summary>
    /// Shows a message box.
    /// </summary>
    /// <param name="message">Message text.</param>
    /// <param name="caption">Caption.</param>
    /// <param name="buttons">Button type.</param>
    /// <param name="icon">Icon type.</param>
    /// <returns>The user's choice.</returns>
    MessageBoxResult ShowMessageBox(
        string message,
        string? caption = null,
        MessageBoxButton buttons = MessageBoxButton.OK,
        MessageBoxImage icon = MessageBoxImage.None);
}

/// <summary>
/// Button types for message boxes.
/// </summary>
public enum MessageBoxButton
{
    OK,
    OKCancel,
    YesNo,
    YesNoCancel
}

/// <summary>
/// Result of a message box.
/// </summary>
public enum MessageBoxResult
{
    None,
    OK,
    Cancel,
    Yes,
    No
}

/// <summary>
/// Icon types for message boxes.
/// </summary>
public enum MessageBoxImage
{
    None,
    Information,
    Warning,
    Error,
    Question
}

/// <summary>
/// Default implementation of IInteropServices using the current UI framework.
/// </summary>
public class DefaultInteropServices : IInteropServices
{
    public string ClipboardText
    {
        get
        {
#if WPF
            return System.Windows.Clipboard.GetText();
#elif AVALONIA
            return Avalonia.Input.Clipboard.GetTextAsync().GetAwaiter().GetResult();
#else
            return string.Empty;
#endif
        }
        set
        {
#if WPF
            System.Windows.Clipboard.SetText(value);
#elif AVALONIA
            Avalonia.Input.Clipboard.SetTextAsync(value).GetAwaiter().GetResult();
#endif
        }
    }

    public async Task<string?> ShowOpenFileDialogAsync(
        string title,
        string filter,
        string? initialDirectory = null)
    {
        // TODO: Implement file dialog for both WPF and Avalonia
        return null;
    }

    public async Task<string?> ShowSaveFileDialogAsync(
        string title,
        string filter,
        string? initialDirectory = null)
    {
        // TODO: Implement file dialog for both WPF and Avalonia
        return null;
    }

    public async Task<string?> ShowFolderBrowserDialogAsync(
        string title,
        string? initialDirectory = null)
    {
        // TODO: Implement folder browser for both WPF and Avalonia
        return null;
    }

    public MessageBoxResult ShowMessageBox(
        string message,
        string? caption = null,
        MessageBoxButton buttons = MessageBoxButton.OK,
        MessageBoxImage icon = MessageBoxImage.None)
    {
        // TODO: Implement message box for both WPF and Avalonia
        return MessageBoxResult.OK;
    }
}
