namespace PhotoEquipmentStore.Domain.Entities;

/// <summary>
/// Справочная запись (категория, роль, статус заказа и т.д.) с признаком возможности удаления.
/// </summary>
public class Reference
{
    private int id;
    private string name;
    private int count;
    private bool isDeleted;

    /// <summary>
    /// Уникальный идентификатор записи.
    /// </summary>
    public int Id
    {
        get { return id; }
        set { id = value; }
    }

    /// <summary>
    /// Наименование справочной записи.
    /// </summary>
    public string Name
    {
        get { return name; }
        set { name = value; }
    }

    /// <summary>
    /// Количество связанных сущностей, использующих данную запись.
    /// </summary>
    public int Count
    {
        get { return count; }
        set { count = value; }
    }

    /// <summary>
    /// Признак того, что запись можно удалить (нет связанных сущностей).
    /// </summary>
    public bool IsDeleted
    {
        get { return isDeleted; }
        set { isDeleted = value; }
    }

    /// <summary>
    /// Создаёт справочную запись с полным набором полей.
    /// </summary>
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

    /// <summary>
    /// Создаёт справочную запись только с идентификатором и наименованием.
    /// </summary>
    public Reference(
        int id,
        string name)
    {
        Id = id;
        Name = name;
    }
}
