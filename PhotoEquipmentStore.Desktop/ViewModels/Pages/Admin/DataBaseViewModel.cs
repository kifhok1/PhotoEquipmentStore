using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using PhotoEquipmentStore.Application.Interfaces;
using PhotoEquipmentStore.Application.Services;
using PhotoEquipmentStore.Notification;
using ReactiveUI;
using System.Reactive;
using PhotoEquipmentStore.Application.DTO;
using PhotoEquipmentStore.Behaviors;

namespace PhotoEquipmentStore.ViewModels.Pages.Admin;/// <summary>
/// ViewModel работы с базой данных: экспорт, импорт, бэкап и восстановление.
/// </summary>


public class DataBaseViewModel : ViewModelBase, IStorageProviderReceiver
{
    private readonly IDatabaseService    _databaseService = new DatabaseService();
    private readonly NotificationService _notification    = NotificationService.Instance;
    private readonly IImportService _importService = new ImportService();

    private static string DefaultFolder => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
        "ФотоМагазин", "Файлы базы данных");

    /// <summary>

    /// Провайдер диалогов выбора файлов и папок.

    /// </summary>

    public IStorageProvider? StorageProvider { get; set; }

    /// <summary>

    /// Список таблиц для экспорта.

    /// </summary>

    public ObservableCollection<string> Tables { get; } =
    [
        "Все таблицы",
        .. PhotoEquipmentStore.Infrastructure.Commands.DatabaseCommands.AllTables
    ];

    /// <summary>

    /// Доступные форматы экспорта.

    /// </summary>

    public ObservableCollection<string> ExportFormats { get; } = ["CSV", "XLSX"];

    private string? _selectedExportTable;
    /// <summary>
    /// Выбранная таблица для экспорта.
    /// </summary>
    public string? SelectedExportTable
    {
        get => _selectedExportTable;
        set => this.RaiseAndSetIfChanged(ref _selectedExportTable, value);
    }

    private string? _selectedExportFormat;
    /// <summary>
    /// Выбранный формат экспорта.
    /// </summary>
    public string? SelectedExportFormat
    {
        get => _selectedExportFormat;
        set => this.RaiseAndSetIfChanged(ref _selectedExportFormat, value);
    }

    private string? _exportFolderPath;
    /// <summary>
    /// Папка сохранения экспорта.
    /// </summary>
    public string? ExportFolderPath
    {
        get => _exportFolderPath;
        set => this.RaiseAndSetIfChanged(ref _exportFolderPath, value);
    }

    private string? _selectedImportTable;
    /// <summary>
    /// Выбранная таблица для импорта.
    /// </summary>
    public string? SelectedImportTable
    {
        get => _selectedImportTable;
        set => this.RaiseAndSetIfChanged(ref _selectedImportTable, value);
    }

    /// <summary>

    /// Список таблиц для импорта.

    /// </summary>

    public ObservableCollection<string> ImportTables { get; } =
        new(PhotoEquipmentStore.Infrastructure.Commands.DatabaseCommands.AllTables);

    private string? _importFilePath;
    /// <summary>
    /// Путь к CSV-файлу импорта.
    /// </summary>
    public string? ImportFilePath
    {
        get => _importFilePath;
        set => this.RaiseAndSetIfChanged(ref _importFilePath, value);
    }

    private string? _restoreFilePath;
    /// <summary>
    /// Путь к SQL-файлу восстановления.
    /// </summary>
    public string? RestoreFilePath
    {
        get => _restoreFilePath;
        set => this.RaiseAndSetIfChanged(ref _restoreFilePath, value);
    }

    private string? _backupFolderPath;
    /// <summary>
    /// Папка сохранения резервной копии.
    /// </summary>
    public string? BackupFolderPath
    {
        get => _backupFolderPath;
        set => this.RaiseAndSetIfChanged(ref _backupFolderPath, value);
    }

    private bool _isBusy;
    /// <summary>
    /// Признак выполнения длительной операции с БД.
    /// </summary>
    public bool IsBusy
    {
        get => _isBusy;
        set => this.RaiseAndSetIfChanged(ref _isBusy, value);
    }

    /// <summary>

    /// Команда выбора папки экспорта.

    /// </summary>

    public ReactiveCommand<Unit, Unit> PickExportFolderCommand  { get; }
    /// <summary>
    /// Команда экспорта данных.
    /// </summary>
    public ReactiveCommand<Unit, Unit> ExportCommand            { get; }
    /// <summary>
    /// Команда выбора файла импорта.
    /// </summary>
    public ReactiveCommand<Unit, Unit> PickImportFileCommand    { get; }
    /// <summary>
    /// Команда импорта данных.
    /// </summary>
    public ReactiveCommand<Unit, Unit> ImportCommand            { get; }
    /// <summary>
    /// Команда выбора файла восстановления.
    /// </summary>
    public ReactiveCommand<Unit, Unit> PickRestoreFileCommand   { get; }
    /// <summary>
    /// Команда восстановления структуры БД.
    /// </summary>
    public ReactiveCommand<Unit, Unit> RestoreCommand           { get; }
    /// <summary>
    /// Команда выбора папки резервной копии.
    /// </summary>
    public ReactiveCommand<Unit, Unit> PickBackupFolderCommand  { get; }
    /// <summary>
    /// Команда создания резервной копии.
    /// </summary>
    public ReactiveCommand<Unit, Unit> BackupCommand            { get; }

    public DataBaseViewModel()
    {
        var notBusy = this.WhenAnyValue(x => x.IsBusy, b => !b);

        PickExportFolderCommand = ReactiveCommand.CreateFromTask(PickExportFolderAsync,  notBusy);
        ExportCommand           = ReactiveCommand.CreateFromTask(ExportAsync,            notBusy);
        PickImportFileCommand   = ReactiveCommand.CreateFromTask(PickImportFileAsync,    notBusy);
        ImportCommand           = ReactiveCommand.CreateFromTask(ImportAsync,            notBusy);
        PickRestoreFileCommand  = ReactiveCommand.CreateFromTask(PickRestoreFileAsync,   notBusy);
        RestoreCommand          = ReactiveCommand.CreateFromTask(RestoreAsync,           notBusy);
        PickBackupFolderCommand = ReactiveCommand.CreateFromTask(PickBackupFolderAsync,  notBusy);
        BackupCommand           = ReactiveCommand.CreateFromTask(BackupAsync,            notBusy);
    }

    private async Task PickExportFolderAsync()
    {
        var folder = await StorageProvider!.OpenFolderPickerAsync(new FolderPickerOpenOptions
        {
            Title                  = "Выберите папку для экспорта",
            SuggestedStartLocation = await StorageProvider.TryGetFolderFromPathAsync(DefaultFolder)
        });

        if (folder.Count > 0)
            ExportFolderPath = folder[0].Path.LocalPath;
    }

    private async Task ExportAsync()
    {
        if (string.IsNullOrWhiteSpace(SelectedExportTable))
        {
            await _notification.ShowErrorAsync("Экспорт", "Выберите таблицу для экспорта.");
            return;
        }

        if (string.IsNullOrWhiteSpace(SelectedExportFormat))
        {
            await _notification.ShowErrorAsync("Экспорт", "Выберите формат файла.");
            return;
        }

        var folder = string.IsNullOrWhiteSpace(ExportFolderPath) ? DefaultFolder : ExportFolderPath;
        var format = SelectedExportFormat.ToLowerInvariant();

        if (SelectedExportTable == "Все таблицы")
        {
            var confirmed = await _notification.ShowWarningAsync(
                "Экспорт всех таблиц",
                $"Будет создан zip-архив со всеми таблицами в формате {SelectedExportFormat}.\n" +
                $"Папка сохранения: {folder}\n\nПродолжить?");

            if (!confirmed) return;
        }

        IsBusy = true;
        try
        {
            DataBaseResultDto result;

            if (SelectedExportTable == "Все таблицы")
            {
                result = await _databaseService.ExportAllTablesAsync(folder, format);
            }
            else
            {
                var fileName = $"{SelectedExportTable}_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.{format}";
                var filePath = Path.Combine(folder, fileName);

                result = format == "csv"
                    ? await _databaseService.ExportTableCsvAsync(SelectedExportTable, filePath)
                    : await _databaseService.ExportTableXlsxAsync(SelectedExportTable, filePath);
            }

            if (result.IsSuccess)
                await _notification.ShowInfoAsync("Экспорт завершён",
                    $"Данные успешно экспортированы в папку:\n{folder}");
            else
                await _notification.ShowErrorAsync("Ошибка экспорта", result.ErrorMessage!);
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task PickImportFileAsync()
    {
        var files = await StorageProvider!.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title          = "Выберите CSV файл",
            AllowMultiple  = false,
            FileTypeFilter =
            [
                new FilePickerFileType("CSV файлы") { Patterns = ["*.csv"] }
            ]
        });

        if (files.Count > 0)
            ImportFilePath = files[0].Path.LocalPath;
    }

    private async Task ImportAsync()
    {
        if (string.IsNullOrWhiteSpace(SelectedImportTable))
        {
            await _notification.ShowErrorAsync("Импорт", "Выберите таблицу для импорта.");
            return;
        }

        if (string.IsNullOrWhiteSpace(ImportFilePath))
        {
            await _notification.ShowErrorAsync("Импорт", "Выберите CSV файл для импорта.");
            return;
        }

        var confirmed = await _notification.ShowWarningAsync(
            "Импорт данных",
            $"Данные из файла будут добавлены в таблицу «{SelectedImportTable}».\n" +
            "Существующие записи не удаляются.\n\nПродолжить?");

        if (!confirmed) return;

        IsBusy = true;
        try
        {
            var result = await _importService.ImportAsync(SelectedImportTable, ImportFilePath);

            if (!result.IsSuccess)
            {
                await _notification.ShowErrorAsync("Импорт", result.ErrorMessage!);
                return;
            }

            var message = $"Импортировано: {result.Imported} записей.";
            if (result.Skipped > 0)
                message += $"\nПропущено: {result.Skipped} записей.";

            if (result.SkipReasons.Count > 0)
                message += "\n\nПричины пропуска:\n" +
                           string.Join("\n", result.SkipReasons.Take(10));

            if (result.SkipReasons.Count > 10)
                message += $"\n...и ещё {result.SkipReasons.Count - 10} ошибок.";

            if (result.Skipped > 0)
                await _notification.ShowErrorAsync("Результат импорта", message);
            else
                await _notification.ShowInfoAsync("Импорт завершён", message);
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task PickRestoreFileAsync()
    {
        var files = await StorageProvider!.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title          = "Выберите SQL файл резервной копии",
            AllowMultiple  = false,
            FileTypeFilter =
            [
                new FilePickerFileType("SQL файлы") { Patterns = ["*.sql"] }
            ]
        });

        if (files.Count > 0)
            RestoreFilePath = files[0].Path.LocalPath;
    }

    private async Task RestoreAsync()
    {

        var filLabel  = string.IsNullOrWhiteSpace(RestoreFilePath)
            ? "файл по умолчанию"
            : Path.GetFileName(RestoreFilePath);

        var confirmed = await _notification.ShowWarningAsync(
            "Восстановление базы данных",
            $"Будет выполнено восстановление из: {filLabel}.\n\n" +
            "Текущая структура и все данные будут перезаписаны. " +
            "Изменения после создания резервной копии будут потеряны.\n\n" +
            "Вы уверены?");

        if (!confirmed) return;

        IsBusy = true;
        try
        {

            var result = await _databaseService.RestoreStructureAsync(RestoreFilePath);

            if (result.IsSuccess)
                await _notification.ShowInfoAsync("Восстановление завершено",
                    "База данных успешно восстановлена.");
            else
                await _notification.ShowErrorAsync("Ошибка восстановления", result.ErrorMessage!);
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task PickBackupFolderAsync()
    {
        var folder = await StorageProvider!.OpenFolderPickerAsync(new FolderPickerOpenOptions
        {
            Title                  = "Выберите папку для резервной копии",
            SuggestedStartLocation = await StorageProvider.TryGetFolderFromPathAsync(DefaultFolder)
        });

        if (folder.Count > 0)
            BackupFolderPath = folder[0].Path.LocalPath;
    }

    private async Task BackupAsync()
    {
        var folder   = string.IsNullOrWhiteSpace(BackupFolderPath) ? DefaultFolder : BackupFolderPath;
        var fileName = $"backup_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.sql";
        var filePath = Path.Combine(folder, fileName);

        var confirmed = await _notification.ShowWarningAsync(
            "Резервная копия",
            $"Резервная копия будет сохранена в:\n{filePath}\n\nПродолжить?");

        if (!confirmed) return;

        IsBusy = true;
        try
        {
            var result = await _databaseService.CreateBackupAsync(filePath);

            if (result.IsSuccess)
                await _notification.ShowInfoAsync("Резервная копия создана",
                    $"Файл сохранён:\n{filePath}");
            else
                await _notification.ShowErrorAsync("Ошибка резервного копирования", result.ErrorMessage!);
        }
        finally
        {
            IsBusy = false;
        }
    }
}
