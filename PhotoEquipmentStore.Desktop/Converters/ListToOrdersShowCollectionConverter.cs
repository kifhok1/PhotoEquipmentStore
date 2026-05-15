using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using Avalonia.Data.Converters;
using PhotoEquipmentStore.Models;

namespace PhotoEquipmentStore.Converters;

public class ListToOrdersShowCollectionConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is IList list)
            return new ObservableCollection<OrderShow>(list.Cast<OrderShow>());
        return new ObservableCollection<OrderShow>();
    }
}