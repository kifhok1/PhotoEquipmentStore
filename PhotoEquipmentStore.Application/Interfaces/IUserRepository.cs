using PhotoEquipmentStore.Domain.Entities;

namespace PhotoEquipmentStore.Application.Interfaces;

public interface IUserRepository
{
    UserAuth? GetByLogin(string login);
}