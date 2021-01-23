using System;
using System.Windows;
using System.Windows.Input;
using Win32 = System.Windows.Forms;

namespace Symirror3
{
    public class MouseGestureHandler
    {
        public const double ClickableRange = 5d;
        public const double ClickableRangeSquared = ClickableRange * ClickableRange;
        public static MouseGestureHandler GetAttachedHandler(DependencyObject obj) => (MouseGestureHandler)obj.GetValue(AttachedHandlerProperty);
        public static void SetAttachedHandler(DependencyObject obj, MouseGestureHandler value) => obj.SetValue(AttachedHandlerProperty, value);

        public static readonly DependencyProperty AttachedHandlerProperty =
            DependencyProperty.RegisterAttached("AttachedHandler",
                typeof(MouseGestureHandler),
                typeof(FrameworkElement),
                new FrameworkPropertyMetadata(null, OnAttachedHandlerChanged));

        private static void OnAttachedHandlerChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is not FrameworkElement target) return;

            if (e.OldValue is MouseGestureHandler oldValue)
            {
                target.MouseDown -= oldValue.Wpf_MouseDown;
                target.MouseMove -= oldValue.Wpf_MouseMove;
                target.MouseUp -= oldValue.Wpf_MouseUp;
            }

            if (e.NewValue is MouseGestureHandler newValue)
            {
                target.MouseDown += newValue.Wpf_MouseDown;
                target.MouseMove += newValue.Wpf_MouseMove;
                target.MouseUp += newValue.Wpf_MouseUp;
            }
        }

        public MouseGestureHandler() { }

        public MouseGestureHandler(MouseGestureEventHandler? mouseClick, MouseGestureEventHandler? mouseDrag)
        {
            if (mouseClick is not null) MouseClick += mouseClick;
            if (mouseDrag is not null) MouseDrag += mouseDrag;
        }

        public void Attach(Win32.Control control)
        {
            control.MouseDown += Win32_MouseDown;
            control.MouseMove += Win32_MouseMove;
            control.MouseUp += Win32_MouseUp;
        }

        public void Detach(Win32.Control control)
        {
            control.MouseDown += Win32_MouseDown;
            control.MouseMove += Win32_MouseMove;
            control.MouseUp += Win32_MouseUp;
        }

        const MouseButton EmptyMouseButton = (MouseButton)(-1);
        Point _mouseDownPoint, _mouseBeforePoint;
        MouseButton _targetButton = EmptyMouseButton;
        
        private void Wpf_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is not FrameworkElement target) return;

            if (_targetButton != EmptyMouseButton)
            {
                target.ReleaseMouseCapture();
                _targetButton = EmptyMouseButton;
            }
            else
            {
                _mouseDownPoint = e.GetPosition(target);
                _mouseBeforePoint = _mouseDownPoint;
                target.CaptureMouse();
                _targetButton = e.ChangedButton;
            }
        }

        private void Wpf_MouseMove(object sender, MouseEventArgs e)
        {
            if (_targetButton == EmptyMouseButton ||
                 sender is not FrameworkElement target ||
                 !target.IsMouseCaptured) return;

            var p = e.GetPosition(target);
            var e2 = new MouseGestureEventArgs(_targetButton, _mouseDownPoint, p, p - _mouseBeforePoint);
            MouseDrag?.Invoke(target, e2);
            if (e2.Handled) _mouseDownPoint = default;
            _mouseBeforePoint = p;
        }

        private void Wpf_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (_targetButton == EmptyMouseButton ||
                sender is not FrameworkElement target ||
                !target.IsMouseCaptured) return;
            
            var p = e.GetPosition(target);
            if ((_mouseDownPoint - p).LengthSquared <= ClickableRangeSquared)
                MouseClick?.Invoke(target, new MouseGestureEventArgs(_targetButton, _mouseDownPoint, p, p - _mouseBeforePoint));

            target.ReleaseMouseCapture();
            _targetButton = EmptyMouseButton;
        }

        private void Win32_MouseDown(object? sender, Win32.MouseEventArgs e)
        {
            if (sender is not Win32.Control) return;

            if (_targetButton != EmptyMouseButton)
            {
                _targetButton = EmptyMouseButton;
            }
            else
            {
                _mouseDownPoint = new Point(e.X, e.Y);
                _mouseBeforePoint = _mouseDownPoint;
                _targetButton = e.Button switch
                {
                    Win32.MouseButtons.Left => MouseButton.Left,
                    Win32.MouseButtons.Right => MouseButton.Right,
                    _ => EmptyMouseButton,
                };
            }
        }

        private void Win32_MouseMove(object? sender, Win32.MouseEventArgs e)
        {
            if (_targetButton == EmptyMouseButton ||
                sender is not Win32.Control target) return;

            var p = new Point(e.X, e.Y);
            var e2 = new MouseGestureEventArgs(_targetButton, _mouseDownPoint, p, p - _mouseBeforePoint);
            MouseDrag?.Invoke(target, e2);
            if (e2.Handled) _mouseDownPoint = default;
            _mouseBeforePoint = p;
        }

        private void Win32_MouseUp(object? sender, Win32.MouseEventArgs e)
        {
            if (_targetButton == EmptyMouseButton ||
                sender is not Win32.Control target) return;

            var p = new Point(e.X, e.Y);
            if ((_mouseDownPoint - p).LengthSquared <= ClickableRangeSquared)
                MouseClick?.Invoke(target, new MouseGestureEventArgs(_targetButton, _mouseDownPoint, p, p - _mouseBeforePoint));
            _targetButton = EmptyMouseButton;
        }

        public event MouseGestureEventHandler? MouseClick;

        public event MouseGestureEventHandler? MouseDrag;
    }

    public class MouseGestureEventArgs : EventArgs
    {
        private readonly Point _mouseDownPosition;
        public MouseButton Button { get; }
        public Point Position { get; }
        public Vector Delta { get; }
        public bool Handled { get; set; }
        
        public MouseGestureEventArgs(MouseButton button, Point mouseDownPosition, Point position, Vector delta) =>
            (Button, _mouseDownPosition, Position, Delta) = (button, mouseDownPosition, position, delta);

        public bool IsInClickableRange() => (_mouseDownPosition - Position).LengthSquared <= MouseGestureHandler.ClickableRangeSquared;
    }

    public delegate void MouseGestureEventHandler(object sender, MouseGestureEventArgs e);
}
