using System;
using System.Windows;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows.Media;

namespace Symirror3
{
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
            new MouseGestureHandler(null, DrawSuface_MouseDrag).Attach(drawSuface);
        }

        private void Window_Closing(object sender, CancelEventArgs e) => viewModel.Dispose();

        private void Window_Activated(object sender, EventArgs e) => viewModel?.Rotate(0f, 0f, 0f);

        private void DrawSuface_MouseDrag(object sender, MouseGestureEventArgs e)
        {
            if (e.Button == MouseButton.Right)
                viewModel.Rotate(0f, 0f, -e.Delta.X);
            else
                viewModel.Rotate(-e.Delta.X, -e.Delta.Y, 0f);
        }

        private void MapImage_MouseClick(object sender, MouseGestureEventArgs e)
        {
            if (e.Button == MouseButton.Left)
                viewModel.MoveBasePointTo(e.Position);
        }

        private void MapImage_MouseDrag(object sender, MouseGestureEventArgs e)
        {
            if (e.Button == MouseButton.Left)
                viewModel.ChangeBasePoint(e.Position);
            else if (e.Button == MouseButton.Right)
                viewModel.MoveBasePointRelative(-e.Delta.X, -e.Delta.Y);
        }
    }
}
