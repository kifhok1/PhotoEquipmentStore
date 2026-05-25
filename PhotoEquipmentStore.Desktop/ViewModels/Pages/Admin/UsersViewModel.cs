using System;
using System.Collections.ObjectModel;
using System.Reactive;
using Avalonia.Media.Imaging;
using PhotoEquipmentStore.Application.Services;
using PhotoEquipmentStore.Helper;
using PhotoEquipmentStore.Models;
using ReactiveUI;

namespace PhotoEquipmentStore.ViewModels.Pages.Admin;

public partial class UsersViewModel : ViewModelBase
{
    public ObservableCollection<UserShow> Users { get; } = new();

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
        EditCommand = ReactiveCommand.Create<UserShow>(
            item => goToEdit?.Invoke(item));

        DeleteCommand = ReactiveCommand.Create<UserShow>(item =>
        {
            // TODO: вызов сервиса удаления
            Users.Remove(item);
            UsersCount = $"Количество элементов на форме: {Users.Count}";
        });

        var usersDb = UsersService.GetUsers();
        foreach (var user in usersDb)
            Users.Add(new UserShow(
                user.Id, user.Name, user.Login,
                user.PhoneNumber, user.Role, user.RoleID,
                BitmapHelper.FromBytes(user.Image)));

        UsersCount = $"Количество элементов на форме: {Users.Count}";
    }
}