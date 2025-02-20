using Avalonia.Data.Converters;
using BEAM.Controls;

namespace BEAM.Converter;
using Avalonia.Data.Converters;
using System;
using System.Globalization;

/// <summary>
/// For the ComboBox of the ArgMaxRendererColorHSV, check if the Value is selected
/// </summary>
public class ChannelIsSelectedConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var item = value as ChannelToHSV;
        var selectedItem = parameter as ChannelToHSV;

        return item != null && selectedItem != null && item.Equals(selectedItem);
    }

    /// <summary>
    /// Not used
    /// </summary>
    /// <param name="value"></param>
    /// <param name="targetType"></param>
    /// <param name="parameter"></param>
    /// <param name="culture"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
