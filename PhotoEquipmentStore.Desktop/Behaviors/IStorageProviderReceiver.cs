using Avalonia.Platform.Storage;

namespace PhotoEquipmentStore.Behaviors;

/// <summary>
/// Контракт для ViewModel, которой необходим доступ к <see cref="IStorageProvider"/> для диалогов файлов.
/// </summary>
public interface IStorageProviderReceiver
{
    /// <summary>
    /// Провайдер хранилища, внедряемый поведением <see cref="StorageProviderInjectorBehavior"/>.
    /// </summary>
    IStorageProvider? StorageProvider { set; }
}
