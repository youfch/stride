// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

#if AVALONIA

using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Stride.Core.Presentation.Quantum.ViewModels;

namespace Stride.GameStudio.View.Panels
{
    /// <summary>
    /// A control that displays an appropriate editor for a property based on its type.
    /// </summary>
    public partial class PropertyEditor : UserControl
    {
        public static readonly StyledProperty<NodeViewModel?> NodeProperty =
            AvaloniaProperty.Register<PropertyEditor, NodeViewModel?>(nameof(Node));

        public static readonly StyledProperty<bool> IsBooleanProperty =
            AvaloniaProperty.Register<PropertyEditor, bool>(nameof(IsBoolean));

        public static readonly StyledProperty<bool> IsNumericProperty =
            AvaloniaProperty.Register<PropertyEditor, bool>(nameof(IsNumeric));

        public static readonly StyledProperty<bool> IsEnumProperty =
            AvaloniaProperty.Register<PropertyEditor, bool>(nameof(IsEnum));

        public static readonly StyledProperty<bool> IsStringProperty =
            AvaloniaProperty.Register<PropertyEditor, bool>(nameof(IsString));

        public PropertyEditor()
        {
        }

        public NodeViewModel? Node
        {
            get => GetValue(NodeProperty);
            set => SetValue(NodeProperty, value);
        }

        public bool IsBoolean
        {
            get => GetValue(IsBooleanProperty);
            set => SetValue(IsBooleanProperty, value);
        }

        public bool IsNumeric
        {
            get => GetValue(IsNumericProperty);
            set => SetValue(IsNumericProperty, value);
        }

        public bool IsEnum
        {
            get => GetValue(IsEnumProperty);
            set => SetValue(IsEnumProperty, value);
        }

        public bool IsString
        {
            get => GetValue(IsStringProperty);
            set => SetValue(IsStringProperty, value);
        }

        protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnAttachedToVisualTree(e);
            UpdateTypeIndicators();
        }

        protected override void OnDataContextChanged(EventArgs e)
        {
            base.OnDataContextChanged(e);
            UpdateTypeIndicators();
        }

        private void UpdateTypeIndicators()
        {
            if (Node != null)
            {
                var type = Node.Type;
                IsBoolean = type == typeof(bool);
                IsNumeric = type == typeof(int) || type == typeof(float) || type == typeof(double) || 
                           type == typeof(decimal) || type == typeof(long) || type == typeof(short);
                IsEnum = type.IsEnum;
                IsString = type == typeof(string);
            }
            else
            {
                IsBoolean = false;
                IsNumeric = false;
                IsEnum = false;
                IsString = false;
            }
        }
    }
}

#endif
