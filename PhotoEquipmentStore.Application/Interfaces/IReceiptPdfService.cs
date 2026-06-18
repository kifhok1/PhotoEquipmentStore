using PhotoEquipmentStore.Application.DTO;

namespace PhotoEquipmentStore.Application.Interfaces;

/// <summary>
/// Контракт сервиса генерации PDF-чеков.
/// </summary>
public interface IReceiptPdfService
{
    /// <summary>
    /// Формирует и сохраняет PDF-чек по данным заказа.
    /// </summary>
    /// <param name="data">Данные чека.</param>
    /// <param name="filePath">Путь для сохранения файла.</param>
    /// <returns>Флаг успеха и текст ошибки при неудаче.</returns>
    Task<(bool Success, string? Error)> SaveReceiptAsync(ReceiptData data, string filePath);
}
