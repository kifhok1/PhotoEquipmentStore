namespace PhotoEquipmentStore.Domain.Entities;

public class Reference
{
    private int id;
    private string name;
    private int count;
    private bool isDeleted;

    public int Id
    {
        get { return id; }
        set { id = value; }
    }
    
    public string Name
    {
        get { return name; }
        set { name = value; }
    }

    public int Count
    {
        get { return count; }
        set { count = value; }
    }

    public bool IsDeleted
    {
        get { return isDeleted; }
        set { isDeleted = value; }
    }
    
    public Reference(
        int id,
        string name,
        int count,
        bool isDeleted)
    {
        Id = id;
        Name = name;
        Count = count; 
        IsDeleted = isDeleted;
    }
    
    public Reference(
        int id,
        string name)
    {
        Id = id;
        Name = name;
    }
}