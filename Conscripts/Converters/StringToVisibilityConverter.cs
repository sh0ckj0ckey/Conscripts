using System;
using Microsoft.UI.Xaml.Data;

namespace Conscripts.Converters
{
    internal partial class StringToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            bool visible = !string.IsNullOrWhiteSpace(value?.ToString());

            if (parameter?.ToString() == "!")
            {
                visible = !visible;
            }

            return visible ? Microsoft.UI.Xaml.Visibility.Visible : Microsoft.UI.Xaml.Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
