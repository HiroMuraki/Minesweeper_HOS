using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace GridGameHOS.Minesweeper {
    [ValueConversion(typeof(int), typeof(SolidColorBrush))]
    public class NearMinesCountToNumberLabel : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            int nearMinesCount = (int)value;
            switch (nearMinesCount) {
                case 0:
                    return new SolidColorBrush(Colors.Transparent);
                case 1:
                    return new SolidColorBrush(Color.FromRgb(0x5A, 0x3B, 0x10));
                case 2:
                    return new SolidColorBrush(Color.FromRgb(0x42, 0x5A, 0x10));
                case 3:
                    return new SolidColorBrush(Color.FromRgb(0x10, 0x41, 0x5A));
                case 4:
                    return new SolidColorBrush(Color.FromRgb(0x10, 0x23, 0x5A));
                case 5:
                    return new SolidColorBrush(Color.FromRgb(0xA2, 0x73, 0x1D));
                case 6:
                    return new SolidColorBrush(Color.FromRgb(0x9C, 0xA2, 0x1D));
                case 7:
                    return new SolidColorBrush(Color.FromRgb(0x5E, 0xA2, 0x1D));
                case 8:
                    return new SolidColorBrush(Color.FromRgb(0x1D, 0x83, 0xA2));
                default:
                    return new SolidColorBrush(Colors.Transparent);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
