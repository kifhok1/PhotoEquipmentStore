using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace PhotoEquipmentStore.Converters;/// <summary>
/// Конвертер между <see cref="DateTimeOffset"/> и <see cref="DateTime"/> для привязок дат.
/// </summary>


public class DateTimeOffsetToDateTimeConverter : IValueConverter
{
    public static readonly DateTimeOffsetToDateTimeConverter Instance = new();

    /// <summary>

    /// Прямое преобразование значения для привязки.

    /// </summary>

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is DateTimeOffset dto ? dto.DateTime : (DateTime?)null;

    /// <summary>

    /// Обратное преобразование значения для привязки.

    /// </summary>

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is DateTime dt ? new DateTimeOffset(dt) : (DateTimeOffset?)null;
}
