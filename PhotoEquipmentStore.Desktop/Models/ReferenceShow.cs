namespace PhotoEquipmentStore.Models;/// <summary>
/// Модель записи справочника для списка и формы редактирования.
/// </summary>


public class ReferenceShow
{
    private int id;
    private string title;
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
    /// Заголовок или наименование записи.
    /// </summary>
    public string Title
    {
        get { return title; }
        set { title = value; }
    }

    /// <summary>

    /// Количество связанных записей.

    /// </summary>

    public int Count
    {
        get => count;
        set => count = value;
    }

    /// <summary>

    /// Форматированное отображение количества.

    /// </summary>

    public string CountShow
    {
        get => count.ToString() + "шт.";
    }

    /// <summary>

    /// Признак удалённой записи справочника.

    /// </summary>

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
