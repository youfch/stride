using System;
using System.Collections;
using System.Collections.Specialized;
using System.Linq;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.VisualTree;

namespace Stride.Core.Presentation.Controls;

/// <summary>
/// An item of the TreeView.
/// </summary>
public class TreeViewItem : ItemsControl
{
    /// <summary>
    /// Identifies the <see cref="IsExpanded"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> IsExpandedProperty =
        AvaloniaProperty.Register<TreeViewItem, bool>(nameof(IsExpanded));

    /// <summary>
    /// Identifies the <see cref="IsSelected"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> IsSelectedProperty =
        AvaloniaProperty.Register<TreeViewItem, bool>(nameof(IsSelected));

    /// <summary>
    /// Identifies the <see cref="IsEditable"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> IsEditableProperty =
        AvaloniaProperty.Register<TreeViewItem, bool>(nameof(IsEditable), defaultValue: true);

    /// <summary>
    /// Identifies the <see cref="IsEditing"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> IsEditingProperty =
        AvaloniaProperty.Register<TreeViewItem, bool>(nameof(IsEditing));

    /// <summary>
    /// Identifies the <see cref="Indentation"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> IndentationProperty =
        AvaloniaProperty.Register<TreeViewItem, double>(nameof(Indentation), defaultValue: 10.0);

    /// <summary>
    /// Identifies the <see cref="HasItems"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> HasItemsProperty =
        AvaloniaProperty.Register<TreeViewItem, bool>(nameof(HasItems));

    static TreeViewItem()
    {
        IsExpandedProperty.Changed.AddClassHandler<TreeViewItem>((item, e) => item.OnIsExpandedChanged());
        IsSelectedProperty.Changed.AddClassHandler<TreeViewItem>((item, e) => item.OnIsSelectedChanged());
    }

    private TreeView? _parentTreeView;
    private bool _isInitialized;

    /// <summary>
    /// Gets or sets whether the item is expanded.
    /// </summary>
    public bool IsExpanded
    {
        get => GetValue(IsExpandedProperty);
        set => SetValue(IsExpandedProperty, value);
    }

    /// <summary>
    /// Gets or sets whether the item is selected.
    /// </summary>
    public bool IsSelected
    {
        get => GetValue(IsSelectedProperty);
        set => SetValue(IsSelectedProperty, value);
    }

    /// <summary>
    /// Gets or sets whether the item is editable.
    /// </summary>
    public bool IsEditable
    {
        get => GetValue(IsEditableProperty);
        set => SetValue(IsEditableProperty, value);
    }

    /// <summary>
    /// Gets or sets whether the item is being edited.
    /// </summary>
    public bool IsEditing
    {
        get => GetValue(IsEditingProperty);
        set => SetValue(IsEditingProperty, value);
    }

    /// <summary>
    /// Gets or sets the indentation.
    /// </summary>
    public double Indentation
    {
        get => GetValue(IndentationProperty);
        set => SetValue(IndentationProperty, value);
    }

    /// <summary>
    /// Gets whether the item has child items.
    /// </summary>
    public bool HasItems
    {
        get => GetValue(HasItemsProperty);
        private set => SetValue(HasItemsProperty, value);
    }

    /// <summary>
    /// Gets the parent TreeView.
    /// </summary>
    public TreeView? ParentTreeView => _parentTreeView;

    /// <summary>
    /// Gets the parent TreeViewItem.
    /// </summary>
    public TreeViewItem? ParentTreeViewItem => this.FindAncestorOfType<TreeViewItem>();

    /// <summary>
    /// Gets the offset (indentation) based on hierarchy level.
    /// </summary>
    public double Offset => ParentTreeViewItem?.Offset + Indentation ?? 0;

    /// <summary>
    /// Gets whether the item is visible (expanded and all parents expanded).
    /// </summary>
    public new bool IsVisible
    {
        get
        {
            if (base.IsVisible == false)
                return false;
            
            var currentItem = ParentTreeViewItem;
            while (currentItem != null)
            {
                if (!currentItem.IsExpanded)
                    return false;
                currentItem = currentItem.ParentTreeViewItem;
            }
            return true;
        }
    }

    public TreeViewItem()
    {
        // Items is initialized by the base class
    }

    internal void Initialize(TreeView parentTreeView)
    {
        if (_isInitialized) return;
        
        _parentTreeView = parentTreeView;
        _isInitialized = true;
        
        // Check if this item's DataContext is in the selection
        if (parentTreeView.SelectedItems.Contains(DataContext))
        {
            IsSelected = true;
        }
    }

    private void OnIsExpandedChanged()
    {
        if (!_isInitialized) return;
        
        // When expanded, ensure children are loaded if lazy loading is enabled
        if (IsExpanded && HasItems)
        {
            // Items are already loaded
        }
    }

    private void OnIsSelectedChanged()
    {
        if (!_isInitialized || _parentTreeView == null) return;
        
        if (IsSelected)
        {
            _parentTreeView.SelectItem(this);
        }
    }

    private void OnItemsChanged()
    {
        HasItems = Items.Count > 0;
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);
        
        if (e.Handled) return;
        
        // Toggle expansion on click
        if (HasItems)
        {
            IsExpanded = !IsExpanded;
            e.Handled = true;
        }
        
        // Select this item
        _parentTreeView?.SelectItem(this);
        e.Handled = true;
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);
        
        if (e.Handled) return;
        
        switch (e.Key)
        {
            case Key.Right:
                if (HasItems && !IsExpanded)
                {
                    IsExpanded = true;
                    e.Handled = true;
                }
                break;
                
            case Key.Left:
                if (IsExpanded)
                {
                    IsExpanded = false;
                    e.Handled = true;
                }
                else if (ParentTreeViewItem != null)
                {
                    ParentTreeViewItem.Focus();
                    e.Handled = true;
                }
                break;
                
            case Key.Down:
                FocusNext();
                e.Handled = true;
                break;
                
            case Key.Up:
                FocusPrevious();
                e.Handled = true;
                break;
                
            case Key.Space:
                _parentTreeView?.SelectItem(this);
                e.Handled = true;
                break;
                
                case Key.Enter:
                    // Activate item (could be overridden by derived classes)
                    e.Handled = true;
                    break;
        }
    }

    private void FocusNext()
    {
        if (IsExpanded && HasItems)
        {
            // Focus first child
            var firstChild = this.GetVisualChildren()
                .OfType<TreeViewItem>()
                .FirstOrDefault();
            firstChild?.Focus();
        }
        else
        {
            // Find next sibling or parent's next sibling
            var next = FindNextVisibleItem(this);
            next?.Focus();
        }
    }

    private void FocusPrevious()
    {
        var prev = FindPreviousVisibleItem(this);
        prev?.Focus();
    }

    private TreeViewItem? FindNextVisibleItem(TreeViewItem current)
    {
        if (IsExpanded && HasItems)
        {
            return this.GetVisualChildren()
                .OfType<TreeViewItem>()
                .FirstOrDefault();
        }
        
        // Walk up to find next sibling
        var parent = ParentTreeViewItem;
        if (parent == null) return null;
        
        var items = parent.Items.Cast<object>().ToList();
        var currentIndex = items.IndexOf(DataContext);
        
        for (var i = currentIndex + 1; i < items.Count; i++)
        {
            // Find the container by walking visual children
            var container = parent.GetVisualChildren()
                .OfType<TreeViewItem>()
                .FirstOrDefault();
            if (container != null)
                return container;
        }
        
        // No more siblings, go to parent's next
        return parent.FindNextVisibleItem(parent);
    }

    private TreeViewItem? FindPreviousVisibleItem(TreeViewItem current)
    {
        var parent = ParentTreeViewItem;
        if (parent == null) return null;
        
        var items = parent.Items.Cast<object>().ToList();
        var currentIndex = items.IndexOf(DataContext);
        
        for (var i = currentIndex - 1; i >= 0; i--)
        {
            // Find the container by walking visual children
            var container = parent.GetVisualChildren()
                .OfType<TreeViewItem>()
                .FirstOrDefault();
            if (container != null)
            {
                // If this item is expanded, return its last visible descendant
                if (container.IsExpanded && container.HasItems)
                {
                    return container.GetLastVisibleDescendant();
                }
                return container;
            }
        }
        
        return parent;
    }

    private TreeViewItem? GetLastVisibleDescendant()
    {
        if (!IsExpanded || !HasItems)
            return this;
        
        var lastItem = this.GetVisualChildren()
            .OfType<TreeViewItem>()
            .LastOrDefault();
        return lastItem?.GetLastVisibleDescendant() ?? this;
    }

    internal void ForceFocus()
    {
        Focus();
    }

    public override string ToString()
    {
        return DataContext != null ? $"{DataContext}" : base.ToString();
    }
}
