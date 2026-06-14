namespace PhotoEquipmentStore.Models;/// <summary>
/// Элемент выбора темы оформления в настройках.
/// </summary>


public class ThemeSettings
{
    private string theme;

    /// <summary>

    /// Возвращает строковое представление темы.

    /// </summary>

    public override string ToString() => Theme;

    /// <summary>

    /// Название темы оформления.

    /// </summary>

    public string Theme
    {
        get => theme;
        set => theme = value;
    }

    public ThemeSettings(string theme)
    {
        Theme = theme;
    }
}
