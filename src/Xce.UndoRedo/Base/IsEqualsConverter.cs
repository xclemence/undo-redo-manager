﻿using System;
using System.Globalization;
using System.Windows.Data;

namespace Xce.UndoRedo.Base
{
    public class IsEqualsConverter : IValueConverter
    {
        private static IValueConverter instance;

        public static IValueConverter Instance => instance ?? (instance = new IsEqualsConverter());

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => value?.Equals(parameter) ?? parameter == null;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
