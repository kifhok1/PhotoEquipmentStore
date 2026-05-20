using System.Collections.ObjectModel;
using PhotoEquipmentStore.Domain.Entities;
using PhotoEquipmentStore.Infrastructure.Commands;

namespace PhotoEquipmentStore.Application.Services;

public class UsersService
{
    public static ObservableCollection<User> GetUsers()
    {
        return UserCommands.GetUsers();
    }
}