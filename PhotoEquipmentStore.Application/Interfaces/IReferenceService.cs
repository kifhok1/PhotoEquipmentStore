using PhotoEquipmentStore.Application.DTO;
using PhotoEquipmentStore.Domain.Entities;
using PhotoEquipmentStore.Domain.Enums;

namespace PhotoEquipmentStore.Application.Interfaces;

public interface IReferenceService
{
    ReferenceResultDto GetRoles();
    ReferenceResultDto GetOrderStatuses();
    ReferenceResultDto GetCategories();
    ReferenceResultDto GetSuppliers();
    ReferenceResultDto GetManufacturers();

    ReferenceResultDto Create(ReferenceType type, string name);
    ReferenceResultDto Update(ReferenceType type, Reference reference);
    ReferenceResultDto Delete(ReferenceType type, int id);

}
