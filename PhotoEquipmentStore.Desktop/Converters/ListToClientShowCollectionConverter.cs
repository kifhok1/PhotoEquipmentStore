using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using Avalonia.Data.Converters;
using PhotoEquipmentStore.Models;

namespace PhotoEquipmentStore.Converters;/// <summary>
/// Конвертер списка клиентов в <see cref="ObservableCollection{ClientShow}"/> при обратной привязке.
/// </summary>


public class ListToClientShowCollectionConverter : IValueConverter
{

    /// <summary>

    /// Прямое преобразование значения для привязки.

    /// </summary>

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value;
    }

    /// <summary>

    /// Обратное преобразование значения для привязки.

    /// </summary>

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is IList list)
            return new ObservableCollection<ClientShow>(list.Cast<ClientShow>());
        return new ObservableCollection<ClientShow>();
    }
}
