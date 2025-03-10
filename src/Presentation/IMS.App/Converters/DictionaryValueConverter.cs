﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace IMS.Presentation.Converters;


public class DictionaryValueConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        
        if (value is IEnumerable<ValidationResult> validationErrors && parameter is string key)
        {
            var result = validationErrors.FirstOrDefault(x => x.MemberNames.Contains( key));
            Debug.WriteLine($"Converting key {key} result {result}"); 
            return  result?.ErrorMessage;
        }
      // return "#";

        return null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) => throw new NotImplementedException();
}

