using Symirror3.Core.Numerics;
using System;
using System.Numerics;

namespace Symirror3.Core;

public struct SphericalPoint : IEquatable<SphericalPoint>
{
    public const double DefaultError = 1.0 / 8192.0;

    public double X;
    public double Y;
    public double Z;

    public readonly double LengthSquared => X * X + Y * Y + Z * Z;

    public readonly double Length => Math.Sqrt(LengthSquared);

    public readonly double Azimuthal => Math.Acos(Z);

    public readonly double Polar => Math.Atan2(Y, X);

    public readonly bool IsValid => 1.0 - DefaultError <= LengthSquared &&
                                    LengthSquared <= 1.0 + DefaultError;

    public SphericalPoint(double x, double y, double z) => (X, Y, Z) = (x, y, z);

    public static SphericalPoint Normalize(double x, double y, double z)
    {
        var length = x * x + y * y + z * z;
        if (1.0 - DefaultError <= length && length <= 1.0 + DefaultError)
            return new SphericalPoint(x, y, z);
        length = Math.Sqrt(length);
        return new SphericalPoint(x / length, y / length, z / length);
    }

    public static SphericalPoint Normalize(in Vector3D vector) => Normalize(vector.X, vector.Y, vector.Z);

    public static SphericalPoint FromSpherical(double azimuthal, double polar)
    {
        var sin = Math.Sin(azimuthal);
        return new SphericalPoint(
            Math.Cos(polar) * sin,
            Math.Sin(polar) * sin,
            Math.Cos(azimuthal));
    }

    public readonly void Validate()
    {
        if (!IsValid)
            throw new InvalidOperationException($"{this} is invalid spherical point.");
    }

    public readonly bool Equals(SphericalPoint other) => (X, Y, Z) == (other.X, other.Y, other.Z);

    public readonly void Deconstruct(out double x, out double y, out double z) => (x, y, z) = (X, Y, Z);

    public readonly override bool Equals(object? obj) => obj is SphericalPoint s && Equals(s);

    public readonly override int GetHashCode() => (X, Y, Z).GetHashCode();

    public readonly override string ToString() => $"({X:f4}, {Y:f4}, {Z:f4})";

    public static double Dot(in SphericalPoint a, in SphericalPoint b) => a.X * b.X + a.Y * b.Y + a.Z * b.Z;

    public static SphericalPoint Cross(in SphericalPoint a, in SphericalPoint b) =>
        Normalize(a.Y * b.Z - a.Z * b.Y, a.Z * b.X - a.X * b.Z, a.X * b.Y - a.Y * b.X);

    public static double Distance(in SphericalPoint a, in SphericalPoint b) => Math.Acos(Dot(in a, in b));

    public static bool ApproximatelyEqual(in SphericalPoint a, in SphericalPoint b, double error = DefaultError) =>
        Math.Abs(a.X - b.X) < error &&
        Math.Abs(a.Y - b.Y) < error &&
        Math.Abs(a.Z - b.Z) < error;

    public static SphericalPoint Lerp(in SphericalPoint a, in SphericalPoint b, double amount)
    {
        if (amount <= 0.0) return a;
        if (amount >= 1.0) return b;

        var omega = Distance(a, b);
        var invSinOmega = 1.0 / Math.Sin(omega);
        var s1 = Math.Sin((1 - amount) * omega) * invSinOmega;
        var s2 = Math.Sin(amount * omega) * invSinOmega;
        return (SphericalPoint)(a * s1 + b * s2);
    }

    internal static double OppositeSin(SphericalPoint a, SphericalPoint b)
    {
        var cos = Dot(a, b);
        return Math.Sqrt(1.0 - cos * cos);
    }

    /// <summary>原点とcenterを通る直線が、法線ベクトルnormalを持ち点sを含む平面と交差する位置の原点からの距離を求めます。</summary>
    /// <param name="center">直線を表す単位ベクトル。</param>
    /// <param name="normal">平面の法線ベクトル。</param>
    /// <param name="s">平面に含まれる1点。</param>
    public static double GetCrossPoint(SphericalPoint center, SphericalPoint normal, SphericalPoint s)
    {
        return Dot(normal, s) / Dot(normal, center);
    }

    /// <summary>シュワルツ三角形から構成された多面体のひとつの面の中心と頂点座標から、双対多面体の指定された面に対応する頂点の原点からの距離します。</summary>
    /// <param name="center">面の中心またはその延長上にある点。</param>
    /// <param name="vertex">頂点の座標。原点からの距離が1である必要があります。</param>
    public static double CatalanPoint(in SphericalPoint center, in SphericalPoint vertex) => 1.0 / Dot(center, vertex);

    public static SphericalPoint operator +(in SphericalPoint a) => a;

    public static SphericalPoint operator -(in SphericalPoint a) => new(-a.X, -a.Y, -a.Z);

    public static Vector3D operator +(in SphericalPoint a, in SphericalPoint b) => new(a.X + b.X, a.Y + b.Y, a.Z + b.Z);

    public static Vector3D operator -(in SphericalPoint a, in SphericalPoint b) => new(a.X - b.X, a.Y - b.Y, a.Z - b.Z);

    public static Vector3D operator *(double s, in SphericalPoint a) => new(a.X * s, a.Y * s, a.Z * s);

    public static Vector3D operator *(in SphericalPoint a, double s) => new(a.X * s, a.Y * s, a.Z * s);

    public static Vector3D operator /(in SphericalPoint a, double s) => new(a.X / s, a.Y / s, a.Z / s);

    public static bool operator ==(in SphericalPoint a, in SphericalPoint b) => a.Equals(b);

    public static bool operator !=(in SphericalPoint a, in SphericalPoint b) => !a.Equals(b);

    public static implicit operator Vector3D(in SphericalPoint p) => new(p.X, p.Y, p.Z);

    public static explicit operator SphericalPoint(in Vector3D v) => new(v.X, v.Y, v.Z);

    public static explicit operator SphericalPoint(Vector3F v) => new(v.X, v.Y, v.Z);

    public static explicit operator Vector3F(in SphericalPoint p) => new((float)p.X, (float)p.Y, (float)p.Z);

    public static explicit operator SphericalPoint(Vector3 v) => new(v.X, v.Y, v.Z);

    public static explicit operator Vector3(in SphericalPoint p) => new((float)p.X, (float)p.Y, (float)p.Z);
}
