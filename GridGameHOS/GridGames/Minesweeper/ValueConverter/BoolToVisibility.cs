﻿using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace GridGameHOS.Minesweeper {
    [ValueConversion(typeof(bool), typeof(BoolToVisibility))]
    public class BoolToVisibility : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            bool status = (bool)value;
            switch (status) {
                case true:
                    return Visibility.Visible;
                case false:
                    return Visibility.Hidden;
                default:
                    return Visibility.Visible;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
