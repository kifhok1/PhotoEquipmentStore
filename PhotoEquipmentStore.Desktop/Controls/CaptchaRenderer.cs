using System;
using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace PhotoEquipmentStore.Controls;

public class CaptchaRenderer : Control
{
    private Color GetColor(string key, Color fallback)
    {
        if (this.TryFindResource(key, out var res) && res is SolidColorBrush brush)
            return brush.Color;
        return fallback;
    }
    
    private Color[] BuildCharColors() =>
    [
        Color.FromRgb(255, 180,  30),   // оранжевый
        Color.FromRgb(80, 130, 220),   // синий
        Color.FromRgb(60, 190, 100),   // зелёный
        Color.FromRgb(180,  80, 220),   // фиолетовый
        Color.FromRgb(255,  80,  80),   // красный
        Color.FromRgb(40, 210, 210),   // бирюзовый
        Color.FromRgb(255, 220,  50),   // жёлтый
        Color.FromRgb(255, 120, 180),   // розовый
        Color.FromRgb(100, 220, 255),   // голубой
        Color.FromRgb(255, 140,  50),   // янтарный
        Color.FromRgb(140, 255, 140),   // мятный
    ];

    private string  _text       = string.Empty;
    private float[] _charX      = [];
    private float[] _charY      = [];
    private float[] _charAngle  = [];
    private float[] _charSize   = [];
    private int[]   _charColor  = [];
    private bool[]  _charItalic = [];

    // EdgeA/EdgeB: 0=left, 1=right, 2=top, 3=bottom
    // PosA/PosB: позиция вдоль выбранного края [0..1]
    private record LineData(int EdgeA, float PosA, int EdgeB, float PosB, Color Color, float Thickness);
    private record DotData(float Xfrac, float Yfrac, float R, Color Color);

    private LineData[] _lines = [];
    private DotData[]  _dots  = [];

    // Переводим (edge, pos) в Point с учётом реальных размеров
    private static Point EdgePoint(int edge, float pos, double w, double h) => edge switch
    {
        0 => new Point(0,     pos * h),   // left
        1 => new Point(w,     pos * h),   // right
        2 => new Point(pos * w,   0),     // top
        _ => new Point(pos * w,   h),     // bottom
    };

    public void SetCaptcha(string text)
    {
        _text = text;
        var rng = new Random();
        int n = text.Length;
        var charColors = BuildCharColors();

        // ── Символы ──────────────────────────────────────────────────────
        _charX      = new float[n];
        _charY      = new float[n];
        _charAngle  = new float[n];
        _charSize   = new float[n];
        _charColor  = new int[n];
        _charItalic = new bool[n];

        for (int i = 0; i < n; i++)
        {
            _charSize[i]   = 17 + rng.Next(6);
            _charAngle[i]  = (float)((rng.NextDouble() - 0.5) * Math.PI / 9);
            _charColor[i]  = rng.Next(charColors.Length);
            _charItalic[i] = rng.Next(3) != 0;

            float slotW = 1f / n;
            _charX[i] = slotW * (i + 0.5f) + (float)(rng.NextDouble() - 0.5) * slotW * 0.3f;
            _charY[i] = 0.5f + (float)(rng.NextDouble() - 0.5) * 0.18f;
        }

        // ── 4–5 линий с произвольным направлением ────────────────────────
        Color[] lineColors =
        [
            Color.FromArgb(160, 200, 130,  40),
            Color.FromArgb(130,  70, 110, 210),
            Color.FromArgb( 90, 180, 180, 180),
            Color.FromArgb(140, 220,  80,  60),
            Color.FromArgb(110,  50, 180, 160),
        ];

        int lineCount = 4 + rng.Next(2);
        _lines = new LineData[lineCount];
        for (int i = 0; i < lineCount; i++)
        {
            // true = горизонтальная (left→right), false = вертикальная (top→bottom)
            bool horizontal = rng.Next(2) == 0;

            _lines[i] = new LineData(
                EdgeA:     horizontal ? 0 : 2,   // left или top
                PosA:      (float)rng.NextDouble(),
                EdgeB:     horizontal ? 1 : 3,   // right или bottom
                PosB:      (float)rng.NextDouble(),
                Color:     lineColors[i % lineColors.Length],
                Thickness: 1.0f + (float)rng.NextDouble() * 0.6f
            );
        }

        // ── 16–32 точек ───────────────────────────────────────────────────
        int dotCount = 16 + rng.Next(16);
        _dots = new DotData[dotCount];
        for (int i = 0; i < dotCount; i++)
        {
            _dots[i] = new DotData(
                Xfrac: (float)rng.NextDouble(),
                Yfrac: (float)rng.NextDouble(),
                R:     2f + (float)rng.NextDouble() * 2.5f,
                Color: charColors[rng.Next(charColors.Length)]
            );
        }

        InvalidateVisual();
    }

    public override void Render(DrawingContext ctx)
    {
        if (_text.Length == 0) return;

        double w = Bounds.Width;
        double h = Bounds.Height;
        var charColors = BuildCharColors();

        // ── Диагональная штриховка фона ───────────────────────────────────
        var stripeColor = GetColor("primary", Color.FromArgb(20, 250, 250, 250));
        var stripePen = new Pen(new SolidColorBrush(stripeColor, 0.1), 6);
        double step = 14;
        for (double x = -(h + step); x < w + h + step; x += step)
            ctx.DrawLine(stripePen, new Point(x, -5), new Point(x + h, h + 5));

        // ── Линии ─────────────────────────────────────────────────────────
        foreach (var l in _lines)
        {
            var pen = new Pen(new SolidColorBrush(l.Color), l.Thickness);
            ctx.DrawLine(pen,
                EdgePoint(l.EdgeA, l.PosA, w, h),
                EdgePoint(l.EdgeB, l.PosB, w, h)
            );
        }

        // ── Точки ─────────────────────────────────────────────────────────
        foreach (var d in _dots)
        {
            var brush = new SolidColorBrush(Color.FromArgb(200, d.Color.R, d.Color.G, d.Color.B));
            ctx.DrawEllipse(brush, null, new Point(d.Xfrac * w, d.Yfrac * h), d.R, d.R);
        }

        // ── Символы ───────────────────────────────────────────────────────
        for (int i = 0; i < _text.Length; i++)
        {
            var brush = new SolidColorBrush(charColors[_charColor[i]]);
            var typeface = new Typeface(
                FontFamily.Default,
                _charItalic[i] ? FontStyle.Italic : FontStyle.Normal,
                FontWeight.Bold
            );
            var ft = new FormattedText(
                _text[i].ToString(),
                CultureInfo.InvariantCulture,
                FlowDirection.LeftToRight,
                typeface,
                _charSize[i],
                brush
            );

            double cx = _charX[i] * w;
            double cy = _charY[i] * h;

            var m = Matrix.CreateTranslation(-cx, -cy)
                  * Matrix.CreateRotation(_charAngle[i])
                  * Matrix.CreateTranslation(cx, cy);

            using (ctx.PushTransform(m))
                ctx.DrawText(ft, new Point(cx - ft.Width / 2, cy - ft.Height / 2));
        }
    }
}