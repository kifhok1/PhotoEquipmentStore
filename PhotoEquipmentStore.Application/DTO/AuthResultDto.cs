using PhotoEquipmentStore.Domain.Entities;

namespace PhotoEquipmentStore.Application.DTO;

public class AuthResultDto
{
    public bool IsSuccess       { get; init; }
    public string? ErrorMessage { get; init; }
    public UserAuth? User       { get; init; }

    public static AuthResultDto Success(UserAuth user) =>
        new() { IsSuccess = true, User = user };

    public static AuthResultDto Failure(string message) =>
        new() { IsSuccess = false, ErrorMessage = message };

}