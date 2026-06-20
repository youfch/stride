using System;
using Avalonia;
using Avalonia.Controls;
using Stride.Core.Mathematics;

namespace Stride.Core.Presentation.Controls;

/// <summary>
/// Editor control for angle values stored in radians but displayed in degrees.
/// </summary>
public class AngleEditor : VectorEditorBase<float?>
{
    /// <summary>
    /// Identifies the <see cref="Degrees"/> styled property.
    /// </summary>
    public static readonly StyledProperty<float> DegreesProperty =
        AvaloniaProperty.Register<AngleEditor, float>(nameof(Degrees), defaultValue: 0f);

    static AngleEditor()
    {
        DegreesProperty.Changed.AddClassHandler<AngleEditor>((editor, e) => editor.OnComponentPropertyChanged(DegreesProperty));
    }

    /// <summary>
    /// Gets or sets the angle in degrees.
    /// </summary>
    public float Degrees
    {
        get => GetValue(DegreesProperty);
        set => SetValue(DegreesProperty, value);
    }

    /// <inheritdoc/>
    public override void ResetValue()
    {
        Value = DefaultValue;
    }

    /// <inheritdoc/>
    protected override void UpdateComponentsFromValue(float? value)
    {
        if (value.HasValue)
        {
            var degrees = GetDisplayValue(value.Value);
            SetCurrentValue(DegreesProperty, degrees);
        }
    }

    /// <inheritdoc/>
    protected override float? UpdateValueFromComponent(AvaloniaProperty property)
    {
        if (property == DegreesProperty)
        {
            return MathUtil.DegreesToRadians(Degrees);
        }

        throw new ArgumentException($"Property {property} is unsupported by method {nameof(UpdateValueFromComponent)}.");
    }

    /// <inheritdoc/>
    protected override float? UpdateValueFromFloat(float value)
    {
        return MathUtil.DegreesToRadians(value);
    }

    /// <summary>
    /// Converts radians to degrees for display.
    /// </summary>
    /// <param name="angleRadians">The angle in radians.</param>
    /// <returns>The angle in degrees.</returns>
    private static float GetDisplayValue(float angleRadians)
    {
        return MathF.Round(MathUtil.RadiansToDegrees(angleRadians), 4);
    }
}
