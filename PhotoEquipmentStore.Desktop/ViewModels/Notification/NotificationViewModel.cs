using System.Reactive;
using System.Threading.Tasks;
using PhotoEquipmentStore.Models;
using ReactiveUI;

namespace PhotoEquipmentStore.ViewModels.Notification;/// <summary>
/// ViewModel модального окна уведомления.
/// </summary>


public class NotificationViewModel : ViewModelBase
{
    private readonly TaskCompletionSource<NotificationResult> _tcs = new();

    /// <summary>

    /// Заголовок уведомления.

    /// </summary>

    public string             Title   { get; }
    /// <summary>
    /// Текст сообщения уведомления.
    /// </summary>
    public string             Message { get; }
    /// <summary>
    /// Тип уведомления (информация, предупреждение, ошибка).
    /// </summary>
    public NotificationType   Type    { get; }
    /// <summary>
    /// Набор кнопок диалога.
    /// </summary>
    public NotificationButtons Buttons { get; }

    /// <summary>

    /// Отображать ли кнопку OK.

    /// </summary>

    public bool ShowOk           => Buttons == NotificationButtons.Ok;
    /// <summary>
    /// Отображать ли кнопки Да/Нет.
    /// </summary>
    public bool ShowYesNo        => Buttons is NotificationButtons.YesNo or NotificationButtons.YesNoCancel;
    /// <summary>
    /// Отображать ли кнопку Отмена.
    /// </summary>
    public bool ShowCancel       => Buttons == NotificationButtons.YesNoCancel;

    /// <summary>

    /// Команда подтверждения уведомления (OK).

    /// </summary>

    public ReactiveCommand<Unit, bool> OkCommand     { get; }
    /// <summary>
    /// Команда положительного ответа (Да).
    /// </summary>
    public ReactiveCommand<Unit, bool> YesCommand    { get; }
    /// <summary>
    /// Команда отрицательного ответа (Нет).
    /// </summary>
    public ReactiveCommand<Unit, bool> NoCommand     { get; }
    /// <summary>
    /// Команда отмены диалога.
    /// </summary>
    public ReactiveCommand<Unit, bool> CancelCommand { get; }

    /// <summary>

    /// Задача, завершающаяся выбранным результатом диалога.

    /// </summary>

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
