using System;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using PhotoEquipmentStore.Models;
using PhotoEquipmentStore.ViewModels.Notification;

namespace PhotoEquipmentStore.Notification;

public class NotificationService
{
    private readonly Subject<NotificationViewModel?> _subject = new();
    public IObservable<NotificationViewModel?> Notifications => _subject;

    public static NotificationService Instance { get; } = new();
    private NotificationService() { }

    public async Task ShowInfoAsync(string title, string message)
        => await ShowAsync(title, message, NotificationType.Info, NotificationButtons.Ok);

    public async Task ShowErrorAsync(string title, string message)
        => await ShowAsync(title, message, NotificationType.Error, NotificationButtons.Ok);

    public async Task<bool> ShowWarningAsync(string title, string message)
    {
        var r = await ShowAsync(title, message, NotificationType.Warning, NotificationButtons.YesNo);
        return r == NotificationResult.Yes;
    }

    public async Task<bool> ShowConfirmAsync(string title, string message)
    {
        var r = await ShowAsync(title, message, NotificationType.Info, NotificationButtons.YesNo);
        return r == NotificationResult.Yes;
    }

    public async Task<NotificationResult> ShowAsync(
        string title,
        string message,
        NotificationType type,
        NotificationButtons buttons)
    {
        var vm = new NotificationViewModel(title, message, type, buttons);
        _subject.OnNext(vm);
        var result = await vm.Result;
        _subject.OnNext(null);
        return result;
    }
}
