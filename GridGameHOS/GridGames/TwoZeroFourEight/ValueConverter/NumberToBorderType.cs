﻿using GridGameHOS.Common;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace GridGameHOS.TwoZeroFourEightLite {
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
