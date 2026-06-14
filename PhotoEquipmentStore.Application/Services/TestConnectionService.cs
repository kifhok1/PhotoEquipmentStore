using PhotoEquipmentStore.Application.DTO;
using PhotoEquipmentStore.Domain.Entities;
using PhotoEquipmentStore.Infrastructure.Connection;

namespace PhotoEquipmentStore.Application.Services;

public class TestConnectionService
{
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
