using PhotoEquipmentStore.Application.DTO;
using PhotoEquipmentStore.Domain.Entities;
using PhotoEquipmentStore.Infrastructure.Connection;

namespace PhotoEquipmentStore.Application.Services;

/// <summary>
/// Сервис проверки подключения к базе данных.
/// </summary>
public class TestConnectionService
{
    /// <summary>
    /// Проверяет возможность подключения к БД с указанными параметрами.
    /// </summary>
    /// <param name="connectionSettings">Параметры подключения.</param>
    /// <returns>Результат проверки соединения.</returns>
    public static TestConnectionDto TestConnection(ConnectionToDBSettings connectionSettings)
    {
        TestConnect testConnect = new TestConnect();
        testConnect.Connect(connectionSettings);
        if (testConnect.Connected)
        {
            return TestConnectionDto.Success();
        }
        else
        {
             return TestConnectionDto.Failure(testConnect.ErrorMassage);
        }
    }
}
