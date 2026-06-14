namespace PhotoEquipmentStore.Domain.Entities;

/// <summary>
/// Роль пользователя с описанием прав доступа.
/// </summary>
public class Role
{
    private int id;
    private string name;
    private string description;

    /// <summary>
    /// Уникальный идентификатор роли.
    /// </summary>
    public  int Id { get => id; set => id = value; }
    /// <summary>
    /// Наименование роли.
    /// </summary>
    public string Name { get => name; set => name = value; }
    /// <summary>
    /// Описание прав и назначения роли.
    /// </summary>
    public string Description { get => description; set => description = value; }

    /// <summary>
    /// Создаёт роль с указанными параметрами.
    /// </summary>
    public Role(int id,
        string name,
        string description)
    {
        this.id = id;
        this.name = name;
        this.description = description;
    }
}
