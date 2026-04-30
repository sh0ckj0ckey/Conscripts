using System;
using Microsoft.UI.Xaml.Data;

namespace Conscripts.Converters
{
    internal partial class Int2VisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            int intValue = value is int i ? i : 0;
            bool visible = intValue > 0;

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
