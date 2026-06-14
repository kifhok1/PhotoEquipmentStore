using System;
using System.IO;
using System.Threading.Tasks;
using PhotoEquipmentStore.Application.DTO;
using PhotoEquipmentStore.Application.Interfaces;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

using QContainer = QuestPDF.Infrastructure.IContainer;

namespace PhotoEquipmentStore.Infrastructure.Services;

/// <summary>
/// Сервис генерации PDF-чеков по заказам с использованием QuestPDF.
/// </summary>
public class ReceiptPdfService : IReceiptPdfService
{

    private const string CPrimary      = "#4E6EF5";
    private const string CPrimaryDark  = "#3A55D4";
    private const string CPrimaryLight = "#B8C8FF";
    private const string CMainText     = "#1A1F36";
    private const string CSecondary    = "#4A5073";
    private const string CMuted        = "#9BA3BF";
    private const string CSurface      = "#FFFFFF";
    private const string CBackground   = "#EAEFF9";
    private const string CBorder       = "#D2DAEC";
    private const string CElevated     = "#F3F6FF";
    private const string CSuccess      = "#27AE60";
    private const string CSuccessBg    = "#EAF7EF";

    private static byte[]? _logoBytes;

    private static byte[]? GetLogoBytes()
    {
        if (_logoBytes is not null) return _logoBytes;
        try
        {
            var path = System.IO.Path.Combine(
                AppContext.BaseDirectory, "Assets", "logo.png");
            _logoBytes = System.IO.File.ReadAllBytes(path);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[ReceiptPdfService] Logo: {ex.Message}");
        }
        return _logoBytes;
    }

    /// <summary>
    /// Формирует и сохраняет PDF-чек по данным заказа.
    /// </summary>
    /// <param name="data">Данные чека.</param>
    /// <param name="filePath">Путь для сохранения файла.</param>
    /// <returns>Флаг успеха и текст ошибки при неудаче.</returns>
    public Task<(bool Success, string? Error)> SaveReceiptAsync(ReceiptData data, string filePath)
    {
        try
        {
            QuestPDF.Settings.License = LicenseType.Community;

            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A5);
                    page.Margin(0);
                    page.PageColor(CSurface);
                    page.DefaultTextStyle(t => t
                        .FontFamily("Arial")
                        .FontSize(9)
                        .FontColor(CMainText));

                    page.Header().Element(c => ComposeHeader(c, data));
                    page.Content().Element(c => ComposeContent(c, data));
                    page.Footer().Element(c => ComposeFooter(c));
                });
            }).GeneratePdf(filePath);

            return Task.FromResult<(bool, string?)>((true, null));
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[ReceiptPdfService] {ex}");
            return Task.FromResult<(bool, string?)>((false, ex.Message));
        }
    }

    private static void ComposeHeader(QContainer container, ReceiptData data)
    {
        container.Column(col =>
        {
            col.Item()
                .Background(CPrimary)
                .PaddingHorizontal(24)
                .PaddingVertical(16)
                .Row(row =>
                {

                    row.RelativeItem().Column(c =>
                    {

                        c.Item().Row(r =>
                        {
                            var logo = GetLogoBytes();
                            if (logo is { Length: > 0 })
                            {
                                r.AutoItem()
                                    .Width(34).Height(34)
                                    .AlignMiddle()
                                    .Image(logo)
                                    .FitArea();

                                r.ConstantItem(10);
                            }

                            r.RelativeItem().AlignMiddle().Column(inner =>
                            {
                                inner.Item()
                                    .Text("ФотоМагазин")
                                    .FontSize(16).Bold().FontColor(CSurface);
                                inner.Item()
                                    .Text("Профессиональная фототехника")
                                    .FontSize(7.5f).FontColor(CPrimaryLight);
                            });
                        });
                    });

                    row.AutoItem().AlignRight().AlignMiddle().Column(c =>
                    {
                        c.Item().AlignRight()
                            .Text($"Чек #{data.OrderNumber}")
                            .FontSize(10).Bold().FontColor(CSurface);
                        c.Item().AlignRight()
                            .Text(data.CreatedAt)
                            .FontSize(8).FontColor(CPrimaryLight);
                    });
                });

            col.Item().Height(4).Background(CPrimaryDark);
        });
    }

    private static void ComposeContent(QContainer container, ReceiptData data)
    {
        container
            .PaddingHorizontal(24)
            .PaddingTop(16)
            .Column(col =>
            {

                col.Item().Row(row =>
                {
                    row.RelativeItem().Element(c => ComposePersonCard(c,
                        label:      "ПОКУПАТЕЛЬ",
                        labelColor: CPrimary,
                        name:       data.ClientName,
                        sub:        data.ClientPhone,
                        badge:      $"Скидка: {data.ClientDiscount}"));

                    row.ConstantItem(10);

                    row.RelativeItem().Element(c => ComposePersonCard(c,
                        label:      "ПРОДАВЕЦ",
                        labelColor: CPrimary,
                        name:       data.SellerName,
                        sub:        null,
                        badge:      null));
                });

                col.Item().Height(14);

                col.Item().Row(r =>
                {
                    r.RelativeItem()
                        .Text("Состав заказа")
                        .FontSize(10).Bold().FontColor(CMainText);
                    r.AutoItem()
                        .Text($"{data.Items.Count} {PluralItems(data.Items.Count)}")
                        .FontSize(8).FontColor(CMuted);
                });

                col.Item().Height(6);

                col.Item()
                    .Background(CPrimary)
                    .PaddingVertical(6)
                    .PaddingHorizontal(10)
                    .Row(r =>
                    {
                        r.RelativeItem(5)
                            .Text("Наименование")
                            .FontSize(8).Bold().FontColor(CSurface);
                        r.RelativeItem(1.2f).AlignCenter()
                            .Text("Кол.")
                            .FontSize(8).Bold().FontColor(CSurface);
                        r.RelativeItem(2.2f).AlignRight()
                            .Text("Цена")
                            .FontSize(8).Bold().FontColor(CSurface);
                        r.RelativeItem(2.2f).AlignRight()
                            .Text("Со скидкой")
                            .FontSize(8).Bold().FontColor(CSurface);
                        r.RelativeItem(2.2f).AlignRight()
                            .Text("Итого")
                            .FontSize(8).Bold().FontColor(CSurface);
                    });

                for (int i = 0; i < data.Items.Count; i++)
                {
                    var item     = data.Items[i];
                    var rowBg    = i % 2 == 0 ? CSurface : CElevated;
                    bool hasDisc = item.FinalPrice < item.OriginalPrice;

                    col.Item()
                        .Background(rowBg)
                        .BorderBottom(1).BorderColor(CBorder)
                        .PaddingVertical(6)
                        .PaddingHorizontal(10)
                        .Row(r =>
                        {
                            r.RelativeItem(5)
                                .Text(item.Name)
                                .FontSize(8.5f).FontColor(CMainText);

                            r.RelativeItem(1.2f).AlignCenter()
                                .Text(item.Quantity.ToString())
                                .FontSize(8.5f).FontColor(CSecondary);

                            r.RelativeItem(2.2f).AlignRight().Text(text =>
                            {
                                var span = text.Span($"{item.OriginalPrice:N0} ₽")
                                    .FontSize(8.5f);
                                if (hasDisc)
                                    span.FontColor(CMuted).Strikethrough();
                                else
                                    span.FontColor(CMainText);
                            });

                            r.RelativeItem(2.2f).AlignRight().Text(text =>
                            {
                                var span = text.Span($"{item.FinalPrice:N0} ₽")
                                    .FontSize(8.5f);
                                if (hasDisc)
                                    span.FontColor(CSuccess).Bold();
                                else
                                    span.FontColor(CMuted);
                            });

                            r.RelativeItem(2.2f).AlignRight()
                                .Text($"{item.LineTotal:N0} ₽")
                                .FontSize(8.5f).Bold().FontColor(CMainText);
                        });
                }

                col.Item().Height(14);

                col.Item().Column(s =>
                {

                    if (data.ProductDiscount > 0 || data.ClientDiscountAmt > 0)
                    {
                        col.Item()
                            .Background(CSuccessBg)
                            .Border(1).BorderColor(CBorder)
                            .PaddingVertical(8)
                            .PaddingHorizontal(10)
                            .Column(disc =>
                            {
                                AddSummaryRow(disc, "Сумма без скидок",
                                    $"{data.Subtotal:N0} ₽");

                                if (data.ProductDiscount > 0)
                                    AddSummaryRow(disc, "Скидка на товары",
                                        $"−{data.ProductDiscount:N0} ₽", CSuccess);

                                if (data.ClientDiscountAmt > 0)
                                    AddSummaryRow(disc,
                                        $"Скидка клиента ({data.ClientDiscountPct}%)",
                                        $"−{data.ClientDiscountAmt:N0} ₽", CSuccess);

                                if (data.Delivery > 0)
                                    AddSummaryRow(disc, "Доставка",
                                        $"+{data.Delivery:N0} ₽", CSecondary);
                            });
                    }
                    else
                    {
                        AddSummaryRow(s, "Сумма", $"{data.Subtotal:N0} ₽");
                        if (data.Delivery > 0)
                            AddSummaryRow(s, "Доставка",
                                $"+{data.Delivery:N0} ₽", CSecondary);
                    }

                    s.Item()
                        .Background(CPrimary)
                        .PaddingVertical(10)
                        .PaddingHorizontal(10)
                        .Row(r =>
                        {
                            r.RelativeItem()
                                .Text("ИТОГО К ОПЛАТЕ")
                                .FontSize(11).Bold().FontColor(CSurface);
                            r.AutoItem()
                                .Text($"{data.Total:N0} ₽")
                                .FontSize(14).Bold().FontColor(CSurface);
                        });
                });
            });
    }

    private static void ComposeFooter(QContainer container)
    {
        container
            .PaddingHorizontal(24)
            .PaddingTop(8)
            .Column(col =>
            {
                col.Item().LineHorizontal(1).LineColor(CBorder);
                col.Item().PaddingTop(6).Row(r =>
                {
                    r.RelativeItem()
                        .Text("Спасибо за покупку!")
                        .FontSize(8).FontColor(CMuted);
                    r.AutoItem()
                        .Text("ФотоМагазин © 2026")
                        .FontSize(8).FontColor(CMuted);
                });
            });
    }

    private static void ComposePersonCard(
        QContainer container,
        string label,
        string labelColor,
        string name,
        string? sub,
        string? badge)
    {
        container
            .Background(CBackground)
            .Border(1).BorderColor(CBorder)
            .Padding(10)
            .Column(c =>
            {
                c.Item()
                    .Text(label)
                    .FontSize(7).Bold().FontColor(labelColor);

                c.Item().Height(5);

                c.Item()
                    .Text(name)
                    .FontSize(9).Bold().FontColor(CMainText);

                if (sub is not null)
                    c.Item()
                        .Text(sub)
                        .FontSize(8.5f).FontColor(CSecondary);

                if (badge is not null)
                {
                    c.Item().Height(5);
                    c.Item()
                        .Background(CElevated)
                        .Border(1).BorderColor(CBorder)
                        .PaddingVertical(3)
                        .PaddingHorizontal(6)
                        .Text(badge)
                        .FontSize(7.5f).FontColor(CPrimary);
                }
            });
    }

    private static void AddSummaryRow(
        ColumnDescriptor col,
        string label,
        string value,
        string? valueColor = null)
    {
        col.Item().PaddingVertical(3).Row(r =>
        {
            r.RelativeItem()
                .Text(label)
                .FontSize(8.5f).FontColor(CSecondary);
            r.AutoItem()
                .Text(value)
                .FontSize(8.5f)
                .FontColor(valueColor ?? CMainText);
        });
    }

    private static string PluralItems(int count) => count switch
    {
        1 => "товар",
        2 or 3 or 4 => "товара",
        _ => "товаров"
    };
}
