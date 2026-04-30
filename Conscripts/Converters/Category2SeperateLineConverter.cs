using System;
using Microsoft.UI.Xaml.Data;

namespace Conscripts.Converters
{
    internal partial class Category2SeperateLineConverter : IValueConverter
    {
        private const string SeperateLineSpecialCategoryName = "376C50B1-B7C1-4E7C-874A-F743DD80D95F";

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            bool visible = SeperateLineSpecialCategoryName == value?.ToString();

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
