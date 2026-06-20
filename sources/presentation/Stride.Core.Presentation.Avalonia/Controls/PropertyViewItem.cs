using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace Stride.Core.Presentation.Controls;

/// <summary>
/// Container item for <see cref="PropertyView"/>. Supports expandable nested properties.
/// </summary>
public class PropertyViewItem : ExpandableItemsControl
{
    public static readonly StyledProperty<bool> HighlightableProperty =
        AvaloniaProperty.Register<PropertyViewItem, bool>(nameof(Highlightable), true);

    public static readonly StyledProperty<bool> IsHighlightedProperty =
        AvaloniaProperty.Register<PropertyViewItem, bool>(nameof(IsHighlighted));

    public static readonly StyledProperty<bool> IsHoveredProperty =
        AvaloniaProperty.Register<PropertyViewItem, bool>(nameof(IsHovered));

    public static readonly StyledProperty<bool> IsKeyboardActiveProperty =
        AvaloniaProperty.Register<PropertyViewItem, bool>(nameof(IsKeyboardActive));

    public static readonly StyledProperty<double> OffsetProperty =
        AvaloniaProperty.Register<PropertyViewItem, double>(nameof(Offset));

    public static readonly StyledProperty<double> IncrementProperty =
        AvaloniaProperty.Register<PropertyViewItem, double>(nameof(Increment));

    public PropertyViewItem(PropertyView propertyView)
    {
        PropertyView = propertyView;
        PointerMoved += OnParentPointerMoved;
        GotFocus += OnGotFocus;
    }

    public PropertyView PropertyView { get; }

    public bool Highlightable
    {
        get => GetValue(HighlightableProperty);
        set => SetValue(HighlightableProperty, value);
    }

    public bool IsHighlighted
    {
        get => GetValue(IsHighlightedProperty);
        set => SetValue(IsHighlightedProperty, value);
    }

    public bool IsHovered
    {
        get => GetValue(IsHoveredProperty);
        set => SetValue(IsHoveredProperty, value);
    }

    public bool IsKeyboardActive
    {
        get => GetValue(IsKeyboardActiveProperty);
        set => SetValue(IsKeyboardActiveProperty, value);
    }

    public double Offset
    {
        get => GetValue(OffsetProperty);
        set => SetValue(OffsetProperty, value);
    }

    public double Increment
    {
        get => GetValue(IncrementProperty);
        set => SetValue(IncrementProperty, value);
    }

    private void OnParentPointerMoved(object? sender, PointerEventArgs e)
    {
        PropertyView?.KeyboardActivateItem(this);
    }

    private void OnGotFocus(object? sender, RoutedEventArgs e)
    {
        PropertyView?.KeyboardActivateItem(this);
    }
}
