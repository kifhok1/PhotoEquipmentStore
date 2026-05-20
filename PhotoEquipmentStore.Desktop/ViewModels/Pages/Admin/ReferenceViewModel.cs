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
    private bool isSelectedRole = true;
    private bool isSelectedSupplier = false;
    private bool isSelectedManufacture = false;
    private bool isSelectedCategory = false;
    private bool isSelectedStatusOrder = false;

    public bool IsSelectedRole
    {
        get => isSelectedRole;
        set
        {
            this.RaiseAndSetIfChanged(ref isSelectedRole, value);
            LoadReferences(ReferenceService.GetRoles());
        }
    }

    public bool IsSelectedSupplier
    {
        get => isSelectedSupplier;
        set
        {
            this.RaiseAndSetIfChanged(ref isSelectedSupplier, value);
            LoadReferences(ReferenceService.GetSuppliers());
        }
    }

    public bool IsSelectedManufacture
    {
        get => isSelectedManufacture;
        set
        {
            this.RaiseAndSetIfChanged(ref isSelectedManufacture, value); 
            LoadReferences(ReferenceService.GetManufacturers());
        }
    }

    public bool IsSelectedCategory
    {
        get => isSelectedCategory;
        set
        {
            this.RaiseAndSetIfChanged(ref isSelectedCategory, value);
            LoadReferences(ReferenceService.GetCategories());
        }
    }

    public bool IsSelectedStatusOrder
    {
        get => isSelectedStatusOrder;
        set
        {
            this.RaiseAndSetIfChanged(ref isSelectedStatusOrder, value);
            LoadReferences(ReferenceService.GetOrderStatuses());
        }
    }
    
    private void LoadReferences(ObservableCollection<Reference> items)
    {
        CurrentReferences.Clear();
        foreach (var item in items)
            CurrentReferences.Add(new ReferenceShow(item.Id, item.Name, item.Count, item.IsDeleted));
        CountReferences = $"Колчество записей на форме: {CurrentReferences.Count}";
    }
    
    private ObservableCollection<ReferenceShow> currentReferences = new();
    private string countReferences = string.Empty;

    public ObservableCollection<ReferenceShow> CurrentReferences
    {
        get => currentReferences;
        private set => this.RaiseAndSetIfChanged(ref currentReferences, value);
    }

    public string CountReferences
    {
        get => countReferences;
        private set => this.RaiseAndSetIfChanged(ref countReferences, value);
    }

    public ReactiveCommand<Unit, Unit> AddReferenceCommand { get; private set; }
    public ReactiveCommand<Unit, Unit> EditReferenceCommand { get; private set; }
    
    public ReferenceViewModel(ReactiveCommand<Unit, Unit>  goToAddReferenceCommand, 
        ReactiveCommand<Unit, Unit> goToEditReferenceCommand)
    {
        AddReferenceCommand = goToAddReferenceCommand;
        EditReferenceCommand = goToEditReferenceCommand;
        LoadReferences(ReferenceService.GetRoles());
    }
    
    
    
    public ReferenceViewModel()
    {
        LoadReferences(ReferenceService.GetRoles());
    }
}