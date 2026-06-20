using Avalonia;
using Avalonia.Controls;

namespace Stride.Core.Presentation.Controls;

/// <summary>
/// Editing mode for vector editors.
/// </summary>
public enum VectorEditingMode
{
    /// <summary>
    /// Normal editing mode (X, Y components).
    /// </summary>
    Normal,

    /// <summary>
    /// All components editing mode.
    /// </summary>
    AllComponents,

    /// <summary>
    /// Length editing mode.
    /// </summary>
    Length
}

/// <summary>
/// Base class for vector editors with editing mode support.
/// </summary>
public abstract class VectorEditor<T> : VectorEditorBase<T>
{
    /// <summary>
    /// Identifies the <see cref="EditingMode"/> styled property.
    /// </summary>
    public static readonly StyledProperty<VectorEditingMode> EditingModeProperty =
        AvaloniaProperty.Register<VectorEditor<T>, VectorEditingMode>(nameof(EditingMode), defaultValue: VectorEditingMode.Normal);

    /// <summary>
    /// Gets or sets the editing mode.
    /// </summary>
    public VectorEditingMode EditingMode
    {
        get => GetValue(EditingModeProperty);
        set => SetValue(EditingModeProperty, value);
    }

    static VectorEditor()
    {
        EditingModeProperty.Changed.AddClassHandler<VectorEditor<T>>((editor, e) =>
        {
            // When editing mode changes, update components from current value
            editor.UpdateComponentsFromValue(editor.Value);
        });
    }
}
