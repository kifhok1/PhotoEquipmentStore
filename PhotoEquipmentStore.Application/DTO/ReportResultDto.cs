
namespace PhotoEquipmentStore.Application.DTO;

/// <summary>
/// Результат формирования и сохранения отчёта.
/// </summary>
public class ReportResultDto
{
    public bool    IsSuccess    { get; init; }
    public string? ErrorMessage { get; init; }
    public string? FilePath     { get; init; }

    public static ReportResultDto Success(string filePath) =>
        new() { IsSuccess = true, FilePath = filePath };

    public static ReportResultDto Failure(string message) =>
        new() { IsSuccess = false, ErrorMessage = message };
}
