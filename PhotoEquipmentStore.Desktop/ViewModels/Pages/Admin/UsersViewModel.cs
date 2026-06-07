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
    private UsersService _usersService =  new UsersService();

    private string _usersCount = string.Empty;
    public string UsersCount
    {
        get => _usersCount;
        set => this.RaiseAndSetIfChanged(ref _usersCount, value);
    }

    public ReactiveCommand<UserShow, Unit> EditCommand   { get; }
    public ReactiveCommand<UserShow, Unit> DeleteCommand { get; }

    public UsersViewModel(Action<UserShow>? goToEdit = null)
    {
        FillUsers();
        EditCommand = ReactiveCommand.Create<UserShow>(
            item => goToEdit?.Invoke(item));

        DeleteCommand = ReactiveCommand.Create<UserShow>(async item =>
        {
            bool confirmed = await NotificationService.Instance.ShowWarningAsync(
                "Удалить запись?",
                $"Вы уверены, что хотите удалить пользователя - {item.Name}? Это действие нельзя будет отменить.");

            if (confirmed)
            {
                var UsersDb = _usersService.DeleteUser(item.Id);
                if (UsersDb.IsSuccess)
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
                BitmapHelper.FromBytes(user.Image))
            );
        UsersCount = $"Количество элементов на форме: {Users.Count}";
    }
}