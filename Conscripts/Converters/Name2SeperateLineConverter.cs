using System;
using Conscripts.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;

namespace Conscripts.Converters
{
    internal class Name2SeperateLineConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            try
            {
                if (value?.ToString() == MainViewModel.SeperateLineSpecialCategoryName)
                {
                    return parameter?.ToString() == "!" ? Visibility.Collapsed : Visibility.Visible;
                }
                else
                {
                    return parameter?.ToString() == "!" ? Visibility.Visible : Visibility.Collapsed;
                }
            }
            catch (Exception ex) { System.Diagnostics.Trace.WriteLine(ex); }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }
}
