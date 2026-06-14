using PhotoEquipmentStore.Application.DTO;

namespace PhotoEquipmentStore.Application.Interfaces;

public interface IImportService
{
    Task<ImportResultDto> ImportAsync(string tableName, string csvFilePath);
}