using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace TrayManager.Infrastructure.Wpf;

public class InstanceCountVisibilityConverter : IValueConverter
{
    public static readonly InstanceCountVisibilityConverter Instance = new();

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is int count && count > 1 ? Visibility.Visible : Visibility.Collapsed;

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
