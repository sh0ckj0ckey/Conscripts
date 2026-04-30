using System;
using Conscripts.Models;
using Microsoft.UI.Xaml.Data;

namespace Conscripts.Converters
{
    internal partial class Enum2FileExtConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            _ = Enum.TryParse(value?.ToString(), out ShortcutType type);
            return type switch
            {
                ShortcutType.Ps1 => ".ps1",
                ShortcutType.Bat => ".bat",
                _ => string.Empty,
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
