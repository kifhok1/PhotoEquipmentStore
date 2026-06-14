using System.Reactive;
using System.Threading.Tasks;
using PhotoEquipmentStore.Models;
using ReactiveUI;

namespace PhotoEquipmentStore.ViewModels.Notification;

public class NotificationViewModel : ViewModelBase
{
    private readonly TaskCompletionSource<NotificationResult> _tcs = new();

    public string             Title   { get; }
    public string             Message { get; }
    public NotificationType   Type    { get; }
    public NotificationButtons Buttons { get; }

    public bool ShowOk           => Buttons == NotificationButtons.Ok;
    public bool ShowYesNo        => Buttons is NotificationButtons.YesNo or NotificationButtons.YesNoCancel;
    public bool ShowCancel       => Buttons == NotificationButtons.YesNoCancel;

    public ReactiveCommand<Unit, bool> OkCommand     { get; }
    public ReactiveCommand<Unit, bool> YesCommand    { get; }
    public ReactiveCommand<Unit, bool> NoCommand     { get; }
    public ReactiveCommand<Unit, bool> CancelCommand { get; }

    public Task<NotificationResult> Result => _tcs.Task;

    public NotificationViewModel(
        string title,
        string message,
        NotificationType type    = NotificationType.Info,
        NotificationButtons buttons = NotificationButtons.Ok)
    {
        Title   = title;
        Message = message;
        Type    = type;
        Buttons = buttons;

        OkCommand     = ReactiveCommand.Create(() => _tcs.TrySetResult(NotificationResult.Ok));
        YesCommand    = ReactiveCommand.Create(() => _tcs.TrySetResult(NotificationResult.Yes));
        NoCommand     = ReactiveCommand.Create(() => _tcs.TrySetResult(NotificationResult.No));
        CancelCommand = ReactiveCommand.Create(() => _tcs.TrySetResult(NotificationResult.Cancel));
    }
}
