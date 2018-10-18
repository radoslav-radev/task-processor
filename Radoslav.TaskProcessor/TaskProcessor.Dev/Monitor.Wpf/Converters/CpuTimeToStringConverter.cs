using System;
using System.Globalization;
using System.Windows.Data;

namespace Radoslav.TaskProcessor
{
    public sealed class CpuTimeToStringConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return string.Empty;
            }

            TimeSpan typedValue = (TimeSpan)value;

            if (typedValue < TimeSpan.FromSeconds(1))
            {
                return Math.Round(typedValue.TotalMilliseconds) + " ms";
            }

            if (typedValue < TimeSpan.FromMinutes(1))
            {
                return Math.Round(typedValue.TotalSeconds) + " s";
            }

            return typedValue.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion IValueConverter Members
    }
}