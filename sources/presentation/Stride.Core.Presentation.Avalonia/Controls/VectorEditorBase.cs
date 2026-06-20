using System;
using Avalonia;
using Avalonia.Data;

namespace Stride.Core.Presentation.Controls;

/// <summary>
/// Base class for vector editors.
/// </summary>
public abstract class VectorEditorBase : AvaloniaObject
{
    /// <summary>
    /// Identifies the <see cref="DecimalPlaces"/> styled property.
    /// </summary>
    public static readonly StyledProperty<int> DecimalPlacesProperty =
        AvaloniaProperty.Register<VectorEditorBase, int>(nameof(DecimalPlaces), defaultValue: -1);

    /// <summary>
    /// Identifies the <see cref="IsDropDownOpen"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> IsDropDownOpenProperty =
        AvaloniaProperty.Register<VectorEditorBase, bool>(nameof(IsDropDownOpen), defaultValue: false);

    /// <summary>
    /// Identifies the <see cref="WatermarkContent"/> styled property.
    /// </summary>
    public static readonly StyledProperty<object?> WatermarkContentProperty =
        AvaloniaProperty.Register<VectorEditorBase, object?>(nameof(WatermarkContent));

    /// <summary>
    /// Gets or sets the number of decimal places displayed in the <see cref="NumericTextBox"/>.
    /// </summary>
    public int DecimalPlaces
    {
        get => GetValue(DecimalPlacesProperty);
        set => SetValue(DecimalPlacesProperty, value);
    }

    /// <summary>
    /// Gets or sets whether the drop-down of this vector editor is currently open.
    /// </summary>
    public bool IsDropDownOpen
    {
        get => GetValue(IsDropDownOpenProperty);
        set => SetValue(IsDropDownOpenProperty, value);
    }

    /// <summary>
    /// Gets or sets the content to display when the TextBox is empty.
    /// </summary>
    public object? WatermarkContent
    {
        get => GetValue(WatermarkContentProperty);
        set => SetValue(WatermarkContentProperty, value);
    }

    /// <summary>
    /// Sets the vector value of this vector editor from a single float value.
    /// </summary>
    /// <param name="value">The value to use to generate a vector.</param>
    public abstract void SetVectorFromValue(float value);

    public abstract void ResetValue();
}

/// <summary>
/// Generic base class for vector editors.
/// </summary>
public abstract class VectorEditorBase<T> : VectorEditorBase
{
    private bool _interlock;
    private AvaloniaProperty? _initializingProperty;

    /// <summary>
    /// Identifies the <see cref="Value"/> styled property.
    /// </summary>
    public static readonly StyledProperty<T?> ValueProperty =
        AvaloniaProperty.Register<VectorEditorBase<T>, T?>(nameof(Value));

    /// <summary>
    /// Identifies the <see cref="DefaultValue"/> styled property.
    /// </summary>
    public static readonly StyledProperty<T?> DefaultValueProperty =
        AvaloniaProperty.Register<VectorEditorBase<T>, T?>(nameof(DefaultValue));

    /// <summary>
    /// Gets or sets the vector associated to this control.
    /// </summary>
    public T? Value
    {
        get => GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    /// <summary>
    /// Gets or sets the value that will be used by the <see cref="VectorEditorBase.ResetValue"/> method to reset the <see cref="Value"/> of this control.
    /// </summary>
    public T? DefaultValue
    {
        get => GetValue(DefaultValueProperty);
        set => SetValue(DefaultValueProperty, value);
    }

    static VectorEditorBase()
    {
        ValueProperty.Changed.AddClassHandler<VectorEditorBase<T>>((editor, e) => editor.OnValueValueChanged());
    }

    public override void SetVectorFromValue(float value)
    {
        Value = UpdateValueFromFloat(value);
    }

    public override void ResetValue()
    {
        Value = DefaultValue;
    }

    /// <summary>
    /// Updates the properties corresponding to the components of the vector from the given vector value.
    /// </summary>
    /// <param name="value">The vector from which to update component properties.</param>
    protected abstract void UpdateComponentsFromValue(T? value);

    /// <summary>
    /// Updates the <see cref="Value"/> property according to a change in the given component property.
    /// </summary>
    /// <param name="property">The component property from which to update the <see cref="Value"/>.</param>
    protected abstract T? UpdateValueFromComponent(AvaloniaProperty property);

    /// <summary>
    /// Updates the <see cref="Value"/> property from a single float.
    /// </summary>
    /// <param name="value">The value to use to generate a vector.</param>
    protected abstract T? UpdateValueFromFloat(float value);

    private void OnValueValueChanged()
    {
        var isInitializing = _initializingProperty == null;
        if (isInitializing)
            _initializingProperty = ValueProperty;

        if (!_interlock)
        {
            _interlock = true;
            UpdateComponentsFromValue(Value);
            _interlock = false;
        }

        // Avalonia bindings auto-update, no need to manually update source
        if (isInitializing)
            _initializingProperty = null;
    }

    protected virtual void OnComponentPropertyChanged(AvaloniaProperty property)
    {
        var isInitializing = _initializingProperty == null;
        if (isInitializing)
            _initializingProperty = property;

        if (!_interlock)
        {
            _interlock = true;
            Value = UpdateValueFromComponent(property);
            UpdateComponentsFromValue(Value);
            _interlock = false;
        }

        if (isInitializing)
            _initializingProperty = null;
    }

    protected static void OnComponentPropertyChanged<TEditor>(TEditor editor, AvaloniaPropertyChangedEventArgs e)
        where TEditor : VectorEditorBase<T>
    {
        editor.OnComponentPropertyChanged(e.Property);
    }

    protected static object? CoerceComponentValue(AvaloniaObject sender, object? basevalue)
    {
        if (basevalue == null)
            return null;

        var editor = (VectorEditorBase<T>)sender;
        var decimalPlaces = editor.DecimalPlaces;
        return decimalPlaces < 0 ? basevalue : MathF.Round(Convert.ToSingle(basevalue), decimalPlaces);
    }
}
