using System;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using PhotoEquipmentStore.Models;
using PhotoEquipmentStore.ViewModels.Notification;

namespace PhotoEquipmentStore.Notification;/// <summary>
/// Сервис отображения модальных уведомлений через ReactiveUI-поток.
/// </summary>


public class NotificationService
{
    private readonly Subject<NotificationViewModel?> _subject = new();
    /// <summary>
    /// Поток активных уведомлений для подписки UI.
    /// </summary>
    public IObservable<NotificationViewModel?> Notifications => _subject;

    /// <summary>

    /// Единственный экземпляр конвертера для привязок.

    /// </summary>

    public static NotificationService Instance { get; } = new();
    private NotificationService() { }

    /// <summary>

    /// Показывает информационное уведомление с кнопкой OK.

    /// </summary>

    public async Task ShowInfoAsync(string title, string message)
        => await ShowAsync(title, message, NotificationType.Info, NotificationButtons.Ok);

    /// <summary>

    /// Показывает уведомление об ошибке с кнопкой OK.

    /// </summary>

    public async Task ShowErrorAsync(string title, string message)
        => await ShowAsync(title, message, NotificationType.Error, NotificationButtons.Ok);

    /// <summary>

    /// Показывает предупреждение с кнопками Да/Нет.

    /// </summary>

    public async Task<bool> ShowWarningAsync(string title, string message)
    {
        var r = await ShowAsync(title, message, NotificationType.Warning, NotificationButtons.YesNo);
        return r == NotificationResult.Yes;
    }

    /// <summary>

    /// Показывает диалог подтверждения с кнопками Да/Нет.

    /// </summary>

    public async Task<bool> ShowConfirmAsync(string title, string message)
    {
        var r = await ShowAsync(title, message, NotificationType.Info, NotificationButtons.YesNo);
        return r == NotificationResult.Yes;
    }

    /// <summary>

    /// Показывает уведомление с заданным типом и набором кнопок.

    /// </summary>

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
