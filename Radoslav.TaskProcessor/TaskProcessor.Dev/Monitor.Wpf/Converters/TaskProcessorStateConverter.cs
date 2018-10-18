using System;
using System.Globalization;
using System.Windows.Data;
using Radoslav.TaskProcessor.Model;

namespace Radoslav.TaskProcessor
{
    public sealed class TaskProcessorStateConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            TaskProcessorState allowedValue = (TaskProcessorState)Enum.Parse(typeof(TaskProcessorState), (string)parameter);

            return (TaskProcessorState)value == allowedValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        #endregion IValueConverter Members
    }
}