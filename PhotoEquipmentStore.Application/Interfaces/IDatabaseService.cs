using PhotoEquipmentStore.Application.DTO;

namespace PhotoEquipmentStore.Application.Interfaces;

public interface IDatabaseService
{
    public Task<DataBaseResultDto> CreateBackupAsync(string outputPath);
    public Task<DataBaseResultDto> RestoreStructureAsync(string? sqlFilePath);
    public Task<DataBaseResultDto> ExportTableCsvAsync(string tableName, string outputPath);
    public Task<DataBaseResultDto> ExportTableXlsxAsync(string tableName, string outputPath);
    public Task<DataBaseResultDto> ExportAllTablesAsync(string outputFolderPath, string format);
}
