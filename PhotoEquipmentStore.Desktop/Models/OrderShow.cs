namespace PhotoEquipmentStore.Models;

public class OrderShow : IPaginationShow
{
    private int id;

    public int Id
    {
        get => id; 
        set => id = value;
    }
}