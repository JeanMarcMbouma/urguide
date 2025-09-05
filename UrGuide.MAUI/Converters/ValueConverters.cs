using Microsoft.Maui.Graphics;
using System.Globalization;

namespace UrGuide.MAUI.Converters;

/// <summary>
/// Converts a decimal value to a boolean indicating if it's greater than zero
/// </summary>
public class IsGreaterThanZeroConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is decimal decimalValue)
            return decimalValue > 0;
        
        if (value is double doubleValue)
            return doubleValue > 0;
        
        if (value is int intValue)
            return intValue > 0;
        
        return false;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// Calculates the difference between NewBudget and CurrentBudget
/// </summary>
public class BudgetDifferenceConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is UrGuide.MAUI.ViewModels.UpdateBudgetViewModel viewModel)
        {
            return viewModel.NewBudget - viewModel.CurrentBudget;
        }
        return 0m;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// Converts a string to a boolean indicating if it's not null or empty
/// </summary>
public class StringIsNotNullOrEmptyConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return !string.IsNullOrEmpty(value?.ToString());
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// Inverts a boolean value
/// </summary>
public class InvertedBoolConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool boolValue)
            return !boolValue;
        
        return true;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool boolValue)
            return !boolValue;
        
        return false;
    }
}