using DoushiKatsu;
using Microsoft.Extensions.DependencyModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KanjiUI.Converters;

internal partial class EnumToggleConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        return value.ToString() == parameter.ToString();
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        //if (value is bool isChecked && isChecked && parameter != null)
        //{
        //    return Enum.Parse(targetType, parameter.ToString());
        //}
        return Enum.Parse(targetType, parameter.ToString());
    }
}
