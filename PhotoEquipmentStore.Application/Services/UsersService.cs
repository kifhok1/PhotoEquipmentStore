using System.Linq;
using PhotoEquipmentStore.Application.DTO;
using PhotoEquipmentStore.Application.Helpers;
using PhotoEquipmentStore.Application.Interfaces;
using PhotoEquipmentStore.Domain.Entities;
using PhotoEquipmentStore.Infrastructure.Commands;
using PhotoEquipmentStore.Infrastructure.Exceptions;

namespace PhotoEquipmentStore.Application.Services;

public class UsersService : IUsersService
{
    private readonly UserCommands _userCommands  = new();
    private readonly Phone        _phoneCommands = new();

    public UserResultDto GetUsers()
    {
        try
        {
            return UserResultDto.Success(UserCommands.GetUsers());
        }
        catch (DatabaseException)
        {
            return UserResultDto.Failure("Не удалось загрузить список пользователей.");
        }
    }

    public UserResultDto CreateUser(User user, string password)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(password))
                return UserResultDto.Failure("Пароль не может быть пустым.");

            var loginCheck = EnsureLoginIsUnique(user.Login, excludeUserId: null);
            if (!loginCheck.IsSuccess) return loginCheck;

            var phoneCheck = EnsurePhoneIsUnique(user.PhoneNumber, excludeUserId: null);
            if (!phoneCheck.IsSuccess) return phoneCheck;

            user.Image = ImageCompressor.CompressIfNeeded(user.Image);

            string passwordHash = PasswordHasher.ComputeSHA256Hash(password);
            _userCommands.CreateUser(user, passwordHash);
            return UserResultDto.Success(user);
        }
        catch (DatabaseException)
        {
            return UserResultDto.Failure("Не удалось создать пользователя.");
        }
    }

    public UserResultDto UpdateUser(User user)
    {
        try
        {
            var loginCheck = EnsureLoginIsUnique(user.Login, excludeUserId: user.Id);
            if (!loginCheck.IsSuccess) return loginCheck;

            var phoneCheck = EnsurePhoneIsUnique(user.PhoneNumber, excludeUserId: user.Id);
            if (!phoneCheck.IsSuccess) return phoneCheck;

            user.Image = ImageCompressor.CompressIfNeeded(user.Image);

            _userCommands.UpdateUser(user);
            return UserResultDto.Success(user);
        }
        catch (DatabaseException)
        {
            return UserResultDto.Failure("Не удалось обновить данные пользователя.");
        }
    }

    public UserResultDto UpdateUser(User user, string password)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(password))
                return UserResultDto.Failure("Пароль не может быть пустым.");

            var loginCheck = EnsureLoginIsUnique(user.Login, excludeUserId: user.Id);
            if (!loginCheck.IsSuccess) return loginCheck;

            var phoneCheck = EnsurePhoneIsUnique(user.PhoneNumber, excludeUserId: user.Id);
            if (!phoneCheck.IsSuccess) return phoneCheck;

            user.Image = ImageCompressor.CompressIfNeeded(user.Image);

            string passwordHash = PasswordHasher.ComputeSHA256Hash(password);
            _userCommands.UpdateUserWithPassword(user, passwordHash);
            return UserResultDto.Success(user);
        }
        catch (DatabaseException)
        {
            return UserResultDto.Failure("Не удалось обновить данные пользователя.");
        }
    }

    public UserResultDto DeleteUser(int userId)
    {
        try
        {
            _userCommands.DeleteUser(userId);
            return UserResultDto.Success();
        }
        catch (DatabaseException)
        {
            return UserResultDto.Failure("Не удалось удалить пользователя.");
        }
    }

    private UserResultDto EnsureLoginIsUnique(string login, int? excludeUserId)
    {
        try
        {
            if (_userCommands.LoginExists(login, excludeUserId))
                return UserResultDto.Failure($"Логин «{login}» уже занят другим пользователем.");

            return UserResultDto.Success();
        }
        catch (DatabaseException)
        {
            return UserResultDto.Failure("Не удалось проверить уникальность логина.");
        }
    }

    private UserResultDto EnsurePhoneIsUnique(string phone, int? excludeUserId)
    {
        try
        {
            var allPhones = _phoneCommands.GetAllPhoneNumbers();

            if (excludeUserId.HasValue)
            {
                var current = UserCommands.GetUsers()
                    .FirstOrDefault(u => u.Id == excludeUserId.Value);
                if (current is not null)
                    allPhones.Remove(current.PhoneNumber);
            }

            if (allPhones.Contains(phone))
                return UserResultDto.Failure(
                    $"Номер {phone} уже используется другим пользователем или клиентом.");

            return UserResultDto.Success();
        }
        catch (DatabaseException)
        {
            return UserResultDto.Failure("Не удалось проверить уникальность номера телефона.");
        }
    }
}
