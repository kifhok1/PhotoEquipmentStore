using DocumentFormat.OpenXml.Spreadsheet;

namespace PhotoEquipmentStore.Application.Reports;
internal class StyleFactory
{
    private readonly Stylesheet       _styles;
    private readonly Fonts            _fonts   = new();
    private readonly Fills            _fills   = new();
    private readonly Borders          _borders = new();
    private readonly CellFormats      _formats = new();
    private readonly NumberingFormats _numFmts = new();

    private uint _fillIdx = 2;
    private uint _fontIdx = 1;
    private uint _fmtIdx  = 164;

    public StyleFactory(Stylesheet styles)
    {
        _styles = styles;
        _fonts.AppendChild(new Font(new FontSize { Val = 10 }, new FontName { Val = "Arial" }));
        _fills.AppendChild(new Fill(new PatternFill { PatternType = PatternValues.None }));
        _fills.AppendChild(new Fill(new PatternFill { PatternType = PatternValues.Gray125 }));
        _borders.AppendChild(new Border());
        _formats.AppendChild(new CellFormat { FontId = 0, FillId = 0, BorderId = 0 });

        styles.Fonts            = _fonts;
        styles.Fills            = _fills;
        styles.Borders          = _borders;
        styles.CellFormats      = _formats;
        styles.NumberingFormats = _numFmts;
    }

    public uint Header(string hexColor)
    {
        uint fontId = AddFont(true, "FFFFFF");
        uint fillId = AddFill(hexColor);
        return AddFormat(fontId, fillId, halign: HorizontalAlignmentValues.Center);
    }

    public uint MetaText()
    {
        uint fontId = AddFont(false, "808080");
        return AddFormat(fontId, 1);
    }

    public uint AltRow()
    {
        uint fillId = AddFill("F2F2F2");
        return AddFormat(0, fillId);
    }

    public uint TotalRow()
    {
        uint fontId = AddFont(true, "000000");
        uint fillId = AddFill("D9D9D9");
        return AddFormat(fontId, fillId);
    }

    public uint RedText()
    {
        uint fontId = AddFont(true, "E74C3C");
        return AddFormat(fontId, 1);
    }

    public uint Medal(string hexColor)
    {
        uint fontId = AddFont(true, "000000");
        uint fillId = AddFill(hexColor);
        return AddFormat(fontId, fillId, halign: HorizontalAlignmentValues.Center);
    }

    public uint Money()
    {
        uint numFmtId = _fmtIdx++;
        _numFmts.AppendChild(new NumberingFormat
        {
            NumberFormatId = numFmtId,
            FormatCode     = "#,##0.00"
        });
        return AddFormat(0, 1, numFmtId: numFmtId);
    }

    private uint AddFont(bool bold, string hexColor)
    {
        var font = new Font(
            new FontSize { Val = 10 },
            new FontName { Val = "Arial" },
            new Color    { Rgb = "FF" + hexColor });
        if (bold) font.AppendChild(new Bold());
        _fonts.AppendChild(font);
        return _fontIdx++;
    }

    private uint AddFill(string hexColor)
    {
        _fills.AppendChild(new Fill(new PatternFill
        {
            PatternType     = PatternValues.Solid,
            ForegroundColor = new ForegroundColor { Rgb = "FF" + hexColor }
        }));
        return _fillIdx++;
    }

    private uint AddFormat(
        uint fontId = 0, uint fillId = 1,
        HorizontalAlignmentValues? halign = null,
        uint numFmtId = 0)
    {
        var fmt = new CellFormat
        {
            FontId = fontId, FillId = fillId, BorderId = 0,
            ApplyFont = true, ApplyFill = true,
            NumberFormatId = numFmtId, ApplyNumberFormat = numFmtId > 0
        };
        if (halign.HasValue)
        {
            fmt.Alignment      = new Alignment { Horizontal = halign.Value };
            fmt.ApplyAlignment = true;
        }
        _formats.AppendChild(fmt);
        return (uint)(_formats.Count() - 1);
    }
}
