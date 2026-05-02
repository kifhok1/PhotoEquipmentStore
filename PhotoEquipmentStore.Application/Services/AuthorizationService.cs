using PhotoEquipmentStore.Application.DTO;
using PhotoEquipmentStore.Application.Helpers;
using Authorization = PhotoEquipmentStore.Infrastructure.Commands.Authorization;

namespace PhotoEquipmentStore.Application.Services;

public class AuthorizationService
{
    public static AuthResultDto Authenticate(string login, string password)
    {
        if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
        {
            return AuthResultDto.Failure("Заполните все поля.");
        }
            

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
}