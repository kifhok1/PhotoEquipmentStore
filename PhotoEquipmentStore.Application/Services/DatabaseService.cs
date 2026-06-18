using System.Data;
using System.Globalization;
using System.IO.Compression;
using System.Text;
using CsvHelper;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using PhotoEquipmentStore.Application.DTO;
using PhotoEquipmentStore.Application.Interfaces;
using PhotoEquipmentStore.Infrastructure.Commands;
using PhotoEquipmentStore.Infrastructure.Exceptions;

namespace PhotoEquipmentStore.Application.Services;

/// <summary>
/// Сервис операций с базой данных: резервное копирование, восстановление и экспорт.
/// </summary>
public class DatabaseService : IDatabaseService
{
    private readonly DatabaseCommands _commands = new();

    private static readonly string DefaultDumpPath = Path.Combine(
        AppContext.BaseDirectory, "Scripts", "default_dump.sql");

    /// <summary>Создаёт резервную копию базы данных.</summary>
    /// <param name="outputPath">Путь для сохранения дампа.</param>
    public async Task<DataBaseResultDto> CreateBackupAsync(string outputPath)
    {
        try
        {
            EnsureDirectory(outputPath);
            await _commands.CreateBackupAsync(outputPath);
            return DataBaseResultDto.Success();
        }
        catch (DatabaseException ex)
        {
            return DataBaseResultDto.Failure(ex.Message);
        }
    }

    /// <summary>Восстанавливает структуру БД из SQL-файла или дампа по умолчанию.</summary>
    /// <param name="sqlFilePath">Путь к SQL-файлу; при null используется файл по умолчанию.</param>
    public async Task<DataBaseResultDto> RestoreStructureAsync(string? sqlFilePath)
    {
        try
        {
            var path = string.IsNullOrWhiteSpace(sqlFilePath) ? DefaultDumpPath : sqlFilePath;

            if (!File.Exists(path))
                return DataBaseResultDto.Failure($"Файл не найден: {path}");

            await _commands.RestoreStructureAsync(path);
            return DataBaseResultDto.Success();
        }
        catch (DatabaseException ex)
        {
            return DataBaseResultDto.Failure(ex.Message);
        }
    }

    /// <summary>Экспортирует таблицу в CSV-файл.</summary>
    /// <param name="tableName">Имя таблицы.</param>
    /// <param name="outputPath">Путь для сохранения файла.</param>
    public async Task<DataBaseResultDto> ExportTableCsvAsync(string tableName, string outputPath)
    {
        try
        {
            EnsureDirectory(outputPath);
            var table = await _commands.FetchTableAsync(tableName);
            await WriteCsvAsync(table, outputPath);
            return DataBaseResultDto.Success();
        }
        catch (DatabaseException ex)
        {
            return DataBaseResultDto.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            return DataBaseResultDto.Failure($"Ошибка при записи CSV: {ex.Message}");
        }
    }

    /// <summary>Экспортирует таблицу в XLSX-файл.</summary>
    /// <param name="tableName">Имя таблицы.</param>
    /// <param name="outputPath">Путь для сохранения файла.</param>
    public async Task<DataBaseResultDto> ExportTableXlsxAsync(string tableName, string outputPath)
    {
        try
        {
            EnsureDirectory(outputPath);
            var table = await _commands.FetchTableAsync(tableName);
            await WriteXlsxAsync(table, tableName, outputPath);
            return DataBaseResultDto.Success();
        }
        catch (DatabaseException ex)
        {
            return DataBaseResultDto.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            return DataBaseResultDto.Failure($"Ошибка при записи XLSX: {ex.Message}");
        }
    }

    /// <summary>Экспортирует все таблицы в ZIP-архив указанного формата.</summary>
    /// <param name="outputFolderPath">Папка для сохранения архива.</param>
    /// <param name="format">Формат файлов: csv или xlsx.</param>
    public async Task<DataBaseResultDto> ExportAllTablesAsync(string outputFolderPath, string format)
    {
        var tempDir = Path.Combine(Path.GetTempPath(), $"db_export_{Guid.NewGuid():N}");
        Directory.CreateDirectory(tempDir);

        try
        {
            foreach (var tableName in DatabaseCommands.AllTables)
            {
                var ext      = format == "csv" ? "csv" : "xlsx";
                var filePath = Path.Combine(tempDir, $"{tableName}.{ext}");
                var table    = await _commands.FetchTableAsync(tableName);

                if (format == "csv")
                    await WriteCsvAsync(table, filePath);
                else
                    await WriteXlsxAsync(table, tableName, filePath);
            }

            var zipName = $"export_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.zip";
            var zipPath = Path.Combine(outputFolderPath, zipName);

            Directory.CreateDirectory(outputFolderPath);
            await Task.Run(() => ZipFile.CreateFromDirectory(tempDir, zipPath));

            return DataBaseResultDto.Success();
        }
        catch (DatabaseException ex)
        {
            return DataBaseResultDto.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            return DataBaseResultDto.Failure($"Ошибка при создании архива: {ex.Message}");
        }
        finally
        {
            Directory.Delete(tempDir, recursive: true);
        }
    }

    private static async Task WriteCsvAsync(DataTable table, string outputPath)
    {
        await Task.Run(() =>
        {
            using var writer = new StreamWriter(outputPath, false, Encoding.UTF8);
            using var csv    = new CsvWriter(writer, CultureInfo.InvariantCulture);

            foreach (DataColumn col in table.Columns)
                csv.WriteField(col.ColumnName);
            csv.NextRecord();

            foreach (DataRow row in table.Rows)
            {
                foreach (var item in row.ItemArray)
                    csv.WriteField(item);
                csv.NextRecord();
            }
        });
    }

    private static async Task WriteXlsxAsync(DataTable table, string sheetName, string outputPath)
    {
        await Task.Run(() =>
        {
            using var document    = SpreadsheetDocument.Create(outputPath, SpreadsheetDocumentType.Workbook);
            var workbookPart      = document.AddWorkbookPart();
            workbookPart.Workbook = new Workbook();

            var worksheetPart         = workbookPart.AddNewPart<WorksheetPart>();
            var sheetData             = new SheetData();
            worksheetPart.Worksheet   = new Worksheet(sheetData);

            var sheets = workbookPart.Workbook.AppendChild(new Sheets());
            sheets.Append(new Sheet
            {
                Id      = workbookPart.GetIdOfPart(worksheetPart),
                SheetId = 1,
                Name    = sheetName
            });

            var headerRow = new Row();
            foreach (DataColumn col in table.Columns)
                headerRow.Append(MakeCell(col.ColumnName));
            sheetData.Append(headerRow);

            foreach (DataRow row in table.Rows)
            {
                var xlRow = new Row();
                foreach (var item in row.ItemArray)
                    xlRow.Append(MakeCell(item?.ToString() ?? string.Empty));
                sheetData.Append(xlRow);
            }

            workbookPart.Workbook.Save();
        });
    }

    private static Cell MakeCell(string value) =>
        new()
        {
            DataType     = CellValues.InlineString,
            InlineString = new InlineString { Text = new Text(value) }
        };

    private static void EnsureDirectory(string filePath)
    {
        var dir = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrEmpty(dir))
            Directory.CreateDirectory(dir);
    }
}
