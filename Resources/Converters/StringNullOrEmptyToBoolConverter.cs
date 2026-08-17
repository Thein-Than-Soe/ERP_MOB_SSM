using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace CS.ERP_MOB_SSM.Resources.Converters
{
    public class StringNullOrEmptyToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool exists = !string.IsNullOrEmpty(value as string);

            if (parameter != null && parameter.ToString().Equals("Inverse", StringComparison.OrdinalIgnoreCase))
                return !exists;

            return exists;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
