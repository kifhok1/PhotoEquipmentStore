namespace PhotoEquipmentStore.Application.DTO;

/// <summary>
/// Результат операции с базой данных (резервное копирование, восстановление, экспорт).
/// </summary>
public class DataBaseResultDto
{
    public bool    IsSuccess    { get; init; }
    public string? ErrorMessage { get; init; }

    public static DataBaseResultDto Success() =>
        new() { IsSuccess = true };

    public static DataBaseResultDto Failure(string message) =>
        new() { IsSuccess = false, ErrorMessage = message };
}
