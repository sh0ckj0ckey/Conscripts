﻿using System;
using Microsoft.UI.Xaml.Data;

namespace Conscripts.Converters
{
    internal class BoolReverseConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            try
            {
                if (value != null)
                {
                    return bool.Parse(value?.ToString() ?? "True") ? false : true;
                }
            }
            catch { }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }
}
