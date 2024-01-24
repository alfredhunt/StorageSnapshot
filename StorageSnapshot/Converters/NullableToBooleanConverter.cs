using System.Globalization;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;

namespace StorageSnapshot.Converters;

public class NullableToBooleanConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        System.Diagnostics.Debug.WriteLine($"NullableToBooleanConverter::Convert {value}");
        return value != null ? false : true;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        return value;
    }
}
