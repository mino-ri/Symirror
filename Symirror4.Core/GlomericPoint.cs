using Symirror4.Core.Numerics;
using System;
using System.Numerics;

namespace Symirror4.Core;

public struct GlomericPoint : IEquatable<GlomericPoint>
{
    public const double DefaultError = 1.0 / 8192.0;

    public double X;
    public double Y;
    public double Z;
    public double W;

    public readonly double LengthSquared => X * X + Y * Y + Z * Z + W * W;

    public readonly double Length => Math.Sqrt(LengthSquared);

    public readonly double Azimuthal => Math.Acos(Z);

    public readonly double Polar => Math.Atan2(Y, X);

    public readonly bool IsValid => 1.0 - DefaultError <= LengthSquared &&
                                    LengthSquared <= 1.0 + DefaultError;

    public GlomericPoint(double x, double y, double z, double w) => (X, Y, Z, W) = (x, y, z, w);

    public static GlomericPoint Normalize(double x, double y, double z, double w)
    {
        var length = x * x + y * y + z * z + w * w;
        if (1.0 - DefaultError <= length && length <= 1.0 + DefaultError)
            return new GlomericPoint(x, y, z, w);
        length = Math.Sqrt(length);
        return new GlomericPoint(x / length, y / length, z / length, w / length);
    }

    public static GlomericPoint Normalize(in Vector4 vector) => Normalize(vector.X, vector.Y, vector.Z, vector.W);

    public static GlomericPoint Normalize(in Vector4D vector) => Normalize(vector.X, vector.Y, vector.Z, vector.W);

    // public static SphericalPoint FromSpherical(double azimuthal, double polar)
    // {
    //     var sin = Math.Sin(azimuthal);
    //     return new SphericalPoint(
    //         Math.Cos(polar) * sin,
    //         Math.Sin(polar) * sin,
    //         Math.Cos(azimuthal));
    // }

    public readonly void Validate()
    {
        if (!IsValid)
            throw new InvalidOperationException($"{this} is invalid spherical point.");
    }

    public readonly bool Equals(GlomericPoint other) => (X, Y, Z, W) == (other.X, other.Y, other.Z, other.W);

    public readonly void Deconstruct(out double x, out double y, out double z, out double w) => (x, y, z, w) = (X, Y, Z, W);

    public readonly override bool Equals(object? obj) => obj is GlomericPoint s && Equals(s);

    public readonly override int GetHashCode() => (X, Y, Z, W).GetHashCode();

    public readonly override string ToString() => $"({X:f4}, {Y:f4}, {Z:f4}, {W:f4})";

    /// <summary>玲胞上の点targetを、点A・B・Cおよび原点を通る3次元空間で裏返します。</summary>
    /// <param name="target">裏返す対象の点。</param>
    /// <param name="a">裏返しの基準となる点。</param>
    /// <param name="b">裏返しの基準となる点。</param>
    /// <param name="c">裏返しの基準となる点。</param>
    /// <returns>点targetを、3次元空間OABCで裏返した点。</returns>
    public readonly GlomericPoint Reverse(in Vector4D a, in Vector4D b, in Vector4D c)
    {
        var (x, y, z, w) = Vector4D.Cross(a, b, c);

        var dot = X * x + Y * y + Z * z + W * w;
        var normal = x * x + y * y + z * z + w * w;
        var factor = 2d * dot / normal;

        return new GlomericPoint(X - factor * x, Y - factor * y, Z - factor * z, W - factor * w);
    }

    public static double Dot(in GlomericPoint a, in GlomericPoint b) => a.X * b.X + a.Y * b.Y + a.Z * b.Z + a.W * b.W;

    public static GlomericPoint Cross(in GlomericPoint a, in GlomericPoint b, in GlomericPoint c)
    {
        var p = b.Z * c.W - b.W * c.Z;
        var q = b.Y * c.W - b.W * c.Y;
        var r = b.Y * c.Z - b.Z * c.Y;
        var s = b.X * c.W - b.W * c.X;
        var t = b.X * c.Z - b.Z * c.X;
        var u = b.X * c.Y - b.Y * c.X;

        return Normalize(
         a.Y * p - a.Z * q + a.W * r,
        -a.X * p + a.Z * s - a.W * t,
         a.X * q - a.Y * s + a.W * u,
        -a.X * r + a.Y * t - a.Z * u);
    }

    public static double Distance(in GlomericPoint a, in GlomericPoint b) => Math.Acos(Dot(in a, in b));

    public static bool ApproximatelyEqual(in GlomericPoint a, in GlomericPoint b, double error = DefaultError) =>
        Math.Abs(a.X - b.X) < error &&
        Math.Abs(a.Y - b.Y) < error &&
        Math.Abs(a.Z - b.Z) < error &&
        Math.Abs(a.W - b.W) < error;

    public static GlomericPoint Lerp(in GlomericPoint a, in GlomericPoint b, double amount)
    {
        if (amount <= 0.0) return a;
        if (amount >= 1.0) return b;

        var omega = Distance(a, b);
        var invSinOmega = 1.0 / Math.Sin(omega);
        var s1 = Math.Sin((1 - amount) * omega) * invSinOmega;
        var s2 = Math.Sin(amount * omega) * invSinOmega;
        return (GlomericPoint)(a * s1 + b * s2);
    }

    internal static double OppositeSin(GlomericPoint a, GlomericPoint b)
    {
        var cos = Dot(a, b);
        return Math.Sqrt(1.0 - cos * cos);
    }

    /// <summary>原点とcenterを通る直線が、法線ベクトルnormalを持ち点sを含む平面と交差する位置の原点からの距離を求めます。</summary>
    /// <param name="center">直線を表す単位ベクトル。</param>
    /// <param name="normal">平面の法線ベクトル。</param>
    /// <param name="s">平面に含まれる1点。</param>
    public static double GetCrossPoint(GlomericPoint center, GlomericPoint normal, GlomericPoint s)
    {
        return Dot(normal, s) / Dot(normal, center);
    }

    /// <summary>シュワルツ三角形から構成された多面体のひとつの面の中心と頂点座標から、双対多面体の指定された面に対応する頂点の原点からの距離します。</summary>
    /// <param name="center">面の中心またはその延長上にある点。</param>
    /// <param name="vertex">頂点の座標。原点からの距離が1である必要があります。</param>
    public static double CatalanPoint(in GlomericPoint center, in GlomericPoint vertex) => 1.0 / Dot(center, vertex);

    public static GlomericPoint operator +(in GlomericPoint a) => a;

    public static GlomericPoint operator -(in GlomericPoint a) => new(-a.X, -a.Y, -a.Z, -a.W);

    public static Vector4D operator +(in GlomericPoint a, in GlomericPoint b) => new(a.X + b.X, a.Y + b.Y, a.Z + b.Z, a.W + b.W);

    public static Vector4D operator -(in GlomericPoint a, in GlomericPoint b) => new(a.X - b.X, a.Y - b.Y, a.Z - b.Z, a.W - b.W);

    public static Vector4D operator *(double s, in GlomericPoint a) => new(a.X * s, a.Y * s, a.Z * s, a.W * s);

    public static Vector4D operator *(in GlomericPoint a, double s) => new(a.X * s, a.Y * s, a.Z * s, a.W * s);

    public static Vector4D operator /(in GlomericPoint a, double s) => new(a.X / s, a.Y / s, a.Z / s, a.W / s);

    public static bool operator ==(in GlomericPoint a, in GlomericPoint b) => a.Equals(b);

    public static bool operator !=(in GlomericPoint a, in GlomericPoint b) => !a.Equals(b);

    public static explicit operator Vector4(in GlomericPoint p) => new((float)p.X, (float)p.Y, (float)p.Z, (float)p.W);

    public static explicit operator GlomericPoint(in Vector4 v) => new(v.X, v.Y, v.Z, v.W);

    public static implicit operator Vector4D(in GlomericPoint p) => new((float)p.X, (float)p.Y, (float)p.Z, (float)p.W);

    public static explicit operator GlomericPoint(in Vector4D v) => new(v.X, v.Y, v.Z, v.W);
}
