namespace PhotoEquipmentStore.Models;

public class ReferenceShow
{
    private int id;
    private string title;
    private int count;
    private bool isDeleted;

    public int Id
    {
        get { return id; }
        set { id = value; }
    }
    public string Title
    {
        get { return title; }
        set { title = value; }
    }

    public int Count
    {
        get => count;
        set => count = value;
    }

    public string CountShow
    {
        get => count.ToString() + "шт.";
    }

    public bool IsDeleted
    {
        get { return isDeleted; }
        set { isDeleted = value; }
    }

    public ReferenceShow(
        int id,
        string title,
        int count,
        bool isDelete)
    {
        Id = id;
        Title = title;
        Count = count;
        IsDeleted = isDelete;
    }
}
