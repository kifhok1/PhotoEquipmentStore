namespace PhotoEquipmentStore.Application.DTO;

/// <summary>
/// Результат импорта данных из CSV-файла.
/// </summary>
public class ImportResultDto
{

    public bool         IsSuccess    { get; init; }
    public string?      ErrorMessage { get; init; }
    public int          Imported     { get; init; }
    public int          Skipped      { get; init; }
    public List<string> SkipReasons  { get; init; } = [];

    public static ImportResultDto Success(int imported, int skipped, List<string> reasons) =>
        new() { IsSuccess = true, Imported = imported, Skipped = skipped, SkipReasons = reasons };

    public static ImportResultDto Failure(string message) =>
        new() { IsSuccess = false, ErrorMessage = message };
}
