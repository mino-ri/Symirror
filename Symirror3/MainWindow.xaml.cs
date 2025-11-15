using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Symirror3;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private ViewModel viewModel = null!;

    public MainWindow()
    {
        InitializeComponent();
    }

    private void Window_ContentRendered(object sender, EventArgs e)
    {
        var dpi = VisualTreeHelper.GetDpi(MapImage);
        viewModel = new ViewModel(drawSuface, 256.0, dpi.DpiScaleX);
        DataContext = viewModel;
        new MouseGestureHandler(null, DrawSurface_MouseDrag).Attach(drawSuface);
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
}
