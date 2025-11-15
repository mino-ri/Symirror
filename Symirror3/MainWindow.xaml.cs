using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;

namespace Symirror3;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private bool _graphicsResizeRequired;
    private ViewModel viewModel = null!;

    public MainWindow()
    {
        InitializeComponent();
    }

    protected override void OnSourceInitialized(EventArgs e)
    {
        base.OnSourceInitialized(e);
        var source = (HwndSource)PresentationSource.FromVisual(this);
        source?.AddHook(WndProc);
    }

    private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
        const int WM_EXITSIZEMOVE = 0x0232;

        if (viewModel is { } && drawSurface is { } && msg == WM_EXITSIZEMOVE)
        {
            viewModel.WindowSizeChanged(drawSurface.Width, drawSurface.Height);
        }

        return IntPtr.Zero;
    }

    private void Window_ContentRendered(object sender, EventArgs e)
    {
        var dpi = VisualTreeHelper.GetDpi(MapImage);
        viewModel = new ViewModel(drawSurface, 256.0, dpi.DpiScaleX);
        DataContext = viewModel;
        MinHeight = ActualHeight;
        MinWidth = ActualWidth;
        new MouseGestureHandler(null, DrawSurface_MouseDrag).Attach(drawSurface);
    }

    private void Window_Closing(object sender, CancelEventArgs e) => viewModel.Dispose();

    private void Window_Activated(object sender, EventArgs e) => viewModel?.Rotate(0f, 0f, 0f);

    private void DrawSurface_MouseDrag(object sender, MouseGestureEventArgs e)
    {
        if (e.Button == MouseButton.Right)
        {
            viewModel.Rotate(0f, 0f, -e.Delta.X);
            e.Handled = true;
        }
        else
        {
            viewModel.Rotate(-e.Delta.X, -e.Delta.Y, 0f);
            e.Handled = true;
        }
    }

    private void MapImage_MouseClick(object sender, MouseGestureEventArgs e)
    {
        if (e.Button == MouseButton.Left)
            viewModel.MoveBasePointTo(e.Position);
    }

    private void MapImage_MouseDrag(object sender, MouseGestureEventArgs e)
    {
        if (e.Button == MouseButton.Left && !e.IsInClickableRange())
        {
            viewModel.ChangeBasePoint(e.Position);
            e.Handled = true;
        }
        else if (e.Button == MouseButton.Right)
        {
            viewModel.MoveBasePointRelative(-e.Delta.X, -e.Delta.Y);
            e.Handled = true;
        }
    }

    private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (viewModel is { } && drawSurface is { })
        {
            viewModel?.WindowSizeChanging();
        }
    }

    private void Window_StateChanged(object sender, EventArgs e)
    {
        if (WindowState is WindowState.Normal or WindowState.Maximized && viewModel is { } && drawSurface is { })
        {
            _graphicsResizeRequired = true;
        }
    }

    private void drawSurface_SizeChanged(object sender, EventArgs e)
    {
        if (!_graphicsResizeRequired)
            return;

        _graphicsResizeRequired = false;
        viewModel.WindowSizeChanged(drawSurface.Width, drawSurface.Height);
    }
}
