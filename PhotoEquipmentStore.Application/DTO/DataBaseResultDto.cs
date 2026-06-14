namespace PhotoEquipmentStore.Application.DTO;

public class DataBaseResultDto
{
    public bool    IsSuccess    { get; init; }
    public string? ErrorMessage { get; init; }

    public static DataBaseResultDto Success() =>
        new() { IsSuccess = true };

    public static DataBaseResultDto Failure(string message) =>
        new() { IsSuccess = false, ErrorMessage = message };
}
