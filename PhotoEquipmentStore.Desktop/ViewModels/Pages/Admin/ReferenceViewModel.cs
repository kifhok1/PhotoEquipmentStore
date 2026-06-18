using System;
using System.Collections.ObjectModel;
using System.Reactive;
using PhotoEquipmentStore.Application.DTO;
using PhotoEquipmentStore.Application.Services;
using PhotoEquipmentStore.Domain.Entities;
using PhotoEquipmentStore.Models;
using PhotoEquipmentStore.Notification;
using ReactiveUI;

namespace PhotoEquipmentStore.ViewModels.Pages.Admin;/// <summary>
/// ViewModel списка справочников с переключением типов.
/// </summary>


public partial class ReferenceViewModel : ViewModelBase
{

    private bool _isSelectedRole        = true;
    private bool _isSelectedSupplier    = false;
    private bool _isSelectedManufacture = false;
    private bool _isSelectedCategory    = false;
    private bool _isSelectedStatusOrder = false;
    private bool _isCreateDeleteVisible = false;
    private ReferenceType _selectedReferenceType = ReferenceType.Role;
    private ReferenceService _referenceService = new ReferenceService();

    /// <summary>

    /// Выбрана вкладка справочника «Роли».

    /// </summary>

    public bool IsSelectedRole
    {
        get => _isSelectedRole;
        set
        {
            this.RaiseAndSetIfChanged(ref _isSelectedRole, value);
            if (!value) return;
            LoadReferences(_referenceService.GetRoles());
            _selectedReferenceType = ReferenceType.Role;
            IsCreateDeleteVisible  = false;
        }
    }

    /// <summary>

    /// Выбрана вкладка справочника «Поставщики».

    /// </summary>

    public bool IsSelectedSupplier
    {
        get => _isSelectedSupplier;
        set
        {
            this.RaiseAndSetIfChanged(ref _isSelectedSupplier, value);
            if (!value) return;
            LoadReferences(_referenceService.GetSuppliers());
            _selectedReferenceType = ReferenceType.Suppliers;
            IsCreateDeleteVisible  = true;
        }
    }

    /// <summary>

    /// Выбрана вкладка справочника «Производители».

    /// </summary>

    public bool IsSelectedManufacture
    {
        get => _isSelectedManufacture;
        set
        {
            this.RaiseAndSetIfChanged(ref _isSelectedManufacture, value);
            if (!value) return;
            LoadReferences(_referenceService.GetManufacturers());
            _selectedReferenceType = ReferenceType.Manufacturers;
            IsCreateDeleteVisible  = true;
        }
    }

    /// <summary>

    /// Выбрана вкладка справочника «Категории».

    /// </summary>

    public bool IsSelectedCategory
    {
        get => _isSelectedCategory;
        set
        {
            this.RaiseAndSetIfChanged(ref _isSelectedCategory, value);
            if (!value) return;
            LoadReferences(_referenceService.GetCategories());
            _selectedReferenceType = ReferenceType.Category;
            IsCreateDeleteVisible  = true;
        }
    }

    /// <summary>

    /// Выбрана вкладка справочника «Статусы заказа».

    /// </summary>

    public bool IsSelectedStatusOrder
    {
        get => _isSelectedStatusOrder;
        set
        {
            this.RaiseAndSetIfChanged(ref _isSelectedStatusOrder, value);
            if (!value) return;
            LoadReferences(_referenceService.GetOrderStatuses());
            _selectedReferenceType = ReferenceType.OrderStatuses;
            IsCreateDeleteVisible  = false;
        }
    }

    /// <summary>

    /// Признак видимости кнопок создания и удаления.

    /// </summary>

    public bool IsCreateDeleteVisible
    {
        get => _isCreateDeleteVisible;
        set => this.RaiseAndSetIfChanged(ref _isCreateDeleteVisible, value);
    }

    private ObservableCollection<ReferenceShow> _currentReferences = new();
    private string _countReferences = string.Empty;

    /// <summary>

    /// Текущий список записей справочника.

    /// </summary>

    public ObservableCollection<ReferenceShow> CurrentReferences
    {
        get => _currentReferences;
        private set => this.RaiseAndSetIfChanged(ref _currentReferences, value);
    }

    /// <summary>

    /// Подпись с количеством записей.

    /// </summary>

    public string CountReferences
    {
        get => _countReferences;
        private set => this.RaiseAndSetIfChanged(ref _countReferences, value);
    }

    private void LoadReferences(ReferenceResultDto items)
    {
        CurrentReferences.Clear();
        foreach (var item in items.References)
            CurrentReferences.Add(new ReferenceShow(item.Id, item.Name, item.Count, item.IsDeleted));
        CountReferences = $"Количество записей на форме: {CurrentReferences.Count}";
    }

    /// <summary>

    /// Команда перехода к добавлению справочника.

    /// </summary>

    public ReactiveCommand<Unit, Unit> AddReferenceCommand { get; }

    /// <summary>

    /// Команда редактирования записи справочника.

    /// </summary>

    public ReactiveCommand<ReferenceShow, Unit> EditReferenceCommand { get; }

    /// <summary>

    /// Команда удаления записи.

    /// </summary>

    public ReactiveCommand<ReferenceShow, Unit> DeleteCommand { get; }

    public ReferenceViewModel(
        Action<ReferenceType>           goToAdd,
        Action<ReferenceType, ReferenceShow> goToEdit)
    {

        AddReferenceCommand = ReactiveCommand.Create(
            () => goToAdd(_selectedReferenceType));

        EditReferenceCommand = ReactiveCommand.Create<ReferenceShow>(
            item => goToEdit(_selectedReferenceType, item));

        DeleteCommand = ReactiveCommand.Create<ReferenceShow>(async item =>
        {
            bool confirmed = await NotificationService.Instance.ShowWarningAsync(
                "Удалить запись?",
                $"Вы уверены, что хотите удалить запись - {item.Title}? Это действие нельзя будет отменить.");
            if (_selectedReferenceType == ReferenceType.Suppliers)
            {
                var dto = _referenceService.Delete(Domain.Enums.ReferenceType.Supplier, item.Id);
                if (dto.IsSuccess)
                {
                    LoadReferences(_referenceService.GetSuppliers());
                    await NotificationService.Instance.ShowInfoAsync(
                        "Успех",
                        $"Запись - {item.Title} удалена.");
                }
                else
                {
                    await NotificationService.Instance.ShowErrorAsync(
                        "Ошибка",
                        $"Ошибка удаления - {dto.ErrorMessage}");
                }
            }
            else if (_selectedReferenceType == ReferenceType.Manufacturers)
            {
                var dto = _referenceService.Delete(Domain.Enums.ReferenceType.Manufacturer, item.Id);
                if (dto.IsSuccess)
                {
                    LoadReferences(_referenceService.GetManufacturers());
                    await NotificationService.Instance.ShowInfoAsync(
                        "Успех",
                        $"Запись - {item.Title} удалена.");
                }
                else
                {
                    await NotificationService.Instance.ShowErrorAsync(
                        "Ошибка",
                        $"Ошибка удаления - {dto.ErrorMessage}");
                }
            }
            else if (_selectedReferenceType == ReferenceType.Category)
            {
                var dto = _referenceService.Delete(Domain.Enums.ReferenceType.Category, item.Id);
                if (dto.IsSuccess)
                {
                    LoadReferences(_referenceService.GetCategories());
                    await NotificationService.Instance.ShowInfoAsync(
                        "Успех",
                        $"Запись - {item.Title} удалена.");
                }
                else
                {
                    await NotificationService.Instance.ShowErrorAsync(
                        "Ошибка",
                        $"Ошибка удаления - {dto.ErrorMessage}");
                }
            }
        });

        LoadReferences(_referenceService.GetRoles());
    }

    public ReferenceViewModel()
    {
        AddReferenceCommand  = ReactiveCommand.Create(() => { });
        EditReferenceCommand = ReactiveCommand.Create<ReferenceShow>(_ => { });
        DeleteCommand        = ReactiveCommand.Create<ReferenceShow>(_ => { });
        LoadReferences(_referenceService.GetRoles());
    }
}
