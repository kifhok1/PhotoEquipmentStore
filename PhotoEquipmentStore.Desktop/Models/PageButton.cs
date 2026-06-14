namespace PhotoEquipmentStore.Models;/// <summary>
/// Модель кнопки страницы в компоненте пагинации.
/// </summary>


public class PageButton
{
    /// <summary>
    /// Номер страницы или null для разделителя.
    /// </summary>
    public int?  PageNumber { get; init; }
    /// <summary>
    /// Признак текущей активной страницы.
    /// </summary>
    public bool  IsCurrent  { get; init; }
    /// <summary>
    /// Признак кнопки-разделителя «…».
    /// </summary>
    public bool  IsEllipsis => PageNumber is null;

    /// <summary>

    /// Создаёт кнопку страницы с номером.

    /// </summary>

    public static PageButton Of(int n, bool current) => new() { PageNumber = n, IsCurrent = current };
    /// <summary>
    /// Создаёт кнопку-разделитель «…» в пагинации.
    /// </summary>
    public static PageButton Dots()                   => new() { PageNumber = null };

}
