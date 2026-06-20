using System;
using System.Threading.Tasks;

namespace Stride.Core.Presentation.Interop;

/// <summary>
/// Interface for hosting cross-platform UI elements.
/// Implementations provide platform-specific hosting logic for embedding
/// one UI framework's content into another.
/// </summary>
public interface IUiHost
{
    /// <summary>
    /// Gets the handle of the hosted content.
    /// </summary>
    IntPtr ContentHandle { get; }

    /// <summary>
    /// Gets the size of the hosted content in device-independent pixels.
    /// </summary>
    Size ContentSize { get; }

    /// <summary>
    /// Sets the size of the hosted content.
    /// </summary>
    /// <param name="width">Width in device-independent pixels.</param>
    /// <param name="height">Height in device-independent pixels.</param>
    void SetContentSize(float width, float height);

    /// <summary>
    /// Shows the hosted content.
    /// </summary>
    void Show();

    /// <summary>
    /// Hides the hosted content.
    /// </summary>
    void Hide();

    /// <summary>
    /// Disposes the hosted content and releases resources.
    /// </summary>
    void Dispose();
}

/// <summary>
/// Size structure for UI elements (device-independent pixels).
/// </summary>
public readonly struct Size
{
    public readonly float Width;
    public readonly float Height;

    public Size(float width, float height)
    {
        Width = width;
        Height = height;
    }

    public static Size Zero => new(0, 0);
}

/// <summary>
/// Factory for creating UI hosts based on the current platform.
/// </summary>
public static class UiHostFactory
{
    /// <summary>
    /// Creates a new UI host for the specified content type.
    /// </summary>
    /// <param name="contentType">Type of content to host ("wpf" or "avalonia").</param>
    /// <returns>A new UI host instance.</returns>
    public static IUiHost CreateHost(string contentType)
    {
        return contentType.ToLowerInvariant() switch
        {
            "wpf" => new WpfHost(),
            "avalonia" => new AvaloniaHost(),
            _ => throw new ArgumentException($"Unknown content type: {contentType}", nameof(contentType))
        };
    }
}
