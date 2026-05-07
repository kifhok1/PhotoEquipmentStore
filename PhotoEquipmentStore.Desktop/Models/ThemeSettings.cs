namespace PhotoEquipmentStore.Models;

public class ThemeSettings
{
    private string theme;
    
    public override string ToString() => Theme;

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