using Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace TwoZeroFourEightLite {
    [ValueConversion(typeof(int), typeof(ImageBrush))]
    public class NumberToIconType : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            int number = (int)value;
            switch (number) {
                case 2:
                case 4:
                    return ResDict.PreSetting["S1"] as ImageBrush;
                case 8:
                case 16:
                    return ResDict.PreSetting["S2"] as ImageBrush;
                case 32:
                case 64:
                    return ResDict.PreSetting["S3"] as ImageBrush;
                case 128:
                case 256:
                    return ResDict.PreSetting["S4"] as ImageBrush;
                case 512:
                case 1024:
                    return ResDict.PreSetting["S5"] as ImageBrush;
                case 2048:
                case 4096:
                    return ResDict.PreSetting["S6"] as ImageBrush;
                default:
                    return null;
            }
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
