using System;
using PhotoEquipmentStore.Application.DTO;
using PhotoEquipmentStore.Application.Helpers;
using Authorization = PhotoEquipmentStore.Infrastructure.Commands.Authorization;

namespace PhotoEquipmentStore.Application.Services;

/// <summary>
/// Сервис аутентификации пользователей по логину и паролю.
/// </summary>
public class AuthorizationService
{
    /// <summary>
    /// Проверяет учётные данные пользователя и возвращает результат входа.
    /// </summary>
    /// <param name="login">Логин пользователя.</param>
    /// <param name="password">Пароль в открытом виде.</param>
    /// <returns>Результат аутентификации с данными пользователя или сообщением об ошибке.</returns>
    public static AuthResultDto Authenticate(string login, string password)
    {
        if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
        {
            return AuthResultDto.Failure("Заполните все поля.");
        }

        try
        {
            var user = Authorization.GetUser(login);

            if (user is null)
            {
                return AuthResultDto.Failure("Пользователь не найден.");
            }

            if (!PasswordHasher.Verify(password, user.HeshPassword))
            {
                return AuthResultDto.Failure("Неверный пароль.");
            }

            return AuthResultDto.Success(user);
        }
        catch (Exception ex)
        {
            return AuthResultDto.Failure("Ошибка подключения");
        }
    }
}
