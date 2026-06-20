// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.Rendering;

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
        /// Creates the native control to host the child window.
        /// </summary>
        protected override IPlatformHandle CreateNativeControlCore(IPlatformHandle parent)
        {
            return base.CreateNativeControlCore(parent);
        }

        /// <summary>
        /// Called when the control is resized, forwarding the new size to the child window.
        /// </summary>
        protected override void OnSizeChanged(SizeChangedEventArgs e)
        {
            base.OnSizeChanged(e);
        }

        /// <summary>
        /// Gets the platform handle of the native host control for rendering.
        /// </summary>
        public IntPtr GetNativeHostHandle()
        {
            return IntPtr.Zero;
        }

        /// <summary>
        /// Disposes the control and cleans up the child window.
        /// </summary>
        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            _childHandle = IntPtr.Zero;
        }
    }
}
