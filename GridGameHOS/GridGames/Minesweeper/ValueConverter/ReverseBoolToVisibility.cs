﻿using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace GridGameHOS.Minesweeper {
    [ValueConversion(typeof(bool), typeof(BoolToVisibility))]
    public class ReverseBoolToVisibility : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            bool status = (bool)value;
            switch (status) {
                case true:
                    return Visibility.Hidden;
                case false:
                    return Visibility.Visible;
                default:
                    return Visibility.Hidden;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
