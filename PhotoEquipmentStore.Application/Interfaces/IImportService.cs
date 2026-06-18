using PhotoEquipmentStore.Application.DTO;

namespace PhotoEquipmentStore.Application.Interfaces;

/// <summary>
/// Контракт сервиса импорта данных из CSV-файлов.
/// </summary>
public interface IImportService
{
    /// <summary>
    /// Импортирует данные из CSV-файла в указанную таблицу.
    /// </summary>
    /// <param name="tableName">Имя целевой таблицы.</param>
    /// <param name="csvFilePath">Путь к CSV-файлу.</param>
    /// <returns>Результат импорта с количеством обработанных и пропущенных строк.</returns>
    Task<ImportResultDto> ImportAsync(string tableName, string csvFilePath);
}
