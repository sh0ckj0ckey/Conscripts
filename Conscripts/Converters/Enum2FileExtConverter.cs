using System;
using Conscripts.Models;
using Microsoft.UI.Xaml.Data;

namespace Conscripts.Converters
{
    internal class Enum2FileExtConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            try
            {
                ShortcutTypeEnum type = (ShortcutTypeEnum)value;
                switch (type)
                {
                    case ShortcutTypeEnum.Ps1:
                        return ".ps1";
                    case ShortcutTypeEnum.Bat:
                        return ".bat";
                }
            }
            catch { }
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }

}
