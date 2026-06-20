using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace Stride.Core.Presentation.Controls;

/// <summary>
/// Selection mode for TreeView.
/// </summary>
public enum SelectionMode
{
    None,
    Single,
    Extended
}

/// <summary>
/// Event args for TreeViewItem events.
/// </summary>
public class TreeViewItemEventArgs : RoutedEventArgs
{
    public TreeViewItemEventArgs(RoutedEvent @event, object source, TreeViewItem item, object dataContext)
        : base(@event)
    {
        Source = source;
        Item = item;
        DataContext = dataContext;
    }

    public object Source { get; }
    public TreeViewItem Item { get; }
    public object DataContext { get; }
}

/// <summary>
/// Represents a control that displays hierarchical data in a tree structure.
/// </summary>
public class TreeView : TemplatedControl
{
    /// <summary>
    /// Identifies the <see cref="Items"/> styled property.
    /// </summary>
    public static readonly StyledProperty<IEnumerable?> ItemsProperty =
        AvaloniaProperty.Register<TreeView, IEnumerable?>(nameof(Items));

    /// <summary>
    /// Identifies the <see cref="SelectedItem"/> styled property.
    /// </summary>
    public static readonly StyledProperty<object?> SelectedItemProperty =
        AvaloniaProperty.Register<TreeView, object?>(nameof(SelectedItem));

    /// <summary>
    /// Identifies the <see cref="SelectedItems"/> styled property.
    /// </summary>
    public static readonly StyledProperty<AvaloniaList<object>> SelectedItemsProperty =
        AvaloniaProperty.Register<TreeView, AvaloniaList<object>>(nameof(SelectedItems));

    /// <summary>
    /// Identifies the <see cref="SelectionMode"/> styled property.
    /// </summary>
    public static readonly StyledProperty<SelectionMode> SelectionModeProperty =
        AvaloniaProperty.Register<TreeView, SelectionMode>(nameof(SelectionMode), defaultValue: SelectionMode.Extended);

    /// <summary>
    /// Identifies the <see cref="IsVirtualizing"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> IsVirtualizingProperty =
        AvaloniaProperty.Register<TreeView, bool>(nameof(IsVirtualizing), defaultValue: false);

    static TreeView()
    {
        ItemsProperty.Changed.AddClassHandler<TreeView>((treeView, e) => treeView.OnItemsChanged());
        SelectedItemProperty.Changed.AddClassHandler<TreeView>((treeView, e) => treeView.OnSelectedItemChanged());
        SelectionModeProperty.Changed.AddClassHandler<TreeView>((treeView, e) => treeView.OnSelectionModeChanged());
    }

    private readonly AvaloniaList<object> _selectedItems;
    private bool _updatingSelection;
    private object? _lastShiftRoot;

    public TreeView()
    {
        _selectedItems = new AvaloniaList<object>();
        SelectedItems = _selectedItems;
    }

    /// <summary>
    /// Gets or sets the items to display.
    /// </summary>
    public IEnumerable? Items
    {
        get => GetValue(ItemsProperty);
        set => SetValue(ItemsProperty, value);
    }

    /// <summary>
    /// Gets the last selected item.
    /// </summary>
    public object? SelectedItem
    {
        get => GetValue(SelectedItemProperty);
        set => SetValue(SelectedItemProperty, value);
    }

    /// <summary>
    /// Gets the list of selected items.
    /// </summary>
    public AvaloniaList<object> SelectedItems
    {
        get => _selectedItems;
        private set { }
    }

    /// <summary>
    /// Gets or sets the selection mode.
    /// </summary>
    public SelectionMode SelectionMode
    {
        get => GetValue(SelectionModeProperty);
        set => SetValue(SelectionModeProperty, value);
    }

    /// <summary>
    /// Gets or sets whether virtualization is enabled.
    /// </summary>
    public bool IsVirtualizing
    {
        get => GetValue(IsVirtualizingProperty);
        set => SetValue(IsVirtualizingProperty, value);
    }

    private void OnItemsChanged()
    {
        // Items changed - selection may need to be updated
    }

    private void OnSelectedItemChanged()
    {
        if (_updatingSelection) return;

        var newValue = SelectedItem;
        
        // Update SelectedItems collection to match
        if (newValue != null && !_selectedItems.Contains(newValue))
        {
            _updatingSelection = true;
            _selectedItems.Clear();
            _selectedItems.Add(newValue);
            _updatingSelection = false;
        }
        else if (newValue == null && _selectedItems.Count > 0)
        {
            _updatingSelection = true;
            _selectedItems.Clear();
            _updatingSelection = false;
        }
    }

    private void OnSelectionModeChanged()
    {
        if (SelectionMode == SelectionMode.None)
        {
            // Clear selections when in None mode
            if (_selectedItems.Count > 0)
            {
                _updatingSelection = true;
                _selectedItems.Clear();
                SelectedItem = null;
                _updatingSelection = false;
            }
        }
        // When switching to Single mode, clear extra selections
        else if (SelectionMode == SelectionMode.Single && _selectedItems.Count > 1)
        {
            _updatingSelection = true;
            while (_selectedItems.Count > 1)
            {
                _selectedItems.RemoveAt(_selectedItems.Count - 1);
            }
            _updatingSelection = false;
        }
    }

    internal void SelectItem(TreeViewItem item)
    {
        if (item == null) return;

        if (SelectionMode == SelectionMode.None)
            return;

        if (SelectionMode == SelectionMode.Single)
        {
            SelectSingleItem(item);
        }
        else if (SelectionMode == SelectionMode.Extended)
        {
            if (IsControlKeyDown)
            {
                ToggleItem(item);
            }
            else if (IsShiftKeyDown && _lastShiftRoot != null)
            {
                SelectWithShift(item);
            }
            else
            {
                SelectSingleItem(item);
            }
        }
    }

    internal void SelectSingleItem(TreeViewItem item)
    {
        _updatingSelection = true;
        _selectedItems.Clear();
        
        if (item.DataContext != null)
        {
            _selectedItems.Add(item.DataContext);
            SelectedItem = item.DataContext;
        }
        else
        {
            SelectedItem = null;
        }
        
        _lastShiftRoot = item.DataContext;
        _updatingSelection = false;
    }

    internal void ToggleItem(TreeViewItem item)
    {
        if (item.DataContext == null) return;

        if (_selectedItems.Contains(item.DataContext))
        {
            _selectedItems.Remove(item.DataContext);
            if (SelectedItem == item.DataContext)
            {
                SelectedItem = _selectedItems.Count > 0 ? _selectedItems[^1] : null;
            }
        }
        else
        {
            _selectedItems.Add(item.DataContext);
            SelectedItem = item.DataContext;
        }
        
        _lastShiftRoot = item.DataContext;
    }

    internal void SelectWithShift(TreeViewItem item)
    {
        if (item.DataContext == null) return;

        // Find the range between lastShiftRoot and current item
        // For simplicity, just select the current item
        _updatingSelection = true;
        _selectedItems.Clear();
        _selectedItems.Add(item.DataContext);
        SelectedItem = item.DataContext;
        _lastShiftRoot = item.DataContext;
        _updatingSelection = false;
    }

    internal static bool IsControlKeyDown => false; // Simplified - can be enhanced with key tracking
    internal static bool IsShiftKeyDown => false; // Simplified - can be enhanced with key tracking
}
