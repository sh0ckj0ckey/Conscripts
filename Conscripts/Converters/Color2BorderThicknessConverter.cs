using System;
using Conscripts.Models;
using Microsoft.UI.Xaml.Data;

namespace Conscripts.Converters
{
    internal partial class Color2BorderThicknessConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (Enum.TryParse(value?.ToString(), out ShortcutColor color))
            {
                return color == ShortcutColor.Transparent ? new Microsoft.UI.Xaml.Thickness(1) : new Microsoft.UI.Xaml.Thickness(0);
            }
            else
            {
                return new Microsoft.UI.Xaml.Thickness(0);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
