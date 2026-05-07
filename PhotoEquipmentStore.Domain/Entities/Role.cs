namespace PhotoEquipmentStore.Domain.Entities;

public class Role
{
    private int id;
    private string name;
    private string description;
    
    public  int Id { get => id; set => id = value; }
    public string Name { get => name; set => name = value; }
    public string Description { get => description; set => description = value; }

    public Role(int id,
        string name,
        string description)
    {
        this.id = id;
        this.name = name;
        this.description = description;
    }
}