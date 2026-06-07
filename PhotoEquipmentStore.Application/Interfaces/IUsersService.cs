using PhotoEquipmentStore.Application.DTO;
using PhotoEquipmentStore.Domain.Entities;

namespace PhotoEquipmentStore.Application.Interfaces;

public interface IUsersService
{
    UserResultDto GetUsers();
    UserResultDto CreateUser(User user, string password);
    UserResultDto UpdateUser(User user);
    UserResultDto DeleteUser(int userId);
}