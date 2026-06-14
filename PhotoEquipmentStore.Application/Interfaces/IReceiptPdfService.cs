using PhotoEquipmentStore.Application.DTO;

namespace PhotoEquipmentStore.Application.Interfaces;

public interface IReceiptPdfService
{
    Task<(bool Success, string? Error)> SaveReceiptAsync(ReceiptData data, string filePath);
}
