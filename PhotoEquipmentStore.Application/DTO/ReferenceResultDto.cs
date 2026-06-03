using PhotoEquipmentStore.Domain.Entities;

namespace PhotoEquipmentStore.Application.DTO;

public class ReferenceResultDto
{
    public bool IsSuccess             { get; init; }
    public string? ErrorMessage       { get; init; }
    public Reference? Reference       { get; init; }
    public List<Reference> References { get; init; } = [];

    public static ReferenceResultDto Success(IEnumerable<Reference> references) =>
        new() { IsSuccess = true, References = references.ToList() };

    public static ReferenceResultDto Success(Reference reference) =>
        new() { IsSuccess = true, Reference = reference };

    public static ReferenceResultDto Success() =>
        new() { IsSuccess = true };

    public static ReferenceResultDto Failure(string message) =>
        new() { IsSuccess = false, ErrorMessage = message };
}
