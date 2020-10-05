using System;
using System.Globalization;
using Xamarin.Forms;

namespace UrGuide.Mobile.Converters
{
    class RatingValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter is int p && value is int v)
                return p <= v;
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
