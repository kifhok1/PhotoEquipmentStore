using System.Collections.ObjectModel;
using PhotoEquipmentStore.Domain.Entities;
using PhotoEquipmentStore.Infrastructure.Commands;

namespace PhotoEquipmentStore.Application.Services;

public class ReferenceService
{
    public static ObservableCollection<Reference> GetRoles()
    {
        return ReferenceCommands.GetRoles();
    }
    
    public static ObservableCollection<Reference> GetManufacturers()
    {
        return ReferenceCommands.GetManufacturers();
    }
    
    public static ObservableCollection<Reference> GetSuppliers()
    {
        return ReferenceCommands.GetSuppliers();
    }
    
    public static ObservableCollection<Reference> GetCategories()
    {
        return ReferenceCommands.GetCategories();
    }
    
    public static ObservableCollection<Reference> GetOrderStatuses()
    {
        return ReferenceCommands.GetOrderStatuses();
    }
}