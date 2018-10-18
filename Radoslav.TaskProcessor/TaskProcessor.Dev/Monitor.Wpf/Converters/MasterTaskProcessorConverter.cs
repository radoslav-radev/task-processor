using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Radoslav.TaskProcessor
{
    public sealed class MasterTaskProcessorConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                throw new NotSupportedException();
            }

            if (value.GetType() == typeof(bool))
            {
                if (targetType == typeof(Brush))
                {
                    return new SolidColorBrush((bool)value ? Colors.Black : Colors.Gray);
                }

                if (targetType == typeof(Thickness))
                {
                    return new Thickness((bool)value ? 2 : 1);
                }
            }

            throw new NotSupportedException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        #endregion IValueConverter Members
    }
}