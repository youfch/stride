// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

#if AVALONIA

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Stride.Core.Presentation.Quantum.ViewModels;

namespace Stride.GameStudio.View.Panels
{
    /// <summary>
    /// A control that displays a single row in the property grid, with support for expand/collapse of child nodes.
    /// </summary>
    public partial class PropertyRowControl : UserControl
    {
        public static readonly StyledProperty<NodeViewModel?> NodeProperty =
            AvaloniaProperty.Register<PropertyRowControl, NodeViewModel?>(nameof(Node));

        public static readonly StyledProperty<bool> IsExpandedProperty =
            AvaloniaProperty.Register<PropertyRowControl, bool>(nameof(IsExpanded), false);

        public static readonly StyledProperty<bool> HasChildrenProperty =
            AvaloniaProperty.Register<PropertyRowControl, bool>(nameof(HasChildren));

        public PropertyRowControl()
        {
        }

        public NodeViewModel? Node
        {
            get => GetValue(NodeProperty);
            set => SetValue(NodeProperty, value);
        }

        public bool IsExpanded
        {
            get => GetValue(IsExpandedProperty);
            set => SetValue(IsExpandedProperty, value);
        }

        public bool HasChildren
        {
            get => GetValue(HasChildrenProperty);
            set => SetValue(HasChildrenProperty, value);
        }

        protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnAttachedToVisualTree(e);
            UpdateProperties();
        }

        protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnDetachedFromVisualTree(e);
        }

        private void UpdateProperties()
        {
            if (Node != null)
            {
                HasChildren = Node.VisibleChildrenCount > 0;
            }
        }

        private void OnToggleExpand(object? sender, RoutedEventArgs e)
        {
            IsExpanded = !IsExpanded;
        }

        static PropertyRowControl()
        {
            IsExpandedProperty.Changed.AddClassHandler<PropertyRowControl>((control, e) => control.OnIsExpandedChanged(e));
        }

        private void OnIsExpandedChanged(AvaloniaPropertyChangedEventArgs e)
        {
            // Force a layout update to show/hide children
            InvalidateMeasure();
            InvalidateArrange();
        }
    }
}

#endif
