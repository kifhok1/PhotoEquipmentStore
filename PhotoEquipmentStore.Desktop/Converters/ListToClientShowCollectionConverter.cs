using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using Avalonia.Data.Converters;
using PhotoEquipmentStore.Models;

namespace PhotoEquipmentStore.Converters;

public class ListToClientShowCollectionConverter : IValueConverter
{
    // Convert вызывается при потоке источник -> цель
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value;
    }

    // ConvertBack вызывается при потоке цель -> источник
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is IList list)
            return new ObservableCollection<ClientShow>(list.Cast<ClientShow>());
        return new ObservableCollection<ClientShow>();
    }
}