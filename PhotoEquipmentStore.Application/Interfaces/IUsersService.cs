using PhotoEquipmentStore.Application.DTO;
using PhotoEquipmentStore.Domain.Entities;

namespace PhotoEquipmentStore.Application.Interfaces;

/// <summary>
/// Контракт сервиса управления пользователями системы.
/// </summary>
public interface IUsersService
{
    /// <summary>Возвращает список всех пользователей.</summary>
    UserResultDto GetUsers();

    /// <summary>Создаёт нового пользователя с указанным паролем.</summary>
    /// <param name="user">Данные пользователя.</param>
    /// <param name="password">Пароль в открытом виде.</param>
    UserResultDto CreateUser(User user, string password);

    /// <summary>Обновляет данные существующего пользователя.</summary>
    /// <param name="user">Обновлённые данные пользователя.</param>
    UserResultDto UpdateUser(User user);

    /// <summary>Удаляет пользователя по идентификатору.</summary>
    /// <param name="userId">Идентификатор пользователя.</param>
    UserResultDto DeleteUser(int userId);
}
