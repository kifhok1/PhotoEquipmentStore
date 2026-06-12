using System;
using System.Collections.Generic;
using System.Linq;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using PhotoEquipmentStore.Application.DTO;
using PhotoEquipmentStore.Domain.Entities;
using PhotoEquipmentStore.Domain.Enums;
using C   = DocumentFormat.OpenXml.Drawing.Charts;
using A   = DocumentFormat.OpenXml.Drawing;
using Xdr = DocumentFormat.OpenXml.Drawing.Spreadsheet;

namespace PhotoEquipmentStore.Application.Reports;

public static class ReportBuilder
{
    // ── Отчёт 1: Продажи ─────────────────────────────────────────────────────

    public static void BuildSalesReport(
        string path,
        List<SalesReportData> data,
        DateTime from,
        DateTime to)
    {
        using var doc = SpreadsheetDocument.Create(path, SpreadsheetDocumentType.Workbook);
        var wb = CreateWorkbook(doc);
        var (wsPart, sheetData, styles) = AddSheet(doc, wb, "Продажи");

        var sf = new StyleFactory(styles);
        uint hStyle     = sf.Header("4E6EF5");
        uint mStyle     = sf.MetaText();
        uint mStyle2    = sf.Money();
        uint altStyle   = sf.AltRow();
        uint totalStyle = sf.TotalRow();

        int row = 1;
        AppendMeta(sheetData, ref row, mStyle,
            "ФотоМагазин — Фотооборудование",
            "ОТЧЁТ ПО ПРОДАЖАМ",
            $"Период: {from:dd.MM.yyyy} — {to:dd.MM.yyyy}",
            $"Дата формирования: {DateTime.Now:dd.MM.yyyy HH:mm}",
            $"Количество заказов: {data.Count}");

        string[] headers =
        {
            "№", "Артикул", "Дата заказа", "Клиент", "Телефон",
            "Сотрудник", "Статус", "Скидка, %", "Позиций", "Кол-во товаров", "Сумма, ₽"
        };
        AppendHeaderRow(sheetData, ref row, headers, hStyle);

        for (int i = 0; i < data.Count; i++)
        {
            var r   = data[i];
            uint cs = i % 2 == 0 ? altStyle : 0u;
            sheetData.AppendChild(NewRow(row++, new List<Cell>
            {
                Num(i + 1, cs),
                Str(r.OrderId, cs),
                Str(r.OrderDate.ToString("dd.MM.yyyy HH:mm"), cs),
                Str(r.ClientName, cs),
                Str(r.ClientPhone, cs),
                Str(r.EmployeeName, cs),
                Str(r.StatusName, cs),
                Num(r.Discount, cs),
                Num(r.ItemsCount, cs),
                Num(r.TotalQuantity, cs),
                Money(r.TotalSum, mStyle2)
            }));
        }

        sheetData.AppendChild(NewRow(row, new List<Cell>
        {
            Str("ИТОГО", totalStyle), Str("", totalStyle), Str("", totalStyle),
            Str("", totalStyle),      Str("", totalStyle), Str("", totalStyle),
            Str("", totalStyle),      Str("", totalStyle),
            Num(data.Sum(r => r.ItemsCount),    totalStyle),
            Num(data.Sum(r => r.TotalQuantity), totalStyle),
            Money(data.Sum(r => r.TotalSum),    totalStyle)
        }));

        SetColumnWidths(wsPart, 6, 22, 18, 28, 18, 24, 18, 14, 10, 14, 14);

        var byDay = data
            .GroupBy(r => r.OrderDate.Date)
            .OrderBy(g => g.Key)
            .ToList();

        if (byDay.Count > 1)
        {
            var labels = byDay.Select(g => g.Key.ToString("dd.MM")).ToList();
            var values = byDay.Select(g => (double)g.Sum(r => r.TotalSum)).ToList();
            AddChartSheet(doc, wb, "График продаж", "Выручка по дням",
                          labels, values, ChartKind.Bar, "4E6EF5");
        }

        doc.Save();
    }

    // ── Отчёт 2: Остатки ─────────────────────────────────────────────────────

    public static void BuildStockReport(
        string path,
        List<StockReportData> data,
        string categoryFilter)
    {
        using var doc = SpreadsheetDocument.Create(path, SpreadsheetDocumentType.Workbook);
        var wb = CreateWorkbook(doc);
        var (wsPart, sheetData, styles) = AddSheet(doc, wb, "Остатки");

        var sf = new StyleFactory(styles);
        uint hStyle     = sf.Header("2ECC71");
        uint mStyle     = sf.MetaText();
        uint altStyle   = sf.AltRow();
        uint totalStyle = sf.TotalRow();
        uint redStyle   = sf.RedText();

        int row = 1;
        AppendMeta(sheetData, ref row, mStyle,
            "ФотоМагазин — Фотооборудование",
            "ОСТАТКИ ТОВАРОВ НА СКЛАДЕ",
            $"Категория: {categoryFilter}",
            $"Дата формирования: {DateTime.Now:dd.MM.yyyy HH:mm}",
            $"Позиций: {data.Count}");

        string[] headers =
        {
            "№", "Наименование", "Категория", "Производитель", "Поставщик", "Остаток, шт."
        };
        AppendHeaderRow(sheetData, ref row, headers, hStyle);

        for (int i = 0; i < data.Count; i++)
        {
            var r    = data[i];
            uint cs  = i % 2 == 0 ? altStyle : 0u;
            uint qSt = r.Quantity == 0 ? redStyle : cs;
            sheetData.AppendChild(NewRow(row++, new List<Cell>
            {
                Num(i + 1, cs),
                Str(r.ProductName, cs),
                Str(r.CategoryName, cs),
                Str(r.ManufacturerName, cs),
                Str(r.SupplierName, cs),
                Num(r.Quantity, qSt)
            }));
        }

        sheetData.AppendChild(NewRow(row, new List<Cell>
        {
            Str("ИТОГО", totalStyle), Str("", totalStyle), Str("", totalStyle),
            Str("", totalStyle),      Str("", totalStyle),
            Num(data.Sum(r => r.Quantity), totalStyle)
        }));

        SetColumnWidths(wsPart, 6, 30, 20, 22, 22, 14);

        var byCategory = data
            .GroupBy(r => r.CategoryName)
            .OrderByDescending(g => g.Sum(x => x.Quantity))
            .ToList();

        if (byCategory.Count > 1)
        {
            var labels = byCategory.Select(g => g.Key).ToList();
            var values = byCategory.Select(g => (double)g.Sum(x => x.Quantity)).ToList();
            AddChartSheet(doc, wb, "Диаграмма остатков", "Остатки по категориям",
                          labels, values, ChartKind.Pie, "2ECC71");
        }

        doc.Save();
    }

    // ── Отчёт 3: Популярность ────────────────────────────────────────────────

    public static void BuildPopularityReport(
        string path,
        List<PopularityReportData> data,
        string categoryFilter,
        PopularityMode mode)
    {
        using var doc = SpreadsheetDocument.Create(path, SpreadsheetDocumentType.Workbook);
        var wb = CreateWorkbook(doc);
        var (wsPart, sheetData, styles) = AddSheet(doc, wb, "Популярность");

        var sf = new StyleFactory(styles);
        uint hStyle      = sf.Header("F5A623");
        uint mStyle      = sf.MetaText();
        uint altStyle    = sf.AltRow();
        uint totalStyle  = sf.TotalRow();
        uint goldStyle   = sf.Medal("FFD700");
        uint silverStyle = sf.Medal("C0C0C0");
        uint bronzeStyle = sf.Medal("CD7F32");
        uint moneyStyle  = sf.Money();

        string title = mode switch
        {
            PopularityMode.AllDesc  => "ВСЕ ТОВАРЫ ПО УБЫВАНИЮ ПОПУЛЯРНОСТИ",
            PopularityMode.AllAsc   => "ВСЕ ТОВАРЫ ПО ВОЗРАСТАНИЮ ПОПУЛЯРНОСТИ",
            PopularityMode.Top30    => "30 САМЫХ ПОПУЛЯРНЫХ ТОВАРОВ",
            PopularityMode.Bottom30 => "30 САМЫХ НЕПОПУЛЯРНЫХ ТОВАРОВ",
            _                       => "ПОПУЛЯРНОСТЬ ТОВАРОВ"
        };

        int row = 1;
        AppendMeta(sheetData, ref row, mStyle,
            "ФотоМагазин — Фотооборудование",
            title,
            $"Категория: {categoryFilter}",
            $"Дата формирования: {DateTime.Now:dd.MM.yyyy HH:mm}",
            $"Позиций: {data.Count}");

        string[] headers = { "Место", "Наименование", "Категория", "Цена, ₽", "Продано, шт.", "Заказов" };
        AppendHeaderRow(sheetData, ref row, headers, hStyle);

        bool medalTop = mode is PopularityMode.AllDesc or PopularityMode.Top30;

        for (int i = 0; i < data.Count; i++)
        {
            var r   = data[i];
            uint cs = i % 2 == 0 ? altStyle : 0u;
            uint rnk = medalTop
                ? i switch { 0 => goldStyle, 1 => silverStyle, 2 => bronzeStyle, _ => cs }
                : cs;

            sheetData.AppendChild(NewRow(row++, new List<Cell>
            {
                Num(r.Rank, rnk),
                Str(r.ProductName, cs),
                Str(r.CategoryName, cs),
                Num(r.Price, moneyStyle),
                Num(r.TotalSold, cs),
                Num(r.OrdersCount, cs)
            }));
        }

        sheetData.AppendChild(NewRow(row, new List<Cell>
        {
            Str("ИТОГО", totalStyle), Str("", totalStyle), Str("", totalStyle),
            Str("", totalStyle),
            Num(data.Sum(r => r.TotalSold),   totalStyle),
            Num(data.Sum(r => r.OrdersCount), totalStyle)
        }));

        SetColumnWidths(wsPart, 8, 34, 22, 14, 14, 12);

        // График: топ-15 из выборки
        var chartData = data.Take(15).ToList();
        if (chartData.Count > 0)
        {
            var labels = chartData.Select(x =>
                x.ProductName.Length > 20 ? x.ProductName[..20] + "…" : x.ProductName).ToList();
            var values = chartData.Select(x => (double)x.TotalSold).ToList();
            string chartTitle = mode switch
            {
                PopularityMode.AllDesc  => "Топ-15 самых продаваемых",
                PopularityMode.AllAsc   => "Топ-15 наименее продаваемых",
                PopularityMode.Top30    => "30 самых популярных товаров",
                PopularityMode.Bottom30 => "30 наименее популярных товаров",
                _                       => "Популярность"
            };
            AddChartSheet(doc, wb, "График популярности", chartTitle,
                          labels, values, ChartKind.Bar, "F5A623");
        }

        doc.Save();
    }

    // ── Chart Sheet ───────────────────────────────────────────────────────────

    private enum ChartKind { Bar, Pie }

    private static void AddChartSheet(
        SpreadsheetDocument doc,
        Workbook wb,
        string sheetName,
        string chartTitle,
        List<string> labels,
        List<double> values,
        ChartKind kind,
        string hexColor)
    {
        var wbPart = doc.WorkbookPart!;
        var sheets = wb.GetFirstChild<Sheets>()!;

        // 1. Скрытый лист с данными для графика
        var dataPart     = wbPart.AddNewPart<WorksheetPart>();
        var dataSd       = new SheetData();
        dataPart.Worksheet = new Worksheet(dataSd);
        string dataSheetName = sheetName + "_data";

        sheets.AppendChild(new Sheet
        {
            Id      = wbPart.GetIdOfPart(dataPart),
            SheetId = (uint)(sheets.ChildElements.Count + 1),
            Name    = dataSheetName,
            State   = SheetStateValues.Hidden
        });

        for (int i = 0; i < labels.Count; i++)
        {
            var r = new Row { RowIndex = (uint)(i + 1) };
            r.AppendChild(Str(labels[i]));
            r.AppendChild(new Cell
            {
                DataType  = CellValues.Number,
                CellValue = new CellValue(values[i])
            });
            dataSd.AppendChild(r);
        }

        // 2. Видимый лист (пустой — только Drawing с диаграммой)
        var chartWsPart = wbPart.AddNewPart<WorksheetPart>();
        var chartSd     = new SheetData();   // ячеек нет
        chartWsPart.Worksheet = new Worksheet(chartSd);

        sheets.AppendChild(new Sheet
        {
            Id      = wbPart.GetIdOfPart(chartWsPart),
            SheetId = (uint)(sheets.ChildElements.Count + 1),
            Name    = sheetName
        });

        // 3. DrawingsPart → ChartPart (один, не два!)
        var drawingPart = chartWsPart.AddNewPart<DrawingsPart>();
        chartWsPart.Worksheet.Append(new Drawing
        {
            Id = chartWsPart.GetIdOfPart(drawingPart)
        });

        var chartPart = drawingPart.AddNewPart<ChartPart>();
        chartPart.ChartSpace = kind == ChartKind.Pie
            ? BuildPieChartSpace(chartTitle, dataSheetName, labels.Count)
            : BuildBarChartSpace(chartTitle, hexColor, dataSheetName, labels.Count);

        // 4. AbsoluteAnchor — диаграмма на весь лист (~24×18 см в EMU)
        drawingPart.WorksheetDrawing = new Xdr.WorksheetDrawing();

        var anchor = new Xdr.AbsoluteAnchor(
            new Xdr.Position { X = 0L, Y = 0L },
            new Xdr.Extent   { Cx = 8640000L, Cy = 6480000L });

        var gf = new Xdr.GraphicFrame { Macro = "" };
        gf.Append(new Xdr.NonVisualGraphicFrameProperties(
            new Xdr.NonVisualDrawingProperties { Id = 2U, Name = "Chart 1" },
            new Xdr.NonVisualGraphicFrameDrawingProperties()));
        gf.Append(new Xdr.Transform(
            new A.Offset  { X = 0L, Y = 0L },
            new A.Extents { Cx = 0L, Cy = 0L }));

        var gfx     = new A.Graphic();
        var gfxData = new A.GraphicData
            { Uri = "http://schemas.openxmlformats.org/drawingml/2006/chart" };
        gfxData.Append(new C.ChartReference
            { Id = drawingPart.GetIdOfPart(chartPart) });
        gfx.Append(gfxData);
        gf.Append(gfx);

        anchor.Append(gf);
        anchor.Append(new Xdr.ClientData());
        drawingPart.WorksheetDrawing.Append(anchor);
    }

    // ── PieChart ──────────────────────────────────────────────────────────────

    private static ChartSpace BuildPieChartSpace(
        string title, string sheetName, int count)
    {
        string[] palette =
        {
            "4E6EF5", "2ECC71", "F5A623", "3498DB",
            "E74C3C", "F39C12", "9B59B6", "1ABC9C",
            "E67E22", "2980B9", "27AE60", "8E44AD"
        };

        var cs = new ChartSpace();
        cs.Append(new EditingLanguage { Val = "ru-RU" });

        var chart = new C.Chart();
        chart.Append(new AutoTitleDeleted { Val = false });
        chart.Append(MakeTitle(title));

        var plotArea = new C.PlotArea();
        var pie      = new C.PieChart();
        pie.Append(new C.VaryColors { Val = true });

        var ser = new C.PieChartSeries();
        ser.Append(new C.Index { Val = 0 });
        ser.Append(new C.Order { Val = 0 });

        for (int i = 0; i < count; i++)
        {
            var dp = new C.DataPoint();
            dp.Append(new C.Index { Val = (uint)i });
            dp.Append(new C.Bubble3D { Val = false });
            var spPr = new C.ShapeProperties();
            var sf   = new A.SolidFill();
            sf.Append(new A.RgbColorModelHex { Val = palette[i % palette.Length] });
            spPr.Append(sf);
            dp.Append(spPr);
            ser.Append(dp);
        }

        var cat    = new C.CategoryAxisData();
        var strRef = new C.StringReference();
        strRef.Append(new C.Formula($"'{sheetName}'!$A$1:$A${count}"));
        cat.Append(strRef);
        ser.Append(cat);

        var val    = new C.Values();
        var numRef = new C.NumberReference();
        numRef.Append(new C.Formula($"'{sheetName}'!$B$1:$B${count}"));
        val.Append(numRef);
        ser.Append(val);

        pie.Append(ser);
        plotArea.Append(pie);
        chart.Append(plotArea);

        var legend = new C.Legend();
        legend.Append(new C.LegendPosition { Val = C.LegendPositionValues.Right });
        chart.Append(legend);

        chart.Append(new C.PlotVisibleOnly { Val = true });
        cs.Append(chart);
        return cs;
    }

    // ── BarChart ──────────────────────────────────────────────────────────────

    private static ChartSpace BuildBarChartSpace(
        string title, string hexColor, string sheetName, int dataCount)
    {
        var cs = new ChartSpace();
        cs.Append(new EditingLanguage { Val = "ru-RU" });

        var chart = new C.Chart();
        chart.Append(new AutoTitleDeleted { Val = false });
        chart.Append(MakeTitle(title));

        var plotArea = new C.PlotArea();
        var barChart = new C.BarChart();
        barChart.Append(new C.BarDirection { Val = C.BarDirectionValues.Column });
        barChart.Append(new C.BarGrouping  { Val = C.BarGroupingValues.Clustered });
        barChart.Append(new C.VaryColors   { Val = false });

        var ser = new C.BarChartSeries();
        ser.Append(new C.Index { Val = 0 });
        ser.Append(new C.Order { Val = 0 });

        var spPr = new C.ShapeProperties();
        var fill = new A.SolidFill();
        fill.Append(new A.RgbColorModelHex { Val = hexColor });
        spPr.Append(fill);
        ser.Append(spPr);

        var cat    = new C.CategoryAxisData();
        var strRef = new C.StringReference();
        strRef.Append(new C.Formula($"'{sheetName}'!$A$1:$A${dataCount}"));
        cat.Append(strRef);
        ser.Append(cat);

        var val    = new C.Values();
        var numRef = new C.NumberReference();
        numRef.Append(new C.Formula($"'{sheetName}'!$B$1:$B${dataCount}"));
        val.Append(numRef);
        ser.Append(val);

        barChart.Append(ser);
        barChart.Append(new C.AxisId { Val = 1 });
        barChart.Append(new C.AxisId { Val = 2 });

        var catAx = new C.CategoryAxis();
        catAx.Append(new C.AxisId       { Val = 1 });
        catAx.Append(new C.Scaling(new C.Orientation { Val = C.OrientationValues.MinMax }));
        catAx.Append(new C.Delete       { Val = false });
        catAx.Append(new C.AxisPosition { Val = C.AxisPositionValues.Bottom });
        catAx.Append(new C.CrossingAxis { Val = 2 });

        var valAx = new C.ValueAxis();
        valAx.Append(new C.AxisId       { Val = 2 });
        valAx.Append(new C.Scaling(new C.Orientation { Val = C.OrientationValues.MinMax }));
        valAx.Append(new C.Delete       { Val = false });
        valAx.Append(new C.AxisPosition { Val = C.AxisPositionValues.Left });
        valAx.Append(new C.CrossingAxis { Val = 1 });
        valAx.Append(new C.CrossBetween { Val = C.CrossBetweenValues.Between });

        plotArea.Append(barChart);
        plotArea.Append(catAx);
        plotArea.Append(valAx);
        chart.Append(plotArea);

        var legend = new C.Legend();
        legend.Append(new C.LegendPosition { Val = C.LegendPositionValues.Bottom });
        chart.Append(legend);

        chart.Append(new C.PlotVisibleOnly { Val = true });
        cs.Append(chart);
        return cs;
    }

    // ── Title helper ──────────────────────────────────────────────────────────

    private static C.Title MakeTitle(string text)
    {
        var titleEl = new C.Title();
        var tx      = new C.ChartText();
        var rich    = new C.RichText();
        rich.Append(new A.BodyProperties());
        rich.Append(new A.ListStyle());
        var para = new A.Paragraph();
        var run  = new A.Run();
        run.Append(new A.RunProperties { Language = "ru-RU", Bold = true });
        run.Append(new A.Text(text));
        para.Append(run);
        rich.Append(para);
        tx.Append(rich);
        titleEl.Append(tx);
        titleEl.Append(new C.Overlay { Val = false });
        return titleEl;
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private static Workbook CreateWorkbook(SpreadsheetDocument doc)
    {
        var wbPart = doc.AddWorkbookPart();
        wbPart.Workbook = new Workbook();
        return wbPart.Workbook;
    }

    private static (WorksheetPart, SheetData, Stylesheet) AddSheet(
        SpreadsheetDocument doc, Workbook wb, string name)
    {
        var wbPart    = doc.WorkbookPart!;
        var wsPart    = wbPart.AddNewPart<WorksheetPart>();
        var sheetData = new SheetData();
        wsPart.Worksheet = new Worksheet(sheetData);

        var stylesPart = wbPart.AddNewPart<WorkbookStylesPart>();
        var styles     = new Stylesheet();
        stylesPart.Stylesheet = styles;

        var sheets = wb.AppendChild(new Sheets());
        sheets.AppendChild(new Sheet
        {
            Id      = wbPart.GetIdOfPart(wsPart),
            SheetId = 1,
            Name    = name
        });

        return (wsPart, sheetData, styles);
    }

    private static void AppendMeta(SheetData sd, ref int row, uint style, params string[] lines)
    {
        foreach (var line in lines)
        {
            var r = new Row { RowIndex = (uint)row++ };
            r.AppendChild(Str(line, style));
            sd.AppendChild(r);
        }
    }

    private static void AppendHeaderRow(SheetData sd, ref int row, string[] headers, uint style)
    {
        var r = new Row { RowIndex = (uint)row++ };
        foreach (var h in headers) r.AppendChild(Str(h, style));
        sd.AppendChild(r);
    }

    private static Row NewRow(int rowIndex, List<Cell> cells)
    {
        var r = new Row { RowIndex = (uint)rowIndex };
        foreach (var c in cells) r.AppendChild(c);
        return r;
    }

    private static Cell Str(string value, uint styleIndex = 0) => new()
    {
        DataType   = CellValues.String,
        CellValue  = new CellValue(value),
        StyleIndex = styleIndex
    };

    private static Cell Num(int value, uint styleIndex = 0) => new()
    {
        DataType   = CellValues.Number,
        CellValue  = new CellValue(value),
        StyleIndex = styleIndex
    };

    private static Cell Money(decimal value, uint styleIndex) => new()
    {
        DataType   = CellValues.Number,
        CellValue  = new CellValue((double)value),
        StyleIndex = styleIndex
    };

    private static void SetColumnWidths(WorksheetPart wsPart, params double[] widths)
    {
        var cols = new Columns();
        for (int i = 0; i < widths.Length; i++)
        {
            cols.AppendChild(new Column
            {
                Min = (uint)(i + 1), Max = (uint)(i + 1),
                Width = widths[i], CustomWidth = true
            });
        }
        wsPart.Worksheet.InsertBefore(cols, wsPart.Worksheet.GetFirstChild<SheetData>()!);
    }
}