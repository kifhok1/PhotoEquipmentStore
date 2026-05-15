namespace PhotoEquipmentStore.Models;

public class ReferenceShow
{
    private int id;
    private string title;
    private int count;

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
    
    public string Count
    {
        get => count.ToString() + "шт.";
        set => count = int.Parse(value);
    }

    public ReferenceShow(
        int id,
        string title,
        int count)
    {
        Id = id;
        Title = title;
        Count = count.ToString();
    }
}