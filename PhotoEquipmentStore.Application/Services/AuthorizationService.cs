using PhotoEquipmentStore.Application.DTO;
using PhotoEquipmentStore.Application.Helpers;
using PhotoEquipmentStore.Application.Interfaces;

namespace PhotoEquipmentStore.Application.Services;

public class AuthorizationService
{
    private readonly IUserRepository _userRepository;

    public AuthorizationService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public AuthResultDto Authenticate(string login, string password)
    {
        if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
            return AuthResultDto.Failure("Заполните все поля.");

        var user = _userRepository.GetByLogin(login);

        if (user is null)
            return AuthResultDto.Failure("Пользователь не найден.");

        if (!PasswordHasher.Verify(password, user.HeshPassword))
            return AuthResultDto.Failure("Неверный пароль.");

        return AuthResultDto.Success(user);
    }
}