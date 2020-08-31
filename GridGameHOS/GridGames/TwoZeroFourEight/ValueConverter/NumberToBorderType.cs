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
    public class NumberToBorderType : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            int number = (int)value;
            switch (number) {
                case 2:
                case 8:
                case 32:
                case 128:
                case 512:
                case 2048:
                    return ResDict.PreSetting["Border_A"] as ImageBrush;
                case 4:
                case 16:
                case 64:
                case 256:
                case 1024:
                case 4096:
                    return ResDict.PreSetting["Border_B"] as ImageBrush;
                default:
                    return null;
            }
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
