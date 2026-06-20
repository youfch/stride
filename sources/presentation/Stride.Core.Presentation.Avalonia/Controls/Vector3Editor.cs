using System;
using Avalonia;
using Avalonia.Controls;
using Stride.Core.Mathematics;

namespace Stride.Core.Presentation.Controls;

/// <summary>
/// Editor control for <see cref="Vector3"/> values.
/// </summary>
public class Vector3Editor : VectorEditor<Vector3?>
{
    /// <summary>
    /// Identifies the <see cref="X"/> styled property.
    /// </summary>
    public static readonly StyledProperty<float?> XProperty =
        AvaloniaProperty.Register<Vector3Editor, float?>(nameof(X));

    /// <summary>
    /// Identifies the <see cref="Y"/> styled property.
    /// </summary>
    public static readonly StyledProperty<float?> YProperty =
        AvaloniaProperty.Register<Vector3Editor, float?>(nameof(Y));

    /// <summary>
    /// Identifies the <see cref="Z"/> styled property.
    /// </summary>
    public static readonly StyledProperty<float?> ZProperty =
        AvaloniaProperty.Register<Vector3Editor, float?>(nameof(Z));

    /// <summary>
    /// Identifies the <see cref="Length"/> styled property.
    /// </summary>
    public static readonly StyledProperty<float?> LengthProperty =
        AvaloniaProperty.Register<Vector3Editor, float?>(nameof(Length));

    static Vector3Editor()
    {
        XProperty.Changed.AddClassHandler<Vector3Editor>((editor, e) => editor.OnComponentPropertyChanged(XProperty));
        YProperty.Changed.AddClassHandler<Vector3Editor>((editor, e) => editor.OnComponentPropertyChanged(YProperty));
        ZProperty.Changed.AddClassHandler<Vector3Editor>((editor, e) => editor.OnComponentPropertyChanged(ZProperty));
        LengthProperty.Changed.AddClassHandler<Vector3Editor>((editor, e) => editor.OnComponentPropertyChanged(LengthProperty));
    }

    /// <summary>
    /// Gets or sets the X component (in Cartesian coordinate system) of the <see cref="Vector3"/> associated to this control.
    /// </summary>
    public float? X
    {
        get => GetValue(XProperty);
        set => SetValue(XProperty, value);
    }

    /// <summary>
    /// Gets or sets the Y component (in Cartesian coordinate system) of the <see cref="Vector3"/> associated to this control.
    /// </summary>
    public float? Y
    {
        get => GetValue(YProperty);
        set => SetValue(YProperty, value);
    }

    /// <summary>
    /// Gets or sets the Z component (in Cartesian coordinate system) of the <see cref="Vector3"/> associated to this control.
    /// </summary>
    public float? Z
    {
        get => GetValue(ZProperty);
        set => SetValue(ZProperty, value);
    }

    /// <summary>
    /// Gets or sets the length (in polar coordinate system) of the <see cref="Vector3"/> associated to this control.
    /// </summary>
    public float? Length
    {
        get => GetValue(LengthProperty);
        set => SetValue(LengthProperty, value);
    }

    /// <inheritdoc/>
    protected override void UpdateComponentsFromValue(Vector3? value)
    {
        if (value != null)
        {
            SetCurrentValue(XProperty, value.Value.X);
            SetCurrentValue(YProperty, value.Value.Y);
            SetCurrentValue(ZProperty, value.Value.Z);
            SetCurrentValue(LengthProperty, value.Value.Length());
        }
    }

    /// <inheritdoc/>
    protected override Vector3? UpdateValueFromComponent(AvaloniaProperty property)
    {
        switch (EditingMode)
        {
            case VectorEditingMode.Normal:
                if (property == XProperty)
                    return X.HasValue && Value.HasValue ? new Vector3(X.Value, Value.Value.Y, Value.Value.Z) : null;
                if (property == YProperty)
                    return Y.HasValue && Value.HasValue ? new Vector3(Value.Value.X, Y.Value, Value.Value.Z) : null;
                if (property == ZProperty)
                    return Z.HasValue && Value.HasValue ? new Vector3(Value.Value.X, Value.Value.Y, Z.Value) : null;
                break;

            case VectorEditingMode.AllComponents:
                if (property == XProperty)
                    return X.HasValue ? new Vector3(X.Value) : null;
                if (property == YProperty)
                    return Y.HasValue ? new Vector3(Y.Value) : null;
                if (property == ZProperty)
                    return Z.HasValue ? new Vector3(Z.Value) : null;
                break;

            case VectorEditingMode.Length:
                if (property == LengthProperty)
                    return Length.HasValue ? FromLength(Value ?? Vector3.One, Length.Value) : null;
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(EditingMode));
        }

        throw new ArgumentException($"Property {property} is unsupported by method {nameof(UpdateValueFromComponent)} in {EditingMode} mode.");
    }

    /// <inheritdoc/>
    protected override Vector3? UpdateValueFromFloat(float value)
    {
        return new Vector3(value);
    }

    private static Vector3 FromLength(Vector3 value, float length)
    {
        var newValue = value;
        newValue.Normalize();
        newValue *= length;
        return newValue;
    }
}
