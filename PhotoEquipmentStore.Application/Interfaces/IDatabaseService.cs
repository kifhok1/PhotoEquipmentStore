using PhotoEquipmentStore.Application.DTO;

namespace PhotoEquipmentStore.Application.Interfaces;

/// <summary>
/// Контракт сервиса операций с базой данных (резервное копирование, восстановление, экспорт).
/// </summary>
public interface IDatabaseService
{
    /// <summary>Создаёт резервную копию базы данных.</summary>
    /// <param name="outputPath">Путь для сохранения дампа.</param>
    public Task<DataBaseResultDto> CreateBackupAsync(string outputPath);

    /// <summary>Восстанавливает структуру базы данных из SQL-файла.</summary>
    /// <param name="sqlFilePath">Путь к SQL-файлу; при null используется файл по умолчанию.</param>
    public Task<DataBaseResultDto> RestoreStructureAsync(string? sqlFilePath);

    /// <summary>Экспортирует таблицу в CSV-файл.</summary>
    /// <param name="tableName">Имя таблицы.</param>
    /// <param name="outputPath">Путь для сохранения файла.</param>
    public Task<DataBaseResultDto> ExportTableCsvAsync(string tableName, string outputPath);

    /// <summary>Экспортирует таблицу в XLSX-файл.</summary>
    /// <param name="tableName">Имя таблицы.</param>
    /// <param name="outputPath">Путь для сохранения файла.</param>
    public Task<DataBaseResultDto> ExportTableXlsxAsync(string tableName, string outputPath);

    /// <summary>Экспортирует все таблицы в ZIP-архив указанного формата.</summary>
    /// <param name="outputFolderPath">Папка для сохранения архива.</param>
    /// <param name="format">Формат файлов: csv или xlsx.</param>
    public Task<DataBaseResultDto> ExportAllTablesAsync(string outputFolderPath, string format);
}
