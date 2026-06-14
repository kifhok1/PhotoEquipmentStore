using System;
using System.Collections.ObjectModel;
using System.Reactive;
using Avalonia.Media.Imaging;
using PhotoEquipmentStore.Application.DTO;
using PhotoEquipmentStore.Application.Services;
using PhotoEquipmentStore.Helper;
using PhotoEquipmentStore.Models;
using PhotoEquipmentStore.Notification;
using ReactiveUI;

namespace PhotoEquipmentStore.ViewModels.Pages.Admin;

public partial class UsersViewModel : ViewModelBase
{
    public ObservableCollection<UserShow> Users { get; } = new();
    private UsersService _usersService = new UsersService();
    private readonly int _currentUserId;

    private string _usersCount = string.Empty;
    public string UsersCount
    {
        get => _usersCount;
        set => this.RaiseAndSetIfChanged(ref _usersCount, value);
    }

    public ReactiveCommand<UserShow, Unit> EditCommand   { get; }
    public ReactiveCommand<UserShow, Unit> DeleteCommand { get; }

    public UsersViewModel(Action<UserShow>? goToEdit = null, int currentUserId = 0)
    {
        _currentUserId = currentUserId;
        FillUsers();

        EditCommand = ReactiveCommand.Create<UserShow>(
            item => goToEdit?.Invoke(item));

        DeleteCommand = ReactiveCommand.Create<UserShow>(async item =>
        {
            if (item.Id == _currentUserId)
                return; // дополнительная защита на уровне VM

            bool confirmed = await NotificationService.Instance.ShowWarningAsync(
                "Удалить запись?",
                $"Вы уверены, что хотите удалить пользователя - {item.Name}? Это действие нельзя будет отменить.");

            if (confirmed)
            {
                var result = _usersService.DeleteUser(item.Id);
                if (result.IsSuccess)
                {
                    await NotificationService.Instance.ShowInfoAsync("Успешно", $"Пользователь - {item.Name} удалён.");
                    FillUsers();
                }
                else
                {
                    await NotificationService.Instance.ShowErrorAsync("Ошибка", $"Не удалось удалить пользователя - {item.Name}.");
                }
            }
        });
    }

    private void FillUsers()
    {
        Users.Clear();
        var usersDb = new UsersService().GetUsers().Users;
        foreach (var user in usersDb)
            Users.Add(new UserShow(
                user.Id, user.Name, user.Login,
                user.PhoneNumber, user.Role, user.RoleID,
                BitmapHelper.FromBytes(user.Image),
                user.Id == _currentUserId)
            );
        UsersCount = $"Количество элементов на форме: {Users.Count}";
    }
}