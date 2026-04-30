using System;
using System.Collections.Generic;
using Conscripts.Models;
using Microsoft.UI;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;

namespace Conscripts.Converters
{
    internal partial class Enum2ColorConverter : IValueConverter
    {
        private static readonly Dictionary<ShortcutColor, SolidColorBrush> _shortcutColors = [];

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            _ = Enum.TryParse(value?.ToString(), out ShortcutColor color);

            SolidColorBrush? colorBrush;

            switch (color)
            {
                case ShortcutColor.Transparent:
                    if (!_shortcutColors.TryGetValue(ShortcutColor.Transparent, out colorBrush))
                    {
                        colorBrush = new SolidColorBrush(Colors.Transparent);
                        _shortcutColors.Add(ShortcutColor.Transparent, colorBrush);
                    }
                    break;
                case ShortcutColor.Red:
                    if (!_shortcutColors.TryGetValue(ShortcutColor.Red, out colorBrush))
                    {
                        colorBrush = new SolidColorBrush(Colors.Firebrick);
                        _shortcutColors.Add(ShortcutColor.Red, colorBrush);
                    }
                    break;
                case ShortcutColor.Orange:
                    if (!_shortcutColors.TryGetValue(ShortcutColor.Orange, out colorBrush))
                    {
                        colorBrush = new SolidColorBrush(Colors.Tomato);
                        _shortcutColors.Add(ShortcutColor.Orange, colorBrush);
                    }
                    break;
                case ShortcutColor.Yellow:
                    if (!_shortcutColors.TryGetValue(ShortcutColor.Yellow, out colorBrush))
                    {
                        colorBrush = new SolidColorBrush(Colors.Goldenrod);
                        _shortcutColors.Add(ShortcutColor.Yellow, colorBrush);
                    }
                    break;
                case ShortcutColor.Green:
                    if (!_shortcutColors.TryGetValue(ShortcutColor.Green, out colorBrush))
                    {
                        colorBrush = new SolidColorBrush(Colors.ForestGreen);
                        _shortcutColors.Add(ShortcutColor.Green, colorBrush);
                    }
                    break;
                case ShortcutColor.Blue:
                    if (!_shortcutColors.TryGetValue(ShortcutColor.Blue, out colorBrush))
                    {
                        colorBrush = new SolidColorBrush(Colors.DodgerBlue);
                        _shortcutColors.Add(ShortcutColor.Blue, colorBrush);
                    }
                    break;
                case ShortcutColor.Purple:
                    if (!_shortcutColors.TryGetValue(ShortcutColor.Purple, out colorBrush))
                    {
                        colorBrush = new SolidColorBrush(Colors.Orchid);
                        _shortcutColors.Add(ShortcutColor.Purple, colorBrush);
                    }
                    break;
                case ShortcutColor.Pink:
                    if (!_shortcutColors.TryGetValue(ShortcutColor.Pink, out colorBrush))
                    {
                        colorBrush = new SolidColorBrush(Colors.DeepPink);
                        _shortcutColors.Add(ShortcutColor.Pink, colorBrush);
                    }
                    break;
                case ShortcutColor.Brown:
                    if (!_shortcutColors.TryGetValue(ShortcutColor.Brown, out colorBrush))
                    {
                        colorBrush = new SolidColorBrush(Colors.Sienna);
                        _shortcutColors.Add(ShortcutColor.Brown, colorBrush);
                    }
                    break;
                case ShortcutColor.Gray:
                    if (!_shortcutColors.TryGetValue(ShortcutColor.Gray, out colorBrush))
                    {
                        colorBrush = new SolidColorBrush(Colors.DimGray);
                        _shortcutColors.Add(ShortcutColor.Gray, colorBrush);
                    }
                    break;
                default:
                    if (!_shortcutColors.TryGetValue(ShortcutColor.Transparent, out colorBrush))
                    {
                        colorBrush = new SolidColorBrush(Colors.Transparent);
                        _shortcutColors.Add(ShortcutColor.Transparent, colorBrush);
                    }
                    break;
            }

            return colorBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
