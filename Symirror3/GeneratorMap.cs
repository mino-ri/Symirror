using Symirror3.Core;
using Symirror3.Core.Numerics;
using Symirror3.Core.Polyhedrons;
using Symirror3.Core.Symmetry;
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Symirror3;

public class GeneratorMap
{
    private const double Sqrt2 = 1.4142135623730951;

    private SymmetryTriangle _triangle;
    private QuaternionD _modelToView;
    private QuaternionD _viewToModel;
    private double _scale;
    private PolyhedronType _polyhedronType;
    private readonly WriteableBitmap _bitmap;
    private readonly int _screenSize;
    private readonly int _halfSize;
    private readonly double _dpiHalfSize;

    public ImageSource ImageSource => _bitmap;

    public GeneratorMap(double size, double dpiScale, SymmetryTriangle triangle, PolyhedronType polyhedronType)
    {
        _triangle = null!;
        _dpiHalfSize = size / 2;
        _screenSize = (int)Math.Round(size * dpiScale);
        _halfSize = _screenSize / 2;
        _bitmap = new WriteableBitmap(_screenSize, _screenSize, 96.0, 96.0, PixelFormats.Pbgra32, null);
        UpdateImage(triangle, polyhedronType);
    }

    public void UpdateImage(SymmetryTriangle triangle, PolyhedronType polyhedronType)
    {
        _triangle = triangle;
        _polyhedronType = polyhedronType;
        var normal = SphericalPoint.Normalize(Vector3D.Cross(
            _triangle[1].Point - _triangle[0].Point, _triangle[2].Point - _triangle[0].Point));
        var origin = new SphericalPoint(0, 0, -1);
        _viewToModel = QuaternionD.FromAxisAngle(SphericalPoint.Cross(origin, normal), SphericalPoint.Distance(normal, origin));
        var viewVertex2 = _viewToModel.Inverse() * _triangle[2].Point;
        _viewToModel *= QuaternionD.RotationZ(Math.Atan2(viewVertex2.X, -viewVertex2.Y));
        _modelToView = _viewToModel.Inverse();
        _scale = 1.0;
        _scale = ((Vector)ModelToView(_triangle[0].Point)).Length * 1.125;

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

        var stride = _bitmap.BackBufferStride / 4;
        var p = new Span<uint>(buffer.ToPointer(), _bitmap.PixelHeight * stride);
        if (_polyhedronType is PolyhedronType.Snub or PolyhedronType.SnubDual)
        {
            SphericalPoint mp, reversed0, reversed1, reversed2;
            double distance0, distance1, distance2;
            uint color, addingFactor;
            for (var y = 0; y < _screenSize; y++)
                for (var x = 0; x < _screenSize; x++)
                {
                    var sp = new Point(x / (double)_halfSize - 1.0, y / (double)_halfSize - 1.0);
                    if (((Vector)sp).LengthSquared >= 1.0)
                    {
                        p[x + y * stride] = 0xFFFFFFFF;
                    }
                    else
                    {
                        mp = ViewToModel(sp);
                        reversed0 = edges[0].Reverse(in mp);
                        reversed1 = edges[1].Reverse(in mp);
                        reversed2 = edges[2].Reverse(in mp);
                        distance0 = SphericalPoint.Dot(reversed2, reversed0);
                        distance1 = SphericalPoint.Dot(reversed0, reversed1);
                        distance2 = SphericalPoint.Dot(reversed1, reversed2);
                        color = 0xFF000000;
                        addingFactor =
                            SphericalPoint.Dot(edges[0].Normal, mp) >= 0.0 &&
                            SphericalPoint.Dot(edges[1].Normal, mp) >= 0.0 &&
                            SphericalPoint.Dot(edges[2].Normal, mp) >= 0.0
                            ? 0xEEu : 0x44u;
                        if (distance0 <= distance1)
                            color |= addingFactor;
                        if (distance1 <= distance2)
                            color |= addingFactor << 8;
                        if (distance2 <= distance0)
                            color |= addingFactor << 16;
                        p[x + y * stride] = color;
                    }
                }
        }
        else if (_polyhedronType == PolyhedronType.Dirhombic)
        {
            SphericalPoint mp, reversed0, reversed1, reversed2;
            double distance0, distance1, distance2;
            uint color, addingFactor;
            for (var y = 0; y < _screenSize; y++)
                for (var x = 0; x < _screenSize; x++)
                {
                    var sp = new Point(x / (double)_halfSize - 1.0, y / (double)_halfSize - 1.0);
                    if (((Vector)sp).LengthSquared >= 1.0)
                    {
                        p[x + y * stride] = 0xFFFFFFFF;
                    }
                    else
                    {
                        mp = ViewToModel(sp);
                        reversed0 = edges[0].Reverse(in mp);
                        reversed1 = edges[1].Reverse(in mp);
                        reversed2 = edges[2].Reverse(in mp);
                        distance0 = (reversed2 - reversed0).Length;
                        distance1 = (reversed0 - reversed1).Length;
                        distance2 = (reversed1 - reversed2).Length / Sqrt2;
                        color = 0xFF000000;
                        addingFactor =
                            SphericalPoint.Dot(edges[0].Normal, mp) >= 0.0 &&
                            SphericalPoint.Dot(edges[1].Normal, mp) >= 0.0 &&
                            SphericalPoint.Dot(edges[2].Normal, mp) >= 0.0
                            ? 0xEEu : 0x44u;
                        if (distance0 <= distance1)
                            color |= addingFactor;
                        if (distance1 <= distance2)
                            color |= addingFactor << 8;
                        if (distance2 <= distance0)
                            color |= addingFactor << 16;
                        p[x + y * stride] = color;
                    }
                }
        }
        else
        {
            var incentor = _triangle.GetIncenter();
            var bisectors = new[]
            {
                    new SphericalRing(_triangle[0].Point, in incentor),
                    new SphericalRing(_triangle[1].Point, in incentor),
                    new SphericalRing(_triangle[2].Point, in incentor),
                };
            SphericalPoint mp;
            uint color, addingFactor;
            for (var y = 0; y < _screenSize; y++)
                for (var x = 0; x < _screenSize; x++)
                {
                    var sp = new Point(x / (double)_halfSize - 1.0, y / (double)_halfSize - 1.0);
                    if (((Vector)sp).LengthSquared >= 1.0)
                    {
                        p[x + y * stride] = 0xFFFFFFFF;
                    }
                    else
                    {
                        mp = ViewToModel(sp);
                        color = 0xFF000000;
                        addingFactor =
                            SphericalPoint.Dot(edges[0].Normal, mp) >= 0.0 &&
                            SphericalPoint.Dot(edges[1].Normal, mp) >= 0.0 &&
                            SphericalPoint.Dot(edges[2].Normal, mp) >= 0.0
                            ? 0xEEu : 0x44u;
                        if (SphericalPoint.Dot(bisectors[0].Normal, mp) >= 0.0)
                            color |= addingFactor;
                        if (SphericalPoint.Dot(bisectors[1].Normal, mp) >= 0.0)
                            color |= addingFactor << 8;
                        if (SphericalPoint.Dot(bisectors[2].Normal, mp) >= 0.0)
                            color |= addingFactor << 16;

                        p[x + y * stride] = color;
                    }
                }
        }
    }

    public Point ModelToView(SphericalPoint p)
    {
        p = _modelToView * p;
        var d = 1.0 - p.Z;
        return new Point(p.X / d / _scale, p.Y / d / _scale);
    }

    public Point ModelToDpi(in SphericalPoint p)
    {
        var vp = ModelToView(p);
        return new Point((vp.X + 1.0) * _dpiHalfSize, (vp.Y + 1.0) * _dpiHalfSize);
    }

    public SphericalPoint ViewToModel(Point p)
    {
        p = new Point(p.X * _scale, p.Y * _scale);
        var d = 1.0 + p.X * p.X + p.Y * p.Y;
        return _viewToModel * new SphericalPoint(2.0 * p.X / d, 2.0 * p.Y / d, (d - 2.0) / d);
    }

    public SphericalPoint ScreenToModel(Point p)
    {
        return ViewToModel(new Point(p.X / _halfSize - 1.0, p.Y / _halfSize - 1.0));
    }

    public SphericalPoint DpiToModel(Point p)
    {
        return ViewToModel(new Point(p.X / _dpiHalfSize - 1.0, p.Y / _dpiHalfSize - 1.0));
    }
}
