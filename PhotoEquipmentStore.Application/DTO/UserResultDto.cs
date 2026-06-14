using System.Collections.Generic;
using System.Linq;
using PhotoEquipmentStore.Domain.Entities;

namespace PhotoEquipmentStore.Application.DTO;

public class UserResultDto
{
    public bool IsSuccess      { get; init; }
    public string? ErrorMessage { get; init; }
    public User? User          { get; init; }
    public List<User> Users    { get; init; } = [];

    public static UserResultDto Success(IEnumerable<User> users) =>
        new() { IsSuccess = true, Users = users.ToList() };

    public static UserResultDto Success(User user) =>
        new() { IsSuccess = true, User = user };

    public static UserResultDto Success() =>
        new() { IsSuccess = true };

    public static UserResultDto Failure(string message) =>
        new() { IsSuccess = false, ErrorMessage = message };
}
