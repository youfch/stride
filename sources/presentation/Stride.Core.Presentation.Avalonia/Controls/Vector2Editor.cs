using System;
using Avalonia;
using Avalonia.Controls;
using Stride.Core.Mathematics;

namespace Stride.Core.Presentation.Controls;

/// <summary>
/// Editor control for <see cref="Vector2"/> values.
/// </summary>
public class Vector2Editor : VectorEditor<Vector2?>
{
    /// <summary>
    /// Identifies the <see cref="X"/> styled property.
    /// </summary>
    public static readonly StyledProperty<float?> XProperty =
        AvaloniaProperty.Register<Vector2Editor, float?>(nameof(X));

    /// <summary>
    /// Identifies the <see cref="Y"/> styled property.
    /// </summary>
    public static readonly StyledProperty<float?> YProperty =
        AvaloniaProperty.Register<Vector2Editor, float?>(nameof(Y));

    /// <summary>
    /// Identifies the <see cref="Length"/> styled property.
    /// </summary>
    public static readonly StyledProperty<float?> LengthProperty =
        AvaloniaProperty.Register<Vector2Editor, float?>(nameof(Length));

    static Vector2Editor()
    {
        XProperty.Changed.AddClassHandler<Vector2Editor>((editor, e) => editor.OnComponentPropertyChanged(XProperty));
        YProperty.Changed.AddClassHandler<Vector2Editor>((editor, e) => editor.OnComponentPropertyChanged(YProperty));
        LengthProperty.Changed.AddClassHandler<Vector2Editor>((editor, e) => editor.OnComponentPropertyChanged(LengthProperty));
    }

    /// <summary>
    /// Gets or sets the X component (in Cartesian coordinate system) of the <see cref="Vector2"/> associated to this control.
    /// </summary>
    public float? X
    {
        get => GetValue(XProperty);
        set => SetValue(XProperty, value);
    }

    /// <summary>
    /// Gets or sets the Y component (in Cartesian coordinate system) of the <see cref="Vector2"/> associated to this control.
    /// </summary>
    public float? Y
    {
        get => GetValue(YProperty);
        set => SetValue(YProperty, value);
    }

    /// <summary>
    /// Gets or sets the length (in polar coordinate system) of the <see cref="Vector2"/> associated to this control.
    /// </summary>
    public float? Length
    {
        get => GetValue(LengthProperty);
        set => SetValue(LengthProperty, value);
    }

    /// <inheritdoc/>
    protected override void UpdateComponentsFromValue(Vector2? value)
    {
        if (value != null)
        {
            SetCurrentValue(XProperty, value.Value.X);
            SetCurrentValue(YProperty, value.Value.Y);
            SetCurrentValue(LengthProperty, value.Value.Length());
        }
    }

    /// <inheritdoc/>
    protected override Vector2? UpdateValueFromComponent(AvaloniaProperty property)
    {
        switch (EditingMode)
        {
            case VectorEditingMode.Normal:
                if (property == XProperty)
                    return X.HasValue && Value.HasValue ? new Vector2(X.Value, Value.Value.Y) : null;
                if (property == YProperty)
                    return Y.HasValue && Value.HasValue ? new Vector2(Value.Value.X, Y.Value) : null;
                break;

            case VectorEditingMode.AllComponents:
                if (property == XProperty)
                    return X.HasValue ? new Vector2(X.Value) : null;
                if (property == YProperty)
                    return Y.HasValue ? new Vector2(Y.Value) : null;
                break;

            case VectorEditingMode.Length:
                if (property == LengthProperty)
                    return Length.HasValue ? FromLength(Value ?? Vector2.One, Length.Value) : null;
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(EditingMode));
        }

        throw new ArgumentException($"Property {property} is unsupported by method {nameof(UpdateValueFromComponent)} in {EditingMode} mode.");
    }

    /// <inheritdoc/>
    protected override Vector2? UpdateValueFromFloat(float value)
    {
        return new Vector2(value);
    }

    private static Vector2 FromLength(Vector2 value, float length)
    {
        var newValue = value;
        newValue.Normalize();
        newValue *= length;
        return newValue;
    }
}
