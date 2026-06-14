namespace PhotoEquipmentStore.Application.DTO;

/// <summary>
/// Результат проверки подключения к базе данных.
/// </summary>
public class TestConnectionDto
{
    public bool IsConnected { get; init; }
    public string? ErrorMessage { get; init; }

    public static TestConnectionDto Success() =>
        new() { IsConnected = true};

    public static TestConnectionDto Failure(string message) =>
        new() { IsConnected = false, ErrorMessage = message };
}
