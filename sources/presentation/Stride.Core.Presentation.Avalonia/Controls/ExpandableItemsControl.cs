using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace Stride.Core.Presentation.Controls;

/// <summary>
/// Base class for expandable items controls (like tree nodes that can show/hide child items).
/// </summary>
public class ExpandableItemsControl : ItemsControl
{
    public static readonly StyledProperty<bool> IsExpandedProperty =
        AvaloniaProperty.Register<ExpandableItemsControl, bool>(nameof(IsExpanded));

    public bool IsExpanded
    {
        get => GetValue(IsExpandedProperty);
        set => SetValue(IsExpandedProperty, value);
    }
}

/// <summary>
/// Interface for items that support expansion/collapse.
/// </summary>
public interface IExpandable
{
    bool IsExpanded { get; set; }
}
