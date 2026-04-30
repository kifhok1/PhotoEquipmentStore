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
            Console.WriteLine("Заполните все поля.");
            return AuthResultDto.Failure("Заполните все поля.");
        }
            

        var user = Authorization.GetUser(login);

        if (user is null)
        {
            Console.WriteLine("Пользователь не найден.");
            return AuthResultDto.Failure("Пользователь не найден.");
        }


        if (!PasswordHasher.Verify(password, user.HeshPassword))
        { 
            Console.WriteLine("Неверный пароль."); 
            return AuthResultDto.Failure("Неверный пароль.");
        }

        return AuthResultDto.Success(user);
    }
}