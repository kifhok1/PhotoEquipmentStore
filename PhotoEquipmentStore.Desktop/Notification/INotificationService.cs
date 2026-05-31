using System.Threading.Tasks;
using PhotoEquipmentStore.Models;

namespace PhotoEquipmentStore.Notification;

public interface INotificationService
{
    Task             ShowInfoAsync(string title, string message);
    Task             ShowErrorAsync(string title, string message);
    Task<bool>       ShowWarningAsync(string title, string message);
    Task<bool>       ShowConfirmAsync(string title, string message);
    Task<NotificationResult> ShowAsync(
        string title,
        string message,
        NotificationType type,
        NotificationButtons buttons);
}