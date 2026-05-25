using System;
using System.Collections.ObjectModel;
using System.Reactive;
using PhotoEquipmentStore.Application.Services;
using PhotoEquipmentStore.Domain.Entities;
using PhotoEquipmentStore.Models;
using ReactiveUI;

namespace PhotoEquipmentStore.ViewModels.Pages.Admin;

public partial class ReferenceViewModel : ViewModelBase
{
    // ── Tab-переключатели ────────────────────────────────────────────────────
    private bool _isSelectedRole        = true;
    private bool _isSelectedSupplier    = false;
    private bool _isSelectedManufacture = false;
    private bool _isSelectedCategory    = false;
    private bool _isSelectedStatusOrder = false;
    private bool _isCreateDeleteVisible = false;
    private ReferenceType _selectedReferenceType = ReferenceType.Role;

    public bool IsSelectedRole
    {
        get => _isSelectedRole;
        set
        {
            this.RaiseAndSetIfChanged(ref _isSelectedRole, value);
            if (!value) return;
            LoadReferences(ReferenceService.GetRoles());
            _selectedReferenceType = ReferenceType.Role;
            IsCreateDeleteVisible  = false;
        }
    }

    public bool IsSelectedSupplier
    {
        get => _isSelectedSupplier;
        set
        {
            this.RaiseAndSetIfChanged(ref _isSelectedSupplier, value);
            if (!value) return;
            LoadReferences(ReferenceService.GetSuppliers());
            _selectedReferenceType = ReferenceType.Suppliers;
            IsCreateDeleteVisible  = true;
        }
    }

    public bool IsSelectedManufacture
    {
        get => _isSelectedManufacture;
        set
        {
            this.RaiseAndSetIfChanged(ref _isSelectedManufacture, value);
            if (!value) return;
            LoadReferences(ReferenceService.GetManufacturers());
            _selectedReferenceType = ReferenceType.Manufacturers;
            IsCreateDeleteVisible  = true;
        }
    }

    public bool IsSelectedCategory
    {
        get => _isSelectedCategory;
        set
        {
            this.RaiseAndSetIfChanged(ref _isSelectedCategory, value);
            if (!value) return;
            LoadReferences(ReferenceService.GetCategories());
            _selectedReferenceType = ReferenceType.Category;
            IsCreateDeleteVisible  = true;
        }
    }

    public bool IsSelectedStatusOrder
    {
        get => _isSelectedStatusOrder;
        set
        {
            this.RaiseAndSetIfChanged(ref _isSelectedStatusOrder, value);
            if (!value) return;
            LoadReferences(ReferenceService.GetOrderStatuses());
            _selectedReferenceType = ReferenceType.OrderStatuses;
            IsCreateDeleteVisible  = false;
        }
    }

    public bool IsCreateDeleteVisible
    {
        get => _isCreateDeleteVisible;
        set => this.RaiseAndSetIfChanged(ref _isCreateDeleteVisible, value);
    }

    // ── Данные таблицы ───────────────────────────────────────────────────────
    private ObservableCollection<ReferenceShow> _currentReferences = new();
    private string _countReferences = string.Empty;

    public ObservableCollection<ReferenceShow> CurrentReferences
    {
        get => _currentReferences;
        private set => this.RaiseAndSetIfChanged(ref _currentReferences, value);
    }

    public string CountReferences
    {
        get => _countReferences;
        private set => this.RaiseAndSetIfChanged(ref _countReferences, value);
    }

    private void LoadReferences(ObservableCollection<Reference> items)
    {
        CurrentReferences.Clear();
        foreach (var item in items)
            CurrentReferences.Add(new ReferenceShow(item.Id, item.Name, item.Count, item.IsDeleted));
        CountReferences = $"Количество записей на форме: {CurrentReferences.Count}";
    }

    // ── Команды ──────────────────────────────────────────────────────────────

    /// Создать — вызывается кнопкой «Создать» в шапке
    public ReactiveCommand<Unit, Unit> AddReferenceCommand { get; }

    /// Редактировать — принимает строку таблицы (CommandParameter)
    public ReactiveCommand<ReferenceShow, Unit> EditReferenceCommand { get; }

    /// Удалить — принимает строку таблицы (CommandParameter)
    public ReactiveCommand<ReferenceShow, Unit> DeleteCommand { get; }

    public ReferenceViewModel(
        Action<ReferenceType>           goToAdd,
        Action<ReferenceType, ReferenceShow> goToEdit)
    {
        // Команда «Создать» передаёт текущий тип справочника
        AddReferenceCommand = ReactiveCommand.Create(
            () => goToAdd(_selectedReferenceType));

        // Команда «Редактировать» передаёт тип + выбранный элемент
        EditReferenceCommand = ReactiveCommand.Create<ReferenceShow>(
            item => goToEdit(_selectedReferenceType, item));

        // Команда «Удалить»
        DeleteCommand = ReactiveCommand.Create<ReferenceShow>(item =>
        {
            // TODO: вызов сервиса удаления
            CurrentReferences.Remove(item);
            CountReferences = $"Количество записей на форме: {CurrentReferences.Count}";
        });

        LoadReferences(ReferenceService.GetRoles());
    }

    // Конструктор для дизайнера
    public ReferenceViewModel()
    {
        AddReferenceCommand  = ReactiveCommand.Create(() => { });
        EditReferenceCommand = ReactiveCommand.Create<ReferenceShow>(_ => { });
        DeleteCommand        = ReactiveCommand.Create<ReferenceShow>(_ => { });
        LoadReferences(ReferenceService.GetRoles());
    }
}