using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using TeploenergoSchedule.Model.FileInfo;

namespace TeploenergoSchedule.Infrastructure.Convertors;

internal class FileStateToColorConvertor : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not FileStateEnum state) return null;

        return state switch
        {
            FileStateEnum.None => Brushes.Black,
            FileStateEnum.Processing => Brushes.DarkGray,
            FileStateEnum.CorrectedSuccess => Brushes.Green,
            FileStateEnum.CorrectedWithWarning => Brushes.Peru,
            FileStateEnum.Error => Brushes.Red,
            _ => Brushes.DarkGray
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}