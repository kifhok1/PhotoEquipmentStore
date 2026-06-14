using Avalonia.Platform.Storage;

namespace PhotoEquipmentStore.Behaviors;

public interface IStorageProviderReceiver
{
    IStorageProvider? StorageProvider { set; }
}