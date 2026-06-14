namespace PhotoEquipmentStore.Models;/// <summary>
/// Результат выбора пользователя в диалоге уведомления.
/// </summary>


public enum NotificationResult
{
    /// <summary>Одна кнопка подтверждения.</summary>
    Ok,
    /// <summary>Пользователь выбрал «Да».</summary>
    Yes,
    /// <summary>Пользователь выбрал «Нет».</summary>
    No,
    /// <summary>Пользователь отменил действие.</summary>
    Cancel
}
