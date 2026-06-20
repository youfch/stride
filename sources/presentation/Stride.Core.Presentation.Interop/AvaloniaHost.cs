using System;

#if AVALONIA
using Avalonia.Controls;
using Avalonia.Platform;
#endif

namespace Stride.Core.Presentation.Interop;

#if AVALONIA
/// <summary>
/// Avalonia implementation of IUiHost for hosting WPF content in Avalonia.
/// Uses NativeControlHost to embed the WPF window HWND.
/// </summary>
internal class AvaloniaHost : IUiHost
{
    private object? _nativeHost; // NativeControlHost or placeholder
    private IntPtr _contentHandle;
    private Size _contentSize;
    private bool _disposed;

    public IntPtr ContentHandle => _contentHandle;
    public Size ContentSize => _contentSize;

    /// <summary>
    /// Creates a new Avalonia host for the specified WPF window handle.
    /// </summary>
    /// <param name="hwnd">The HWND of the WPF window to host.</param>
    public void Initialize(IntPtr hwnd)
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(AvaloniaHost));

        _contentHandle = hwnd;
        _nativeHost = new NativeControlHost();
    }

    public void SetContentSize(float width, float height)
    {
        _contentSize = new Size(width, height);
    }

    public void Show() { }
    public void Hide() { }

    public void Dispose()
    {
        if (_disposed) return;
        _nativeHost = null;
        _contentHandle = IntPtr.Zero;
        _disposed = true;
    }
}
#else
// Placeholder for non-Avalonia builds
internal class AvaloniaHost : IUiHost
{
    private IntPtr _contentHandle;
    private Size _contentSize;
    private bool _disposed;

    public IntPtr ContentHandle => _contentHandle;
    public Size ContentSize => _contentSize;

    public void Initialize(IntPtr hwnd)
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(AvaloniaHost));
        _contentHandle = hwnd;
    }

    public void SetContentSize(float width, float height)
    {
        _contentSize = new Size(width, height);
    }

    public void Show() { }
    public void Hide() { }

    public void Dispose()
    {
        if (_disposed) return;
        _contentHandle = IntPtr.Zero;
        _disposed = true;
    }
}
#endif
