using System;
using Microsoft.UI.Xaml.Data;

namespace Conscripts.Converters
{
    internal partial class Bool2VisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            bool boolValue = value is bool b && b;

            if (parameter?.ToString() == "!")
            {
                boolValue = !boolValue;
            }

            return boolValue ? Microsoft.UI.Xaml.Visibility.Visible : Microsoft.UI.Xaml.Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
