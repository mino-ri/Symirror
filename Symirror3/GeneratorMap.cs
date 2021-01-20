using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Symirror3.Core;
using Symirror3.Core.Numerics;
using Symirror3.Core.Symmetry;

namespace Symirror3
{
    public class GeneratorMap
    {
        private SymmetryTriangle _triangle;
        private QuaternionD _modelToView;
        private QuaternionD _viewToModel;
        private double _scale;
        private readonly WriteableBitmap _bitmap;
        private readonly int _size;
        private readonly int _halfSize;

        public ImageSource ImageSource => _bitmap;

        public GeneratorMap(int size, SymmetryTriangle triangle)
        {
            _triangle = null!;
            _size = size;
            _halfSize = _size / 2;
            _bitmap = new WriteableBitmap(_size, _size, 96.0, 96.0, PixelFormats.Pbgra32, null);
            UpdateImage(triangle);
        }

        public void UpdateImage(SymmetryTriangle triangle)
        {
            _triangle = triangle;
            var normal = SphericalPoint.Normalize(Vector3D.Cross(
                _triangle[1].Point - _triangle[0].Point, _triangle[2].Point - _triangle[0].Point));
            var origin = new SphericalPoint(0, 0, -1);
            _viewToModel = QuaternionD.FromAxisAngle(SphericalPoint.Cross(origin, normal), SphericalPoint.Distance(normal, origin));
            var viewVertex0 = _viewToModel.Inverse() * _triangle[2].Point;
            _viewToModel = _viewToModel * QuaternionD.RotationZ(Math.Atan2(viewVertex0.X, -viewVertex0.Y));
            _modelToView = _viewToModel.Inverse();

            try
            {
                _bitmap.Lock();
                WriteBitmapCore(_bitmap.BackBuffer);
                _bitmap.AddDirtyRect(new Int32Rect(0, 0, _bitmap.PixelWidth, _bitmap.PixelHeight));
            }
            finally
            {
                _bitmap.Unlock();
            }
        }

        private unsafe void WriteBitmapCore(IntPtr buffer)
        {
            var edges = new[]
            {
                _triangle.Edge(0),
                _triangle.Edge(1),
                _triangle.Edge(2),
            };
            var incentor = _triangle.GetIncenter();
            var bisectors = new[]
            {
                new SphericalRing(_triangle[0].Point, in incentor),
                new SphericalRing(_triangle[1].Point, in incentor),
                new SphericalRing(_triangle[2].Point, in incentor),
            };

            var stride = _bitmap.BackBufferStride / 4;
            Span<uint> p = new Span<uint>(buffer.ToPointer(), _bitmap.PixelHeight * stride);
            for (var y = 0; y < _size; y++)
                for (var x = 0; x < _size; x++)
                {
                    var sp = new Point(x / (double)_halfSize - 1.0, y / (double)_halfSize - 1.0);
                    if (((Vector)sp).LengthSquared >= 1.0)
                    {
                        p[x + y * stride] = 0xFFFFFFFF;
                    }
                    else
                    {
                        var mp = ViewToModel(sp);
                        var color = 0xFF000000;
                        if (SphericalPoint.Dot(edges[0].Normal, mp) >= 0.0)
                            color += 0x00222222;
                        if (SphericalPoint.Dot(edges[1].Normal, mp) >= 0.0)
                            color += 0x00222222;
                        if (SphericalPoint.Dot(edges[2].Normal, mp) >= 0.0)
                            color += 0x00222222;
                        if (color == 0xFF666666)
                        {
                            color = 0xFF000000;
                            if (SphericalPoint.Dot(bisectors[0].Normal, mp) >= 0.0)
                                color |= 0x000000EE;
                            if (SphericalPoint.Dot(bisectors[1].Normal, mp) >= 0.0)
                                color |= 0x0000EE00;
                            if (SphericalPoint.Dot(bisectors[2].Normal, mp) >= 0.0)
                                color |= 0x00EE0000;
                        }

                        p[x + y * stride] = color;
                    }
                }
        }

        public Point ModelToView(SphericalPoint p)
        {
            p = _modelToView * p;
            var d = 1.0 - p.Z;
            return new Point(p.X / d, p.Y / d);
        }

        public SphericalPoint ViewToModel(Point p)
        {
            var d = 1.0 + p.X * p.X + p.Y * p.Y;
            return _viewToModel * new SphericalPoint(2.0 * p.X / d, 2.0 * p.Y / d, (d - 2.0) / d);
        }
    }
}
