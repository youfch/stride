using System;

#if WPF
using System.Windows;
using System.Windows.Interop;
using System.Windows.Controls;
#endif

namespace Stride.Core.Presentation.Interop;

#if WPF
/// <summary>
/// WPF implementation of IUiHost for hosting Avalonia content in WPF.
/// Uses HwndHost to embed the Avalonia window HWND.
/// </summary>
internal class WpfHost : IUiHost
{
    private HwndHost? _hwndHost;
    private IntPtr _contentHandle;
    private Size _contentSize;
    private bool _disposed;

    public IntPtr ContentHandle => _contentHandle;
    public Size ContentSize => _contentSize;

    /// <summary>
    /// Creates a new WPF host for the specified Avalonia window handle.
    /// </summary>
    /// <param name="hwnd">The HWND of the Avalonia window to host.</param>
    public void Initialize(IntPtr hwnd)
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(WpfHost));

        _contentHandle = hwnd;
        _hwndHost = new HwndHost();
        _hwndHost.Child = new FrameworkElement(); // Placeholder
    }

    public void SetContentSize(float width, float height)
    {
        _contentSize = new Size(width, height);
        // Resize the hosted content if needed
    }

    public void Show()
    {
        // Content is shown when hosted in WPF visual tree
    }

    public void Hide()
    {
        // Hide the hosted content
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        _hwndHost?.Dispose();
        _hwndHost = null;
        _contentHandle = IntPtr.Zero;
        _disposed = true;
    }

    /// <summary>
    /// Builds the handle for the hosted content.
    /// </summary>
    protected override IntPtr BuildWindowCore(IntPtr hwndParent)
    {
        return _contentHandle;
    }

    /// <summary>
    /// Destroys the handle for the hosted content.
    /// </summary>
    protected override void DestroyWindowCore(IntPtr hwnd)
    {
        // Handle is managed by Avalonia
    }
}
#else
// Placeholder for non-WPF builds
internal class WpfHost : IUiHost
{
    private IntPtr _contentHandle;
    private Size _contentSize;
    private bool _disposed;

    public IntPtr ContentHandle => _contentHandle;
    public Size ContentSize => _contentSize;

    public void Initialize(IntPtr hwnd)
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(WpfHost));

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
