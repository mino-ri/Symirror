using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace Symirror3;

internal class MinimumConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        double[] doubles = [.. values.OfType<double>().Where(x => x > 0)];
        return doubles.Length == 0 ? Binding.DoNothing : doubles.Min();
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
