namespace PhotoEquipmentStore.Models;/// <summary>
/// Тип модального уведомления (информация, предупреждение, ошибка).
/// </summary>


public enum NotificationType
{
    /// <summary>Информационное сообщение.</summary>
    Info,
    /// <summary>Предупреждение.</summary>
    Warning,
    /// <summary>Сообщение об ошибке.</summary>
    Error
}
