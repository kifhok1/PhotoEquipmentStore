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

    public static readonly StyledProperty<string?> ImagePathProperty =
        AvaloniaProperty.Register<ImageBox, string?>(nameof(ImagePath));

    public string? ImagePath
    {
        get => GetValue(ImagePathProperty);
        set => SetValue(ImagePathProperty, value);
    }

    public event EventHandler<string>? ImageSelected;

    private Bitmap? _currentBitmap;

    private IBrush? _defaultBorderStroke;
    private IBrush? _defaultIconBorderBrush;
    private IBrush? _defaultIconStroke;

    static ImageBox()
    {
        ImagePathProperty.Changed.AddClassHandler<ImageBox>(
            (box, e) => box.OnImagePathChanged(e.NewValue as string));
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
        _defaultBorderStroke    = DropBorderRect.Stroke;
        _defaultIconBorderBrush = IconCircleBorder.BorderBrush;
        _defaultIconStroke      = IconPath.Stroke;
    }

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

    protected override void OnPointerEntered(PointerEventArgs e)
    {
        base.OnPointerEntered(e);
        SetHighlight(true);
    }

    protected override void OnPointerExited(PointerEventArgs e)
    {
        base.OnPointerExited(e);
        SetHighlight(false);
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

    protected override async void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);
        if (ImagePath is not null) return;
        await PickFileAsync();
    }

    private void OnImagePathChanged(string? path)
    {
        _currentBitmap?.Dispose();
        _currentBitmap = null;
        PreviewImage.Source = null;

        if (path is not null && File.Exists(path))
        {
            try
            {
                _currentBitmap = new Bitmap(path);
                PreviewImage.Source = _currentBitmap;
                ShowImageState(path);
                return;
            }
            catch
            {
                
            }
        }

        ShowDropState();
    }

    private void ShowImageState(string path)
    {
        ImageLayer.IsVisible  = true;
        ClearButton.IsVisible = true;
        DropContent.IsVisible = false;
        Cursor = new Cursor(StandardCursorType.Arrow);
 
        var info   = new FileInfo(path);
        var size   = FormatFileSize(info.Length);
        var name = Path.GetFileName(path);
        if (name.Length > 20)
        {
            name = name.Substring(0, 20) + "... ";
        }
        FileInfoText.Text = $"{name} ({size})";
 
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
    }

    private static string FormatFileSize(long bytes) => bytes switch
    {
        >= 1024 * 1024 => $"{bytes / (1024.0 * 1024.0):F1} МБ",
        >= 1024        => $"{bytes / 1024.0:F0} КБ",
        _              => $"{bytes} Б"
    };

    private void OnClearButtonClick(object? sender, RoutedEventArgs e)
    {
        e.Handled = true;
        ImagePath = null;
    }

    private async Task PickFileAsync()
    {
        var topLevel = TopLevel.GetTopLevel(this);
        if (topLevel is null) return;

        var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title         = "Выберите изображение",
            AllowMultiple = false,
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

    private void ProcessFile(string path)
    {
        var ext = Path.GetExtension(path).ToLowerInvariant();
        if (!AllowedExtensions.Contains(ext)) return;

        var info = new FileInfo(path);
        if (!info.Exists || info.Length > MaxFileSizeBytes) return;

        ImagePath = path;
        ImageSelected?.Invoke(this, path);
    }

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
        _currentBitmap?.Dispose();
        _currentBitmap = null;
    }
}