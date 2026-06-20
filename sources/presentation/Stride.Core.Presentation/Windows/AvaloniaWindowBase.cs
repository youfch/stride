// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

#if AVALONIA

using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;

namespace Stride.Core.Presentation.Windows
{
    /// <summary>
    /// Base window class for Avalonia that provides compatibility with WPF event handlers.
    /// </summary>
    public abstract class AvaloniaWindowBase : Window
    {
        /// <summary>
        /// Called when the window is closed.
        /// </summary>
        protected virtual void OnClosed(EventArgs e)
        {
            // Avalonia uses Closed event, not an overridable method
        }

        /// <summary>
        /// Called when the window is closing.
        /// </summary>
        protected virtual void OnClosing(CancelEventArgs e)
        {
            // Avalonia uses Closing event
        }

        /// <summary>
        /// Called when the window state changes.
        /// </summary>
        protected virtual void OnStateChanged(EventArgs e)
        {
            // Avalonia uses WindowState property and StateChanged event
        }

        /// <summary>
        /// Called when the window is loaded.
        /// </summary>
        protected virtual void OnLoaded(RoutedEventArgs e)
        {
            // Avalonia uses Loaded event
        }

        /// <summary>
        /// Called when the window is unloaded.
        /// </summary>
        protected virtual void OnUnloaded(RoutedEventArgs e)
        {
            // Avalonia uses Unloaded event
        }

        /// <summary>
        /// Called when a mouse button is pressed.
        /// </summary>
        protected virtual void OnMouseDown(MouseButtonEventArgs e)
        {
            // Avalonia uses MouseDown event
        }

        /// <summary>
        /// Called when a mouse button is released.
        /// </summary>
        protected virtual void OnMouseUp(MouseButtonEventArgs e)
        {
            // Avalonia uses MouseUp event
        }

        /// <summary>
        /// Called when the mouse moves.
        /// </summary>
        protected virtual void OnMouseMove(MouseEventArgs e)
        {
            // Avalonia uses MouseMove event
        }

        /// <summary>
        /// Called when the mouse enters the window.
        /// </summary>
        protected virtual void OnMouseEnter(MouseEventArgs e)
        {
            // Avalonia uses MouseEnter event
        }

        /// <summary>
        /// Called when the mouse leaves the window.
        /// </summary>
        protected virtual void OnMouseLeave(MouseEventArgs e)
        {
            // Avalonia uses MouseLeave event
        }

        /// <summary>
        /// Called when a key is pressed.
        /// </summary>
        protected virtual void OnKeyDown(KeyEventArgs e)
        {
            // Avalonia uses KeyDown event
        }

        /// <summary>
        /// Called when a key is released.
        /// </summary>
        protected virtual void OnKeyUp(KeyEventArgs e)
        {
            // Avalonia uses KeyUp event
        }

        /// <summary>
        /// Called when the window receives input.
        /// </summary>
        protected virtual void OnPreviewKeyDown(KeyEventArgs e)
        {
            // Avalonia uses KeyDown event (no preview by default)
        }

        /// <summary>
        /// Called when the window receives input.
        /// </summary>
        protected virtual void OnPreviewKeyUp(KeyEventArgs e)
        {
            // Avalonia uses KeyUp event (no preview by default)
        }

        /// <summary>
        /// Called when the window receives mouse input.
        /// </summary>
        protected virtual void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            // Avalonia uses MouseDown event (no preview by default)
        }

        /// <summary>
        /// Called when the window receives mouse input.
        /// </summary>
        protected virtual void OnPreviewMouseUp(MouseButtonEventArgs e)
        {
            // Avalonia uses MouseUp event (no preview by default)
        }

        /// <summary>
        /// Called when the window receives mouse input.
        /// </summary>
        protected virtual void OnPreviewMouseMove(MouseEventArgs e)
        {
            // Avalonia uses MouseMove event (no preview by default)
        }

        /// <summary>
        /// Called when the window receives mouse input.
        /// </summary>
        protected virtual void OnPreviewMouseWheel(MouseWheelEventArgs e)
        {
            // Avalonia uses MouseWheel event (no preview by default)
        }

        /// <summary>
        /// Called when the window receives scroll input.
        /// </summary>
        protected virtual void OnPreviewMouseWheel(MouseWheelEventArgs e)
        {
            // Avalonia uses MouseWheel event (no preview by default)
        }
    }
}

#endif