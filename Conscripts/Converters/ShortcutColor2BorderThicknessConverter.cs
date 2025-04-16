using System;
using Conscripts.ViewModels;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml;
using Conscripts.Models;

namespace Conscripts.Converters
{
    internal class ShortcutColor2BorderThicknessConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            try
            {
                return value?.ToString() == ShortcutColorEnum.Transparent.ToString() ? new Thickness(1) : new Thickness(0);
            }
            catch (Exception ex) { System.Diagnostics.Trace.WriteLine(ex); }
            return new Thickness(0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }
}
