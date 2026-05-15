namespace PhotoEquipmentStore.Models;

public class PageButton
{
    public int?  PageNumber { get; init; }
    public bool  IsCurrent  { get; init; }
    public bool  IsEllipsis => PageNumber is null;

    public static PageButton Of(int n, bool current) => new() { PageNumber = n, IsCurrent = current };
    public static PageButton Dots()                   => new() { PageNumber = null };

}