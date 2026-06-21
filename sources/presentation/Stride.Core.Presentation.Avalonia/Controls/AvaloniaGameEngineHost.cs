// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Platform;

namespace Stride.Core.Presentation.Avalonia.Controls
{
    /// <summary>
    /// An Avalonia control that hosts a game engine rendering window using <see cref="NativeControlHost"/>.
    /// This is the Avalonia equivalent of the WPF <c>GameEngineHost</c>.
    /// </summary>
    public class AvaloniaGameEngineHost : NativeControlHost, IDisposable
    {
        private IntPtr _childHandle;
        private bool _disposed;

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, uint uFlags);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool DestroyWindow(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr GetParent(IntPtr hWnd);

        private const uint SWP_NOZORDER = 0x0004;
        private const uint SWP_SHOWWINDOW = 0x0040;
        private const uint SWP_NOMOVE = 0x0002;
        private const uint SWP_NOSIZE = 0x0001;
        private const int SW_SHOW = 5;
        private const int SW_HIDE = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="AvaloniaGameEngineHost"/> class.
        /// </summary>
        /// <param name="childHandle">The native window handle of the child rendering window.</param>
        public AvaloniaGameEngineHost(IntPtr childHandle)
        {
            _childHandle = childHandle;
            MinWidth = 32;
            MinHeight = 32;
            Focusable = true;
        }

        /// <summary>
        /// Gets the native handle of the hosted rendering window.
        /// </summary>
        public IntPtr ChildHandle => _childHandle;

        /// <summary>
        /// Creates the native control and reparents the child window.
        /// </summary>
        protected override IPlatformHandle CreateNativeControlCore(IPlatformHandle parent)
        {
            if (_childHandle != IntPtr.Zero && parent != null && parent.Handle != IntPtr.Zero)
            {
                // Reparent the child rendering window to this control
                SetParent(_childHandle, parent.Handle);

                // Position the child to fill the control
                var bounds = Bounds;
                SetWindowPos(_childHandle, IntPtr.Zero,
                    0, 0, Math.Max(1, (int)bounds.Width), Math.Max(1, (int)bounds.Height),
                    SWP_NOZORDER | SWP_SHOWWINDOW);

                ShowWindow(_childHandle, SW_SHOW);
            }

            return base.CreateNativeControlCore(parent);
        }

        /// <summary>
        /// Called when the control is resized, forwarding the new size to the child window.
        /// </summary>
        protected override void OnSizeChanged(SizeChangedEventArgs e)
        {
            base.OnSizeChanged(e);

            if (_childHandle != IntPtr.Zero && e.NewSize.Width > 0 && e.NewSize.Height > 0)
            {
                // Forward the new size to the child rendering window
                SetWindowPos(_childHandle, IntPtr.Zero,
                    0, 0, (int)e.NewSize.Width, (int)e.NewSize.Height,
                    SWP_NOZORDER);
            }
        }

        /// <summary>
        /// Gets the native host handle for rendering.
        /// Returns the parent control's native handle that the game engine can render into.
        /// </summary>
        public IntPtr GetNativeHostHandle()
        {
            if (_childHandle != IntPtr.Zero)
            {
                return GetParent(_childHandle);
            }
            return IntPtr.Zero;
        }

        /// <summary>
        /// Disposes the control and cleans up the child window.
        /// </summary>
        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            if (_childHandle != IntPtr.Zero)
            {
                // Detach and destroy the child window
                SetParent(_childHandle, IntPtr.Zero);
                DestroyWindow(_childHandle);
                _childHandle = IntPtr.Zero;
            }
        }
    }
}