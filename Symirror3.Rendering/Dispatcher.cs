using System;
using System.Numerics;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using Symirror3.Core.Polyhedrons;
using Symirror3.Core.Symmetry;
using Sphere = Symirror3.Core.Sphere;
using IndirectX;

namespace Symirror3.Rendering
{
    public sealed class Dispatcher : IDisposable
    {
        private const float RotationDelta = MathF.PI / 720f;
        private const float MoveDelta = MathF.PI / 1800f;

        private readonly Graphics _graphics;
        private readonly LinkedList<IFrameTask> _frameTasks = new();
        private readonly ConcurrentQueue<IMessage?> _messageQueue = new();
        private int _bufferFrames;
        private bool _stopRequested;
        private Task? _renderLoop;
        private int _rotateCount;
        private readonly object _lockObj = new object();

        private PolyhedronSelector<Vector3> _polyhedronSelector;
        private PolyhedronBase<Vector3> _polyhedron;
        private PolygonRenderer _renderer;
        private PolygonFilter _filter;

        public Symmetry<Vector3> Symmetry => _polyhedronSelector.Symmetry;

        public Dispatcher(IntPtr surfaceHandle, int width, int height, SymmetrySymbol symbol)
        {
            _graphics = new Graphics(surfaceHandle, width, height);
            _polyhedronSelector = new PolyhedronSelector<Vector3>(Vector3Operator.Instance, symbol);
            _polyhedron = _polyhedronSelector.GetPolyhedron(PolyhedronType.Normal);
            _renderer = new StandardPolygonRenderer();
            _filter = new PolygonFilter();
            _renderer.OnActivate(_graphics);
        }

        public void SendMessage(IMessage? message)
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

        private void HandleMessage(IMessage? message)
        {
            switch (message)
            {
                case ChangeLight msg:
                    _renderer.LightFactor = msg.Light;
                    _renderer.ShadowFactor = msg.Shadow;
                    break;
                case MoveBasePoint msg:
                    HandleMoveBasePoint(msg);
                    break;
                case MoveBasePointTo msg:
                    HandleMoveBasePointTo(msg);
                    break;
                case Rotate msg:
                    HandleRotate(msg);
                    break;
                case ChangeAutoRotation msg:
                    HandleAutoRotation(msg);
                    break;
                case ResetRotation msg:
                    _graphics.SetWorld(in Matrix4.Identity);
                    _rotateCount = 0;
                    break;
                case ChangeSymbol msg:
                    _polyhedronSelector.Symbol = msg.Symbol;
                    _polyhedron = _polyhedronSelector.GetPolyhedron(msg.PolyhedronType);
                    break;
                case ChangePolyhedronType msg:
                    _polyhedron = _polyhedronSelector.GetPolyhedron(msg.PolyhedronType);
                    break;
                case ChangeFaceVisible msg:
                    _filter.HandleMessage(msg);
                    break;
                case ChangeFaceViewType msg:
                    _filter.HandleMessage(msg);
                    break;
                case ChangeFaceRenderType msg:
                    var (light, shadow) = (_renderer.LightFactor, _renderer.ShadowFactor);
                    _renderer = msg.FaceRenderType switch
                    {
                        FaceRenderType.Holed => new HoledPolygonRenderer(),
                        _ => new StandardPolygonRenderer(),
                    }; 
                    _renderer.LightFactor = light;
                    _renderer.ShadowFactor = shadow;
                    _renderer.OnActivate(_graphics);
                    break;
                default:
                    StartTask(message!, _ => true);
                    break;
            }

            void HandleMoveBasePoint(MoveBasePoint msg)
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

            void HandleMoveBasePointTo(MoveBasePointTo msg)
            {
                if (HasTask<MoveBasePointFromTo>()) return;
                var distance = MathF.Acos(Math.Clamp(Vector3.Dot(_polyhedron.BasePoint, msg.To), -1f, 1f));
                var baseCount = Math.Max(40, (int)(distance / Math.PI * 180));
                StartCountTask(new MoveBasePointFromTo(_polyhedron.BasePoint, msg.To, baseCount), baseCount, (m, count) =>
                {
                    _polyhedron.BasePoint = Sphere.Lerp(Vector3Operator.Instance, m.To, m.From, (double)count / m.FrameCount);
                });
            }

            void HandleRotate(Rotate msg)
            {
                if (msg.RotateX != 0f)
                    _graphics.World *= Matrix4.RotationY(msg.RotateX * RotationDelta);
                if (msg.RotateY != 0f)
                    _graphics.World *= Matrix4.RotationX(msg.RotateY * -RotationDelta);
                if (msg.RotateZ != 0f)
                    _graphics.World *= Matrix4.RotationZ(msg.RotateZ * RotationDelta);
                _graphics.FlushWorld();
            }

            void HandleAutoRotation(ChangeAutoRotation msg)
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
        }

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
