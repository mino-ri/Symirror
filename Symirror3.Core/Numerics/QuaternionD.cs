using System;
using static System.Math;

namespace Symirror3.Core.Numerics;

public struct QuaternionD : IEquatable<QuaternionD>
{
    public double X;
    public double Y;
    public double Z;
    public double W;

    public QuaternionD(double x, double y, double z, double w) => (X, Y, Z, W) = (x, y, z, w);

    public QuaternionD(in Vector3D vector, double w) => (X, Y, Z, W) = (vector.X, vector.Y, vector.Z, w);

    public readonly void Deconstruct(out double x, out double y, out double z, out double w) => (x, y, z, w) = (X, Y, Z, W);

    public static readonly QuaternionD Identity = new(0.0, 0.0, 0.0, 1.0);

    public readonly double LengthSquared => X * X + Y * Y + Z * Z + W * W;

    public readonly double Length => Sqrt(LengthSquared);

    public readonly QuaternionD Normalize()
    {
        var invNorm = 1.0 / Length;
        return new QuaternionD(X * invNorm, Y * invNorm, Z * invNorm, W * invNorm);
    }

    public readonly QuaternionD Conjugate => new(-X, -Y, -Z, W);

    public readonly QuaternionD Inverse()
    {
        var invNorm = 1.0f / LengthSquared;
        return new QuaternionD(-X * invNorm, -Y * invNorm, -Z * invNorm, W * invNorm);
    }

    public static QuaternionD FromAxisAngle(Vector3D axis, double angle)
    {
        var halfAngle = angle * 0.5;
        var sin = Sin(halfAngle);
        var cos = Cos(halfAngle);
        return new QuaternionD(axis.X * sin, axis.Y * sin, axis.Z * sin, cos);
    }

    public static QuaternionD RotationX(double angle)
    {
        var halfAngle = angle * 0.5;
        return new QuaternionD(Sin(halfAngle), 0.0, 0.0, Cos(halfAngle));
    }

    public static QuaternionD RotationY(double angle)
    {
        var halfAngle = angle * 0.5;
        return new QuaternionD(0.0, Sin(halfAngle), 0.0, Cos(halfAngle));
    }

    public static QuaternionD RotationZ(double angle)
    {
        var halfAngle = angle * 0.5;
        return new QuaternionD(0.0, 0.0, Sin(halfAngle), Cos(halfAngle));
    }

    public static double Dot(in QuaternionD a, in QuaternionD b) =>
        a.X * b.X + a.Y * b.Y + a.Z * b.Z + a.W * b.W;

    public readonly bool Equals(QuaternionD other) => X == other.X && Y == other.Y && Z == other.Z && W == other.W;

    public override readonly bool Equals(object? obj) => obj is QuaternionD other && Equals(other);

    public override readonly int GetHashCode() => (X, Y, Z, W).GetHashCode();

    public override readonly string ToString() => $"[{X}, {Y}, {Z}, {W}]";

    public readonly Vector3D ToBasePoint()
    {
        return new Vector3D(
            2.0 * (X * Z + Y * W),
            2.0 * (Y * Z - X * W),
            Z * Z + W * W - X * X - Y * Y);
    }

    public readonly Vector3F ToBasePointF()
    {
        return new Vector3F(
            (float)(2.0 * (X * Z + Y * W)),
            (float)(2.0 * (Y * Z - X * W)),
            (float)(Z * Z + W * W - X * X - Y * Y));
    }

    public static Vector3D operator *(in QuaternionD a, in Vector3D v)
    {
        var sx = a.W * v.X + a.Y * v.Z - a.Z * v.Y;
        var sy = a.W * v.Y + a.Z * v.X - a.X * v.Z;
        var sz = a.W * v.Z + a.X * v.Y - a.Y * v.X;
        var sw = a.X * v.X + a.Y * v.Y + a.Z * v.Z;

        return new Vector3D(
            sx * a.W + sw * a.X + sz * a.Y - sy * a.Z,
            sy * a.W + sw * a.Y + sx * a.Z - sz * a.X,
            sz * a.W + sw * a.Z + sy * a.X - sx * a.Y);
    }

    public static SphericalPoint operator *(in QuaternionD a, in SphericalPoint v)
    {
        var sx = a.W * v.X + a.Y * v.Z - a.Z * v.Y;
        var sy = a.W * v.Y + a.Z * v.X - a.X * v.Z;
        var sz = a.W * v.Z + a.X * v.Y - a.Y * v.X;
        var sw = a.X * v.X + a.Y * v.Y + a.Z * v.Z;

        return new SphericalPoint(
            sx * a.W + sw * a.X + sz * a.Y - sy * a.Z,
            sy * a.W + sw * a.Y + sx * a.Z - sz * a.X,
            sz * a.W + sw * a.Z + sy * a.X - sx * a.Y);
    }

    public static Vector3F operator *(in QuaternionD a, Vector3F v)
    {
        var sx = a.W * v.X + a.Y * v.Z - a.Z * v.Y;
        var sy = a.W * v.Y + a.Z * v.X - a.X * v.Z;
        var sz = a.W * v.Z + a.X * v.Y - a.Y * v.X;
        var sw = a.X * v.X + a.Y * v.Y + a.Z * v.Z;

        return new Vector3F(
            (float)(sx * a.W + sw * a.X + sz * a.Y - sy * a.Z),
            (float)(sy * a.W + sw * a.Y + sx * a.Z - sz * a.X),
            (float)(sz * a.W + sw * a.Z + sy * a.X - sx * a.Y));
    }

    public static bool operator ==(in QuaternionD a, in QuaternionD b) => a.Equals(b);

    public static bool operator !=(in QuaternionD a, in QuaternionD b) => !a.Equals(b);

    public static QuaternionD operator +(in QuaternionD a) => a;

    public static QuaternionD operator -(in QuaternionD a) => new(-a.X, -a.Y, -a.Z, -a.W);

    public static QuaternionD operator +(in QuaternionD a, in QuaternionD b) =>
        new(a.X + b.X, a.Y + b.Y, a.Z + b.Z, a.W + b.W);

    public static QuaternionD operator -(in QuaternionD a, in QuaternionD b) =>
        new(a.X - b.X, a.Y - b.Y, a.Z - b.Z, a.W - b.W);

    public static QuaternionD operator *(in QuaternionD a, in QuaternionD b)
    {
        return new QuaternionD(
            a.X * b.W + b.X * a.W + a.Y * b.Z - a.Z * b.Y,
            a.Y * b.W + b.Y * a.W + a.Z * b.X - a.X * b.Z,
            a.Z * b.W + b.Z * a.W + a.X * b.Y - a.Y * b.X,
            a.W * b.W - a.X * b.X - a.Y * b.Y - a.Z * b.Z);
    }

    public static QuaternionD operator *(in QuaternionD a, double f) =>
        new(a.X * f, a.Y * f, a.Z * f, a.W * f);

    public static QuaternionD operator /(in QuaternionD a, in QuaternionD b) => a * b.Inverse();

    public static QuaternionD operator /(in QuaternionD a, double f) =>
        new(a.X / f, a.Y / f, a.Z / f, a.W / f);
}
