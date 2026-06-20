using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;

namespace Stride.Core.Presentation.Controls;

/// <summary>
/// Avalonia implementation of PropertyView (PropertyGrid).
/// Displays a hierarchical collection of property items with highlight/hover/keyboard-active states.
/// </summary>
public class PropertyView : ItemsControl
{
    public static readonly StyledProperty<GridLength> NameColumnSizeProperty =
        AvaloniaProperty.Register<PropertyView, GridLength>(nameof(NameColumnSize), new GridLength(150));

    public static readonly StyledProperty<PropertyViewItem?> HighlightedItemProperty =
        AvaloniaProperty.Register<PropertyView, PropertyViewItem?>(nameof(HighlightedItem));

    public static readonly StyledProperty<PropertyViewItem?> HoveredItemProperty =
        AvaloniaProperty.Register<PropertyView, PropertyViewItem?>(nameof(HoveredItem));

    public static readonly StyledProperty<PropertyViewItem?> KeyboardActiveItemProperty =
        AvaloniaProperty.Register<PropertyView, PropertyViewItem?>(nameof(KeyboardActiveItem));

    static PropertyView()
    {
        FocusableProperty.OverrideDefaultValue(typeof(PropertyView), true);
    }

    public PropertyView()
    {
        PointerMoved += OnPointerMoved;
        PointerReleased += OnPointerReleased;
    }

    public GridLength NameColumnSize
    {
        get => GetValue(NameColumnSizeProperty);
        set => SetValue(NameColumnSizeProperty, value);
    }

    public PropertyViewItem? HighlightedItem
    {
        get => GetValue(HighlightedItemProperty);
        set => SetValue(HighlightedItemProperty, value);
    }

    public PropertyViewItem? HoveredItem
    {
        get => GetValue(HoveredItemProperty);
        set => SetValue(HoveredItemProperty, value);
    }

    public PropertyViewItem? KeyboardActiveItem
    {
        get => GetValue(KeyboardActiveItemProperty);
        set => SetValue(KeyboardActiveItemProperty, value);
    }

    private void OnPointerMoved(object? sender, PointerEventArgs e)
    {
        if (sender is PropertyViewItem item)
        {
            if (item.Highlightable)
                HighlightedItem = item;

            HoveredItem = item;
        }
    }

    private void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        HoveredItem = null;
        HighlightedItem = null;
    }

    internal void KeyboardActivateItem(PropertyViewItem? item)
    {
        KeyboardActiveItem = item;
    }
}
