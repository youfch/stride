// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

#if AVALONIA

using System;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace Stride.GameStudio.View.Panels
{
    /// <summary>
    /// Converts a boolean to a rotation angle (0 or 90 degrees).
    /// </summary>
    public class BoolToAngleConverter : IValueConverter
    {
        public static readonly BoolToAngleConverter Instance = new();

        public object? Convert(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
        {
            if (value is bool b)
            {
                return b ? 90.0 : 0.0;
            }
            return 0.0;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

#endif
