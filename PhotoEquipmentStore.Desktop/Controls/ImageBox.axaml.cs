using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;

namespace PhotoEquipmentStore.Controls;

public partial class ImageBox : UserControl
{
    private static readonly string[] AllowedExtensions = [".png", ".jpg", ".jpeg"];
    private const long MaxFileSizeBytes = 2L * 1024 * 1024;

    public static readonly StyledProperty<Bitmap?> ImageSourceProperty =
        AvaloniaProperty.Register<ImageBox, Bitmap?>(nameof(ImageSource));

    public Bitmap? ImageSource
    {
        get => GetValue(ImageSourceProperty);
        set => SetValue(ImageSourceProperty, value);
    }

    private Bitmap? _ownedBitmap;

    // Кешируем после OnAttachedToVisualTree, когда DynamicResource уже резолвится
    private IBrush? _defaultBorderStroke;
    private IBrush? _defaultIconBorderBrush;
    private IBrush? _defaultIconStroke;

    static ImageBox()
    {
        ImageSourceProperty.Changed.AddClassHandler<ImageBox>(
            (box, e) => box.OnImageSourceChanged(e.NewValue as Bitmap));
    }

    public ImageBox()
    {
        InitializeComponent();
        DragDrop.SetAllowDrop(this, true);
        AddHandler(DragDrop.DragEnterEvent, OnDragEnter);
        AddHandler(DragDrop.DragLeaveEvent, OnDragLeave);
        AddHandler(DragDrop.DragOverEvent,  OnDragOver);
        AddHandler(DragDrop.DropEvent,      OnDrop);
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);

        // DynamicResource гарантированно резолвится к этому моменту
        _defaultBorderStroke    = DropBorderRect.Stroke;
        _defaultIconBorderBrush = IconCircleBorder.BorderBrush;
        _defaultIconStroke      = IconPath.Stroke;

        // Привязка могла выставить ImageSource до прикрепления — применяем состояние
        if (ImageSource is not null)
        {
            PreviewImage.Source = ImageSource;
            ShowImageState("Изображение загружено");
        }
        else
        {
            ShowDropState();
        }
    }

    private void OnImageSourceChanged(Bitmap? bitmap)
    {
        if (bitmap != _ownedBitmap)
        {
            _ownedBitmap?.Dispose();
            _ownedBitmap = null;
        }

        PreviewImage.Source = bitmap;

        if (bitmap is not null)
            ShowImageState("Изображение загружено");
        else
            ShowDropState();
    }

    private void ShowImageState(string label)
    {
        ImageLayer.IsVisible  = true;
        ClearButton.IsVisible = true;
        DropContent.IsVisible = false;
        Cursor = new Cursor(StandardCursorType.Arrow);

        FileInfoText.Text = label;

        DropBorderRect.StrokeDashArray = null;
        DropBorderRect.StrokeThickness = 1;
        DropBorderRect.Stroke          = _defaultBorderStroke;
    }

    private void ShowDropState()
    {
        ImageLayer.IsVisible  = false;
        ClearButton.IsVisible = false;
        DropContent.IsVisible = true;
        Cursor = new Cursor(StandardCursorType.Hand);

        DropBorderRect.StrokeDashArray = new AvaloniaList<double> { 2, 1 };
        DropBorderRect.StrokeThickness = 3;
        DropBorderRect.Stroke          = _defaultBorderStroke;
    }

    // SetHighlight использует кеш — ClearValue больше не нужен
    private void SetHighlight(bool active)
    {
        if (active)
        {
            var primary = this.FindResource("primary") as IBrush;
            DropBorderRect.Stroke        = primary;
            IconCircleBorder.BorderBrush = primary;
            IconPath.Stroke              = primary;
        }
        else
        {
            DropBorderRect.Stroke        = _defaultBorderStroke;
            IconCircleBorder.BorderBrush = _defaultIconBorderBrush;
            IconPath.Stroke              = _defaultIconStroke;
        }
    }

    protected override void OnPointerEntered(PointerEventArgs e) { base.OnPointerEntered(e); SetHighlight(true);  }
    protected override void OnPointerExited(PointerEventArgs e)  { base.OnPointerExited(e);  SetHighlight(false); }

    protected override async void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);
        if (ImageSource is not null) return;
        await PickFileAsync();
    }

    private void OnDragEnter(object? sender, DragEventArgs e)
    {
        if (!HasImageFiles(e)) { e.DragEffects = DragDropEffects.None; return; }
        e.DragEffects = DragDropEffects.Copy;
        SetHighlight(true);
    }
    private void OnDragLeave(object? sender, DragEventArgs e) => SetHighlight(false);
    private void OnDragOver(object? sender, DragEventArgs e) =>
        e.DragEffects = HasImageFiles(e) ? DragDropEffects.Copy : DragDropEffects.None;

    private void OnDrop(object? sender, DragEventArgs e)
    {
        SetHighlight(false);
        if (!e.Data.Contains(DataFormats.Files)) return;
        var path = e.Data.GetFiles()?.FirstOrDefault()?.TryGetLocalPath();
        if (path is not null) ProcessFile(path);
    }

    private void OnClearButtonClick(object? sender, RoutedEventArgs e)
    {
        e.Handled   = true;
        ImageSource = null;
    }

    private void ProcessFile(string path)
    {
        var ext = Path.GetExtension(path).ToLowerInvariant();
        if (!AllowedExtensions.Contains(ext)) return;

        var info = new FileInfo(path);
        if (!info.Exists || info.Length > MaxFileSizeBytes) return;

        try
        {
            var bitmap = new Bitmap(path);
            _ownedBitmap?.Dispose();
            _ownedBitmap = bitmap;

            ImageSource = bitmap;

            var size = FormatFileSize(info.Length);
            var name = Path.GetFileName(path);
            if (name.Length > 20) name = name[..20] + "...";
            FileInfoText.Text = $"{name} ({size})";
        }
        catch { }
    }

    private async Task PickFileAsync()
    {
        var topLevel = TopLevel.GetTopLevel(this);
        if (topLevel is null) return;

        var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title          = "Выберите изображение",
            AllowMultiple  = false,
            FileTypeFilter =
            [
                new FilePickerFileType("Изображения (PNG, JPG)")
                {
                    Patterns  = ["*.png", "*.jpg", "*.jpeg"],
                    MimeTypes = ["image/png", "image/jpeg"]
                }
            ]
        });

        var path = files.FirstOrDefault()?.TryGetLocalPath();
        if (path is not null) ProcessFile(path);
    }

    private static string FormatFileSize(long bytes) => bytes switch
    {
        >= 1024 * 1024 => $"{bytes / (1024.0 * 1024.0):F1} МБ",
        >= 1024        => $"{bytes / 1024.0:F0} КБ",
        _              => $"{bytes} Б"
    };

    private static bool HasImageFiles(DragEventArgs e)
    {
        if (!e.Data.Contains(DataFormats.Files)) return false;
        return e.Data.GetFiles()?.Any(f =>
        {
            var p = f.TryGetLocalPath();
            return p is not null &&
                   AllowedExtensions.Contains(Path.GetExtension(p).ToLowerInvariant());
        }) ?? false;
    }

    protected override void OnUnloaded(RoutedEventArgs e)
    {
        base.OnUnloaded(e);
        _ownedBitmap?.Dispose();
        _ownedBitmap = null;
    }
}