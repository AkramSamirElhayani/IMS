using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace IMS.Presentation.Converters;


public class DictionaryValueConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        Debug.WriteLine($"Converting Value {value} parameter {parameter}");
        if (value is Dictionary<string, List<string>> dictionary && parameter is string key)
        {
            var result = dictionary.TryGetValue(key, out var list) ? list : null;
            Debug.WriteLine($"Converting key {key} result {result}");


            return result;
        }
        return "#";

        return null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) => throw new NotImplementedException();
}

