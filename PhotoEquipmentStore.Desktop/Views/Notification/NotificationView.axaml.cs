using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace PhotoEquipmentStore.Views.Notification;

/// <summary>
/// Модальное окно уведомлений (информация, предупреждение, ошибка, подтверждение).
/// </summary>
public partial class NotificationView : UserControl
{
    public NotificationView()
    {
        InitializeComponent();
    }
}
