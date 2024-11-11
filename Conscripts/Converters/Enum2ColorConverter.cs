using System;
using System.Collections.Generic;
using Conscripts.Models;
using Microsoft.UI;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;

namespace Conscripts.Converters
{
    internal class Enum2ColorConverter : IValueConverter
    {
        private static Dictionary<ShortcutColorEnum, SolidColorBrush> _shortcutColors = new();
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            try
            {
                ShortcutColorEnum color = (ShortcutColorEnum)value;
                switch (color)
                {
                    case ShortcutColorEnum.Transparent:
                        if (!_shortcutColors.ContainsKey(ShortcutColorEnum.Transparent))
                        {
                            _shortcutColors.Add(ShortcutColorEnum.Transparent, new SolidColorBrush(Colors.Transparent));
                        }
                        return _shortcutColors[ShortcutColorEnum.Transparent];
                    case ShortcutColorEnum.Red:
                        if (!_shortcutColors.ContainsKey(ShortcutColorEnum.Red))
                        {
                            _shortcutColors.Add(ShortcutColorEnum.Red, new SolidColorBrush(Colors.Firebrick));
                        }
                        return _shortcutColors[ShortcutColorEnum.Red];
                    case ShortcutColorEnum.Orange:
                        if (!_shortcutColors.ContainsKey(ShortcutColorEnum.Orange))
                        {
                            _shortcutColors.Add(ShortcutColorEnum.Orange, new SolidColorBrush(Colors.Tomato));
                        }
                        return _shortcutColors[ShortcutColorEnum.Orange];
                    case ShortcutColorEnum.Yellow:
                        if (!_shortcutColors.ContainsKey(ShortcutColorEnum.Yellow))
                        {
                            _shortcutColors.Add(ShortcutColorEnum.Yellow, new SolidColorBrush(Colors.Goldenrod));
                        }
                        return _shortcutColors[ShortcutColorEnum.Yellow];
                    case ShortcutColorEnum.Green:
                        if (!_shortcutColors.ContainsKey(ShortcutColorEnum.Green))
                        {
                            _shortcutColors.Add(ShortcutColorEnum.Green, new SolidColorBrush(Colors.ForestGreen));
                        }
                        return _shortcutColors[ShortcutColorEnum.Green];
                    case ShortcutColorEnum.Blue:
                        if (!_shortcutColors.ContainsKey(ShortcutColorEnum.Blue))
                        {
                            _shortcutColors.Add(ShortcutColorEnum.Blue, new SolidColorBrush(Colors.DodgerBlue));
                        }
                        return _shortcutColors[ShortcutColorEnum.Blue];
                    case ShortcutColorEnum.Purple:
                        if (!_shortcutColors.ContainsKey(ShortcutColorEnum.Purple))
                        {
                            _shortcutColors.Add(ShortcutColorEnum.Purple, new SolidColorBrush(Colors.Orchid));
                        }
                        return _shortcutColors[ShortcutColorEnum.Purple];
                    case ShortcutColorEnum.Pink:
                        if (!_shortcutColors.ContainsKey(ShortcutColorEnum.Pink))
                        {
                            _shortcutColors.Add(ShortcutColorEnum.Pink, new SolidColorBrush(Colors.DeepPink));
                        }
                        return _shortcutColors[ShortcutColorEnum.Pink];
                    case ShortcutColorEnum.Brown:
                        if (!_shortcutColors.ContainsKey(ShortcutColorEnum.Brown))
                        {
                            _shortcutColors.Add(ShortcutColorEnum.Brown, new SolidColorBrush(Colors.Sienna));
                        }
                        return _shortcutColors[ShortcutColorEnum.Brown];
                    case ShortcutColorEnum.Gray:
                        if (!_shortcutColors.ContainsKey(ShortcutColorEnum.Gray))
                        {
                            _shortcutColors.Add(ShortcutColorEnum.Gray, new SolidColorBrush(Colors.DimGray));
                        }
                        return _shortcutColors[ShortcutColorEnum.Gray];
                    default:
                        break;
                }
            }
            catch (Exception ex) { System.Diagnostics.Trace.WriteLine(ex); }
            return new SolidColorBrush(Colors.Transparent);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }
}