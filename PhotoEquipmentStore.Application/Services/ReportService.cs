using System;
using System.IO;
using System.Threading.Tasks;
using PhotoEquipmentStore.Application.DTO;
using PhotoEquipmentStore.Application.Reports;
using PhotoEquipmentStore.Domain.Enums;
using PhotoEquipmentStore.Infrastructure.Commands;
using PhotoEquipmentStore.Infrastructure.Exceptions;

namespace PhotoEquipmentStore.Application.Services;

public class ReportService
{
    private readonly ReportCommands _commands = new();

    public (DateTime Min, DateTime Max) GetOrderDateRange()
        => _commands.GetOrderDateRange();

    public DashboardStatsDto GetDashboardStats()
    {
        try
        {
            var s = _commands.GetDashboardStats();
            return new DashboardStatsDto(
                s.OrdersToday, s.ItemsToday, s.RevenueToday,
                s.OrdersMonth, s.ItemsMonth, s.RevenueMonth);
        }
        catch (DatabaseException) { return DashboardStatsDto.Empty; }
    }

    public async Task<ReportResultDto> BuildSalesReportAsync(
        DateTime from, DateTime to,
        Func<string, Task<string?>> pickSavePath)
    {
        try
        {
            var data = _commands.GetSalesReport(from, to);
            if (data.Count == 0)
                return ReportResultDto.Failure("Нет данных за выбранный период.");

            string defaultPath = GetOutputPath($"Продажи_{from:ddMMyyyy}_{to:ddMMyyyy}.xlsx");
            string path        = await ResolveSavePath(defaultPath, pickSavePath);

            ReportBuilder.BuildSalesReport(path, data, from, to);
            return ReportResultDto.Success(path);
        }
        catch (DatabaseException)
        {
            return ReportResultDto.Failure("Не удалось получить данные для отчёта по продажам.");
        }
        catch (Exception)
        {
            return ReportResultDto.Failure("Не удалось сохранить отчёт по продажам.");
        }
    }

    public async Task<ReportResultDto> BuildStockReportAsync(
        int? categoryId, string categoryName,
        Func<string, Task<string?>> pickSavePath)
    {
        try
        {
            var data = _commands.GetStockReport(categoryId);
            if (data.Count == 0)
                return ReportResultDto.Failure("Нет товаров по выбранной категории.");

            string defaultPath = GetOutputPath($"Остатки_{categoryName}_{DateTime.Now:ddMMyyyy}.xlsx");
            string path        = await ResolveSavePath(defaultPath, pickSavePath);

            ReportBuilder.BuildStockReport(path, data, categoryName);
            return ReportResultDto.Success(path);
        }
        catch (DatabaseException)
        {
            return ReportResultDto.Failure("Не удалось получить данные для отчёта по остаткам.");
        }
        catch (Exception)
        {
            return ReportResultDto.Failure("Не удалось сохранить отчёт по остаткам.");
        }
    }

    public async Task<ReportResultDto> BuildPopularityReportAsync(
        int? categoryId, string categoryName, PopularityMode mode,
        Func<string, Task<string?>> pickSavePath)
    {
        try
        {
            var data = _commands.GetPopularityReport(categoryId, mode);
            if (data.Count == 0)
                return ReportResultDto.Failure("Нет данных по выбранной категории.");

            bool allCategories = categoryId is null;

            string modeTag = mode switch
            {
                PopularityMode.AllDesc  => "Все_убывание",
                PopularityMode.AllAsc   => "Все_возрастание",
                PopularityMode.Top30    => "Топ30",
                PopularityMode.Bottom30 => "Антитоп30",
                _                       => "Все"
            };

            string defaultPath = GetOutputPath(
                $"Популярность_{modeTag}_{categoryName}_{DateTime.Now:ddMMyyyy}.xlsx");
            string path = await ResolveSavePath(defaultPath, pickSavePath);

            ReportBuilder.BuildPopularityReport(path, data, categoryName, mode, allCategories);
            return ReportResultDto.Success(path);
        }
        catch (DatabaseException)
        {
            return ReportResultDto.Failure("Не удалось получить данные для отчёта по популярности.");
        }
        catch (Exception)
        {
            return ReportResultDto.Failure("Не удалось сохранить отчёт по популярности.");
        }
    }

    private static async Task<string> ResolveSavePath(
        string defaultPath,
        Func<string, Task<string?>> pickSavePath)
    {
        try
        {
            string? chosen = await pickSavePath(defaultPath);
            if (!string.IsNullOrWhiteSpace(chosen))
                return chosen;
        }
        catch {  }

        return defaultPath;
    }

    private static string GetOutputPath(string fileName)
    {
        string dir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "ФотоМагазин", "Отчёты");
        Directory.CreateDirectory(dir);
        return Path.Combine(dir, fileName);
    }
}
