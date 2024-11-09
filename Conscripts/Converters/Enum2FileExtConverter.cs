using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI;
using Conscripts.Models;

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
