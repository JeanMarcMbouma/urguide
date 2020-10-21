using System;
using System.Globalization;
using Xamarin.Forms;

namespace UrGuide.Mobile.Converters
{
    class NumericToSocialUnitConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!string.IsNullOrEmpty(value?.ToString()) && float.TryParse(value.ToString(), out float v))
            {
                if (v / Math.Pow(10, 6) >= 1)
                    return $"{v / Math.Pow(10, 6): #.#}M";
                if (v / Math.Pow(10, 3) >= 1)
                    return $"{v / Math.Pow(10, 3): #.#}k";
                return v;
            }
            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
