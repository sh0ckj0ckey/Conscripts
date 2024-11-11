using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml;

namespace Conscripts.Converters
{
    internal class StringToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            try
            {
                if (parameter == null)
                {
                    return (string.IsNullOrWhiteSpace(value?.ToString())) ? Visibility.Collapsed : Visibility.Visible;
                }

                if (parameter != null && parameter.ToString() == "!")
                {
                    return (string.IsNullOrWhiteSpace(value?.ToString())) ? Visibility.Visible : Visibility.Collapsed;
                }
            }
            catch { }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }
}
