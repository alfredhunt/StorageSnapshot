﻿using System.Globalization;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;

namespace StorageSnapshot.Converters;

public class NullableToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        var visibility = value != null ? Visibility.Visible : Visibility.Collapsed;
        System.Diagnostics.Debug.WriteLine($"NullableToVisibilityConverter::Convert {visibility}");
        return visibility;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        return value;
    }
}

public class NullableToInverseVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        System.Diagnostics.Debug.WriteLine($"NullableToInverseVisibilityConverter::Convert {value}");
        return value == null ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        return value;
    }
}