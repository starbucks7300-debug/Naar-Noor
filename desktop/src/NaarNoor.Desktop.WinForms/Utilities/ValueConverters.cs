namespace NaarNoor.Desktop.WinForms.Utilities
{
    /// <summary>
    /// Value converter interface for data binding transformations.
    /// </summary>
    public interface IValueConverter
    {
        object? Convert(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo? culture);
        object? ConvertBack(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo? culture);
    }

    /// <summary>
    /// Converter to make elements visible only when ErrorMessage is not null/empty.
    /// </summary>
    public class ErrorMessageToVisibleConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo? culture)
        {
            return !string.IsNullOrEmpty(value as string);
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo? culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Converter to invert boolean values for button enabled state.
    /// </summary>
    public class InverseBooleanConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo? culture)
        {
            return !(value is bool b && b);
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo? culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Converter to format decimal values as currency.
    /// </summary>
    public class DecimalToCurrencyConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo? culture)
        {
            if (value is decimal d)
            {
                return d.ToString("C2");
            }
            return "$0.00";
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo? culture)
        {
            throw new NotImplementedException();
        }
    }
}
