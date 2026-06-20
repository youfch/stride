using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Stride.Core.Mathematics;

namespace Stride.Core.Presentation.Controls;

/// <summary>
/// A color picker control that allows selecting colors via HSV and RGB values.
/// </summary>
public class ColorPicker : AvaloniaObject
{
    /// <summary>
    /// Identifies the <see cref="Color"/> styled property.
    /// </summary>
    public static readonly StyledProperty<Color4> ColorProperty =
        AvaloniaProperty.Register<ColorPicker, Color4>(nameof(Color), defaultValue: Color4.White);

    /// <summary>
    /// Identifies the <see cref="Hue"/> styled property.
    /// </summary>
    public static readonly StyledProperty<float> HueProperty =
        AvaloniaProperty.Register<ColorPicker, float>(nameof(Hue), defaultValue: 0f);

    /// <summary>
    /// Identifies the <see cref="Saturation"/> styled property.
    /// </summary>
    public static readonly StyledProperty<float> SaturationProperty =
        AvaloniaProperty.Register<ColorPicker, float>(nameof(Saturation), defaultValue: 0f);

    /// <summary>
    /// Identifies the <see cref="Brightness"/> styled property.
    /// </summary>
    public static readonly StyledProperty<float> BrightnessProperty =
        AvaloniaProperty.Register<ColorPicker, float>(nameof(Brightness), defaultValue: 0f);

    /// <summary>
    /// Identifies the <see cref="Red"/> styled property.
    /// </summary>
    public static readonly StyledProperty<byte> RedProperty =
        AvaloniaProperty.Register<ColorPicker, byte>(nameof(Red), defaultValue: 0);

    /// <summary>
    /// Identifies the <see cref="Green"/> styled property.
    /// </summary>
    public static readonly StyledProperty<byte> GreenProperty =
        AvaloniaProperty.Register<ColorPicker, byte>(nameof(Green), defaultValue: 0);

    /// <summary>
    /// Identifies the <see cref="Blue"/> styled property.
    /// </summary>
    public static readonly StyledProperty<byte> BlueProperty =
        AvaloniaProperty.Register<ColorPicker, byte>(nameof(Blue), defaultValue: 0);

    /// <summary>
    /// Identifies the <see cref="Alpha"/> styled property.
    /// </summary>
    public static readonly StyledProperty<byte> AlphaProperty =
        AvaloniaProperty.Register<ColorPicker, byte>(nameof(Alpha), defaultValue: 255);

    /// <summary>
    /// Identifies the <see cref="ShowAlpha"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> ShowAlphaProperty =
        AvaloniaProperty.Register<ColorPicker, bool>(nameof(ShowAlpha), defaultValue: true);

    static ColorPicker()
    {
        ColorProperty.Changed.AddClassHandler<ColorPicker>((picker, e) => picker.OnColorChanged());
        HueProperty.Changed.AddClassHandler<ColorPicker>((picker, e) => picker.OnHSVPropertyChanged());
        SaturationProperty.Changed.AddClassHandler<ColorPicker>((picker, e) => picker.OnHSVPropertyChanged());
        BrightnessProperty.Changed.AddClassHandler<ColorPicker>((picker, e) => picker.OnHSVPropertyChanged());
        RedProperty.Changed.AddClassHandler<ColorPicker>((picker, e) => picker.OnRGBAPropertyChanged());
        GreenProperty.Changed.AddClassHandler<ColorPicker>((picker, e) => picker.OnRGBAPropertyChanged());
        BlueProperty.Changed.AddClassHandler<ColorPicker>((picker, e) => picker.OnRGBAPropertyChanged());
        AlphaProperty.Changed.AddClassHandler<ColorPicker>((picker, e) => picker.OnRGBAPropertyChanged());
    }

    private bool _interlock;
    private ColorHSV _internalColor;

    /// <summary>
    /// Gets or sets the color associated to this color picker.
    /// </summary>
    public Color4 Color
    {
        get => GetValue(ColorProperty);
        set => SetValue(ColorProperty, value);
    }

    /// <summary>
    /// Gets or sets the hue of the color associated to this color picker.
    /// </summary>
    public float Hue
    {
        get => GetValue(HueProperty);
        set => SetValue(HueProperty, value);
    }

    /// <summary>
    /// Gets or sets the saturation of the color associated to this color picker.
    /// </summary>
    public float Saturation
    {
        get => GetValue(SaturationProperty);
        set => SetValue(SaturationProperty, value);
    }

    /// <summary>
    /// Gets or sets the brightness of the color associated to this color picker.
    /// </summary>
    public float Brightness
    {
        get => GetValue(BrightnessProperty);
        set => SetValue(BrightnessProperty, value);
    }

    /// <summary>
    /// Gets or sets the red component of the color associated to this color picker.
    /// </summary>
    public byte Red
    {
        get => GetValue(RedProperty);
        set => SetValue(RedProperty, value);
    }

    /// <summary>
    /// Gets or sets the green component of the color associated to this color picker.
    /// </summary>
    public byte Green
    {
        get => GetValue(GreenProperty);
        set => SetValue(GreenProperty, value);
    }

    /// <summary>
    /// Gets or sets the blue component of the color associated to this color picker.
    /// </summary>
    public byte Blue
    {
        get => GetValue(BlueProperty);
        set => SetValue(BlueProperty, value);
    }

    /// <summary>
    /// Gets or sets the alpha component of the color associated to this color picker.
    /// </summary>
    public byte Alpha
    {
        get => GetValue(AlphaProperty);
        set => SetValue(AlphaProperty, value);
    }

    /// <summary>
    /// Gets or sets whether the alpha component of the color should be displayed in the color picker.
    /// </summary>
    public bool ShowAlpha
    {
        get => GetValue(ShowAlphaProperty);
        set => SetValue(ShowAlphaProperty, value);
    }

    public ColorPicker()
    {
        _internalColor = ColorHSV.FromColor(Color);
    }

    private void OnColorChanged()
    {
        bool isInitializing = _internalColor.Equals(ColorHSV.FromColor(Color));
        if (isInitializing) return;

        if (_interlock) return;

        _interlock = true;
        _internalColor = ColorHSV.FromColor(Color);
        var colorRGBA = _internalColor.ToColor();

        SetCurrentValue(RedProperty, (byte)Math.Round(colorRGBA.R * 255.0f));
        SetCurrentValue(GreenProperty, (byte)Math.Round(colorRGBA.G * 255.0f));
        SetCurrentValue(BlueProperty, (byte)Math.Round(colorRGBA.B * 255.0f));
        SetCurrentValue(AlphaProperty, (byte)Math.Round(colorRGBA.A * 255.0f));

        SetCurrentValue(HueProperty, _internalColor.H);
        SetCurrentValue(SaturationProperty, _internalColor.S * 100.0f);
        SetCurrentValue(BrightnessProperty, _internalColor.V * 100.0f);
        _interlock = false;
    }

    private void OnHSVPropertyChanged()
    {
        if (_interlock) return;

        _interlock = true;
        _internalColor = new ColorHSV(Hue, Saturation / 100.0f, Brightness / 100.0f, Alpha / 255.0f);
        var colorRGBA = _internalColor.ToColor();

        SetCurrentValue(RedProperty, (byte)Math.Round(colorRGBA.R * 255.0f));
        SetCurrentValue(GreenProperty, (byte)Math.Round(colorRGBA.G * 255.0f));
        SetCurrentValue(BlueProperty, (byte)Math.Round(colorRGBA.B * 255.0f));
        SetCurrentValue(AlphaProperty, (byte)Math.Round(colorRGBA.A * 255.0f));

        SetCurrentValue(ColorProperty, colorRGBA);
        _interlock = false;
    }

    private void OnRGBAPropertyChanged()
    {
        if (_interlock) return;

        _interlock = true;
        var colorRGBA = new Color4(Red / 255.0f, Green / 255.0f, Blue / 255.0f, Alpha / 255.0f);
        _internalColor = ColorHSV.FromColor(colorRGBA);

        SetCurrentValue(HueProperty, _internalColor.H);
        SetCurrentValue(SaturationProperty, _internalColor.S * 100.0f);
        SetCurrentValue(BrightnessProperty, _internalColor.V * 100.0f);

        SetCurrentValue(ColorProperty, colorRGBA);
        _interlock = false;
    }
}
