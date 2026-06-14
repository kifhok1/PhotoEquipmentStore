using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Xaml.Interactivity;

namespace PhotoEquipmentStore.Behaviors;

public class WindowResizeBehavior : Behavior<Border>
{
    public static readonly StyledProperty<WindowEdge> EdgeProperty =
        AvaloniaProperty.Register<WindowResizeBehavior, WindowEdge>(nameof(Edge));

    public WindowEdge Edge
    {
        get => GetValue(EdgeProperty);
        set => SetValue(EdgeProperty, value);
    }

    private bool _isDragging;
    private PixelPoint _startPointerPos;
    private PixelPoint _startWindowPos;
    private Size _startWindowSize;
    private Window? _window;

    protected override void OnAttached()
    {
        base.OnAttached();
        if (AssociatedObject is null) return;
        AssociatedObject.PointerPressed += OnPointerPressed;
        AssociatedObject.PointerMoved += OnPointerMoved;
        AssociatedObject.PointerReleased += OnPointerReleased;
        AssociatedObject.PointerEntered += OnPointerEntered;
        AssociatedObject.PointerExited += OnPointerExited;
    }

    protected override void OnDetaching()
    {
        base.OnDetaching();
        if (AssociatedObject is null) return;
        AssociatedObject.PointerPressed -= OnPointerPressed;
        AssociatedObject.PointerMoved -= OnPointerMoved;
        AssociatedObject.PointerReleased -= OnPointerReleased;
        AssociatedObject.PointerEntered -= OnPointerEntered;
        AssociatedObject.PointerExited -= OnPointerExited;
    }

    private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (!e.GetCurrentPoint(AssociatedObject).Properties.IsLeftButtonPressed) return;
        _window = TopLevel.GetTopLevel(AssociatedObject) as Window;
    
        if (_window is null) return;

        _isDragging = true;
        _startPointerPos = _window.PointToScreen(e.GetPosition(_window));
        _startWindowPos = _window.Position;
        _startWindowSize = new Size(_window.Width, _window.Height);
    
        e.Pointer.Capture(AssociatedObject);
        e.Handled = true;
    }

    private void OnPointerMoved(object? sender, PointerEventArgs e)
    {
        if (!_isDragging || _window is null) return;

        var currentPos = _window.PointToScreen(e.GetPosition(_window));
        var dx = currentPos.X - _startPointerPos.X;
        var dy = currentPos.Y - _startPointerPos.Y;

        double left = _startWindowPos.X;
        double top = _startWindowPos.Y;
        var width = _startWindowSize.Width;
        var height = _startWindowSize.Height;

        switch (Edge)
        {
            case WindowEdge.East:
                width = Clamp(width + dx);
                break;
            case WindowEdge.West:
                width = Clamp(width - dx);
                if (width != _startWindowSize.Width)
                    left = _startWindowPos.X + (_startWindowSize.Width - width);
                break;
            case WindowEdge.South:
                height = Clamp(height + dy, isHeight: true);
                break;
            case WindowEdge.North:
                height = Clamp(height - dy, isHeight: true);
                if (height != _startWindowSize.Height)
                    top = _startWindowPos.Y + (_startWindowSize.Height - height);
                break;
            case WindowEdge.SouthEast:
                width = Clamp(width + dx);
                height = Clamp(height + dy, isHeight: true);
                break;
            case WindowEdge.SouthWest:
                width = Clamp(width - dx);
                if (width != _startWindowSize.Width)
                    left = _startWindowPos.X + (_startWindowSize.Width - width);
                height = Clamp(height + dy, isHeight: true);
                break;
            case WindowEdge.NorthEast:
                width = Clamp(width + dx);
                height = Clamp(height - dy, isHeight: true);
                if (height != _startWindowSize.Height)
                    top = _startWindowPos.Y + (_startWindowSize.Height - height);
                break;
            case WindowEdge.NorthWest:
                width = Clamp(width - dx);
                if (width != _startWindowSize.Width)
                    left = _startWindowPos.X + (_startWindowSize.Width - width);
                height = Clamp(height - dy, isHeight: true);
                if (height != _startWindowSize.Height)
                    top = _startWindowPos.Y + (_startWindowSize.Height - height);
                break;
        }

        _window.Position = new PixelPoint((int)left, (int)top);
        _window.Width = width;
        _window.Height = height;
    }

    private void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (!_isDragging) return;
        _isDragging = false;
        e.Pointer.Capture(null);
    }

    private double Clamp(double value, bool isHeight = false)
    {
        if (_window is null) return value;
        var min = isHeight ? _window.MinHeight : _window.MinWidth;
        var max = isHeight ? _window.MaxHeight : _window.MaxWidth;
        return Math.Max(min, Math.Min(max, value));
    }

    private void OnPointerEntered(object? sender, PointerEventArgs e)
    {
        var cursor = Edge switch
        {
            WindowEdge.North or WindowEdge.South => new Cursor(StandardCursorType.SizeNorthSouth),
            WindowEdge.West or WindowEdge.East => new Cursor(StandardCursorType.SizeWestEast),
            WindowEdge.NorthWest or WindowEdge.SouthEast => new Cursor(StandardCursorType.BottomRightCorner),
            WindowEdge.NorthEast or WindowEdge.SouthWest => new Cursor(StandardCursorType.BottomLeftCorner),
            _ => Cursor.Default
        };
        if (AssociatedObject is not null)
            AssociatedObject.Cursor = cursor;
    }

    private void OnPointerExited(object? sender, PointerEventArgs e)
    {
        if (AssociatedObject is not null)
            AssociatedObject.Cursor = Cursor.Default;
    }
}