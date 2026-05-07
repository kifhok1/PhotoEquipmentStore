namespace PhotoEquipmentStore.Application.DTO;

public class TestConnectionDto
{
    public bool IsConnected { get; init; }
    public string? ErrorMessage { get; init; }
    
    public static TestConnectionDto Success() =>
        new() { IsConnected = true};

    public static TestConnectionDto Failure(string message) =>
        new() { IsConnected = false, ErrorMessage = message };
}