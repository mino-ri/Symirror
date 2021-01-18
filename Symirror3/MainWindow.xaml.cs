using System;
using System.Windows;
using System.ComponentModel;
using System.Windows.Forms;
using System.Windows.Input;
using MouseEventArgs = System.Windows.Forms.MouseEventArgs;

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
            viewModel = new ViewModel(drawSuface);
            DataContext = viewModel;
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            ((ViewModel)DataContext).Dispose();
        }

        int _oldX, _oldY;
        bool _pushing;
        private void DrawSuface_MouseDown(object sender, MouseEventArgs e)
        {
            _pushing = true;
            _oldX = e.X;
            _oldY = e.Y;
        }

        private void DrawSuface_MouseUp(object sender, MouseEventArgs e)
        {
            _pushing = false;
        }

        private void DrawSuface_MouseMove(object sender, MouseEventArgs e)
        {
            if (_pushing)
            {
                if (e.Button.HasFlag(MouseButtons.Right))
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

    }
}
