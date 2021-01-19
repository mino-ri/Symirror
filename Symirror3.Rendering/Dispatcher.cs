using System;
using System.Numerics;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using Symirror3.Core;
using Symirror3.Core.Polyhedrons;
using Symirror3.Core.Symmetry;
using IndirectX;

namespace Symirror3.Rendering
{
    public sealed class Dispatcher : IDisposable
    {
        private const float RotationDelta = MathF.PI / 720f;
        private const float MoveDelta = MathF.PI / 1800f;

        private readonly Graphics _graphics;
        private readonly LinkedList<IFrameTask> _frameTasks = new();
        private readonly ConcurrentQueue<IMessage> _messageQueue = new();
        private readonly object _lockObj = new object();
        private int _bufferFrames;
        private bool _stopRequested;
        private Task? _renderLoop;
        private int _rotateCount;

        private readonly PolyhedronSelector<Vector3> _polyhedronSelector;
        private PolyhedronBase<Vector3> _polyhedron;
        private PolygonRenderer _renderer;
        private readonly PolygonFilter _filter;

        public SymmetryGroup Symmetry => _polyhedronSelector.Symmetry;

        public Dispatcher(IntPtr surfaceHandle, int width, int height, SymmetrySymbol symbol)
        {
            _graphics = new Graphics(surfaceHandle, width, height);
            _polyhedronSelector = new PolyhedronSelector<Vector3>(Vector3Operator.Instance, symbol);
            _polyhedron = _polyhedronSelector.GetPolyhedron(PolyhedronType.Normal);
            _renderer = new StandardPolygonRenderer();
            _filter = new PolygonFilter();
            _renderer.OnActivate(_graphics);
        }

        public void SendMessage(IMessage message)
        {
            _messageQueue.Enqueue(message);
            lock (_lockObj)
            {
                if (_renderLoop is null)
                    StartRenderLoop();
            }
        }

        public void Stop()
        {
            if (_renderLoop is { } loop)
            {
                _bufferFrames = 0;
                _stopRequested = true;
                loop.Wait();
            }
        }

        private void StartRenderLoop()
        {
            _bufferFrames = 60;
            _renderLoop = Task.Run(RenderLoop);
        }

        private void RenderLoop()
        {
            while (!_stopRequested && _bufferFrames > 0)
            {
                while (_messageQueue.TryDequeue(out var message))
                {
                    HandleMessage(message);
                    _bufferFrames = 60;
                }

                var currentTask = _frameTasks.First;
                if (currentTask is { })
                {
                    while (currentTask is { })
                    {
                        var current = currentTask;
                        currentTask = current.Next;
                        if (current.Value.Invoke())
                            _frameTasks.Remove(current);
                    }

                    _bufferFrames = 60;
                }
                else
                    _bufferFrames--;

                if (_bufferFrames >= 59)
                {
                    _graphics.Clear();
                    _renderer.Render(_filter.Filter(_polyhedron), _graphics);
                }

                _graphics.Present();
            }

            lock (_lockObj)
            {
                _renderLoop = null;
            }
        }

        private void HandleMessage(IMessage message)
        {
            HandleMessageCore((dynamic)message);
        }

        #region HandleMessageCore

        private void HandleMessageCore(ChangeLight msg)
        {
            _renderer.LightFactor = msg.Light;
            _renderer.ShadowFactor = msg.Shadow;
        }

        private void HandleMessageCore(MoveBasePoint msg)
        {
            var p = _polyhedron.BasePoint;
            if (msg.RotateX != 0f)
            {
                var sin = MathF.Sin(msg.RotateX * MoveDelta);
                var cos = MathF.Cos(msg.RotateX * MoveDelta);
                (p.X, p.Z) = (p.X * cos - p.Z * sin, p.Z * cos + p.X * sin);
            }
            if (msg.RotateY != 0f)
            {
                var sin = MathF.Sin(msg.RotateY * -MoveDelta);
                var cos = MathF.Cos(msg.RotateY * -MoveDelta);
                (p.Y, p.Z) = (p.Y * cos - p.Z * sin, p.Z * cos + p.Y * sin);
            }
            _polyhedron.BasePoint = p;
        }

        private void HandleMessageCore(MoveBasePointTo msg)
        {
            if (HasTask<MoveBasePointFromTo>()) return;
            var distance = Math.Acos(Math.Clamp(SphericalPoint.Dot(_polyhedron.BasePoint, msg.To), -1.0, 1.0));
            if (distance <= Math.PI / 180.0)
            {
                _polyhedron.BasePoint = msg.To;
                return;
            }
            var frameCount = Math.Max(40, (int)(distance / Math.PI * 180));
            HandleMessageCore(new MoveBasePointFromTo(_polyhedron.BasePoint, msg.To, frameCount));
        }

        private void HandleMessageCore(MoveBasePointFromTo msg)
        {
            if (HasTask<MoveBasePointFromTo>()) return;
            StartCountTask(msg, msg.FrameCount, (m, count) =>
            {
                _polyhedron.BasePoint = SphericalPoint.Lerp(m.To, m.From, (double)count / m.FrameCount);
            });
        }

        private void HandleMessageCore(Rotate msg)
        {
            if (msg.RotateX != 0f)
                _graphics.World *= Matrix4.RotationY(msg.RotateX * RotationDelta);
            if (msg.RotateY != 0f)
                _graphics.World *= Matrix4.RotationX(msg.RotateY * -RotationDelta);
            if (msg.RotateZ != 0f)
                _graphics.World *= Matrix4.RotationZ(msg.RotateZ * RotationDelta);
            _graphics.FlushWorld();
        }

        private void HandleMessageCore(ChangeAutoRotation msg)
        {
            var hasTask = HasTask<ChangeAutoRotation>();
            if (msg.AutoRotation && !hasTask)
            {
                StartTask(msg, m =>
                {
                    _rotateCount = (_rotateCount + 1) & 1023;
                    _graphics.SetWorld(Matrix4.RotationX((float)Math.PI / 512f * _rotateCount) *
                                        Matrix4.RotationY((float)Math.PI / 512f * _rotateCount));
                    return false;
                });
            }
            else if (!msg.AutoRotation && hasTask)
            {
                for (var node = _frameTasks.First; node is { }; node = node.Next)
                {
                    if (node.Value.Message is ChangeAutoRotation)
                    {
                        _frameTasks.Remove(node);
                        break;
                    }
                }
            }
        }

        private void HandleMessageCore(ResetRotation _)
        {
            _graphics.SetWorld(in Matrix4.Identity);
            _rotateCount = 0;
        }

        private void HandleMessageCore(ChangeSymbol msg)
        {
            _polyhedronSelector.Symbol = msg.Symbol;
            HandleMessageCore(new ChangePolyhedronType(msg.PolyhedronType));
        }

        private void HandleMessageCore(ChangePolyhedronType msg) => _polyhedron = _polyhedronSelector.GetPolyhedron(msg.PolyhedronType);

        private void HandleMessageCore(ChangeFaceVisible msg) => _filter.HandleMessage(msg);

        private void HandleMessageCore(ChangeFaceViewType msg) => _filter.HandleMessage(msg);

        private void HandleMessageCore(ChangeFaceRenderType msg)
        {
            var (light, shadow) = (_renderer.LightFactor, _renderer.ShadowFactor);
            _renderer = msg.FaceRenderType switch
            {
                FaceRenderType.Holed => new HoledPolygonRenderer(),
                _ => new StandardPolygonRenderer(),
            };
            _renderer.LightFactor = light;
            _renderer.ShadowFactor = shadow;
            _renderer.OnActivate(_graphics);
        }

        #pragma warning disable CA1822 // メンバーを static に設定します
        private void HandleMessageCore(IMessage message)
        #pragma warning restore CA1822
        {
            System.Diagnostics.Debug.Print($"Unhandled message: {message?.GetType().Name ?? "[null]"}");
        }
        #endregion

        private bool HasTask<T>() where T : IMessage => _frameTasks.Any(t => t.Message is T);

        private void StartTask<T>(T message, Func<T, bool> action) where T : IMessage =>
            _frameTasks.AddLast(new FrameTask<T>(message, action));

        private void StartCountTask<T>(T message, int count, Action<T, int> action) where T : IMessage =>
            _frameTasks.AddLast(new CountFrameTask<T>(message, count, action));

        public void Dispose()
        {
            Stop();
            _graphics.Dispose();
        }
    }
}
