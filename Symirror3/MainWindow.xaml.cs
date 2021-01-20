using System;
using System.Windows;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows.Media;
using Win32 = System.Windows.Forms;

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
            viewModel = new ViewModel(drawSuface, (int)Math.Round(256.0 * dpi.DpiScaleX));
            DataContext = viewModel;
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            viewModel.Dispose();
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            viewModel?.Rotate(0f, 0f, 0f);
        }

        int _oldX, _oldY;
        bool _pushing;
        private void DrawSuface_MouseDown(object sender, Win32.MouseEventArgs e)
        {
            _pushing = true;
            _oldX = e.X;
            _oldY = e.Y;
        }

        private void DrawSuface_MouseUp(object sender, Win32.MouseEventArgs e)
        {
            _pushing = false;
        }

        private void DrawSuface_MouseMove(object sender, Win32.MouseEventArgs e)
        {
            if (_pushing)
            {
                if (e.Button.HasFlag(Win32.MouseButtons.Right))
                {
                    viewModel.MoveBasePoint(_oldX - e.X, _oldY - e.Y);
                }
                else if (Keyboard.Modifiers.HasFlag(ModifierKeys.Shift))
                {
                    viewModel.Rotate(0f, 0f, _oldX - e.X);
                }
                else
                {
                    viewModel.Rotate(_oldX - e.X, _oldY - e.Y, 0f);
                }

                _oldX = e.X;
                _oldY = e.Y;
            }
        }

        private void MapImage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var p = e.GetPosition(MapImage);
                viewModel.ChangeBasePoint(new Point(p.X / 128.0 - 1.0, p.Y / 128.0 - 1.0));
            }
        }

        private void MapImage_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {   
                var p = e.GetPosition(MapImage);
                viewModel.ChangeBasePoint(new Point(p.X / 128.0 - 1.0, p.Y / 128.0 - 1.0));
            }
        }
    }
}
