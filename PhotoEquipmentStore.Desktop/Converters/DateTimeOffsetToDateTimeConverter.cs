using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace PhotoEquipmentStore.Converters;

public class DateTimeOffsetToDateTimeConverter : IValueConverter
{
    public static readonly DateTimeOffsetToDateTimeConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is DateTimeOffset dto ? dto.DateTime : (DateTime?)null;

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is DateTime dt ? new DateTimeOffset(dt) : (DateTimeOffset?)null;
}
