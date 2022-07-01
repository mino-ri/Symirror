using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace Symirror4.Core.Numerics;

/// <summary>3次元上の座標を表します。</summary>
[StructLayout(LayoutKind.Sequential)]
public readonly struct Vector4D : IEquatable<Vector4D>
{
    /// <summary>X座標。</summary>
    public readonly double X;

    /// <summary>Y座標。</summary>
    public readonly double Y;

    /// <summary>Z座標。</summary>
    public readonly double Z;

    /// <summary>Z座標。</summary>
    public readonly double W;

    public double LengthSquared => X * X + Y * Y + Z * Z + W * W;

    public double Length => Math.Sqrt(LengthSquared);

    public Vector4D Normalize()
    {
        var mag = Length;
        if (mag == 0) return this;
        return this / Length;
    }

    /// <summary><see cref="Vector4D"/>構造体の新しいインスタンスを生成します。</summary>
    /// <param name="x">X成分。</param>
    /// <param name="y">Y成分。</param>
    /// <param name="z">Z成分。</param>
    public Vector4D(double x, double y, double z, double w) => (X, Y, Z, W) = (x, y, z, w);

    /// <summary>このオブジェクトを、それと等価な文字列に変換します。</summary>
    /// <returns>現在のオブジェクトを表す<see cref="string"/>。</returns>
    public override string ToString() => $"({X}, {Y}, {Z}, {W})";

    public void Deconstruct(out double x, out double y, out double z, out double w) => (x, y, z, w) = (X, Y, Z, W);

    public bool Equals(Vector4D other) => (X, Y, Z) == (other.X, other.Y, other.Z);

    public override bool Equals(object? obj) => obj is Vector4D other && other.Equals(this);

    public override int GetHashCode() => (X, Y, Z).GetHashCode();

    /// <summary>ベクトルの内積を求めます。</summary>
    public static double Dot(in Vector4D a, in Vector4D b) => a.X * b.X + a.Y * b.Y + a.Z * b.Z;

    /// <summary>玲胞上の点targetを、点A・B・Cおよび原点を通る3次元空間で裏返します。</summary>
    /// <param name="target">裏返す対象の点。</param>
    /// <param name="a">裏返しの基準となる点。</param>
    /// <param name="b">裏返しの基準となる点。</param>
    /// <param name="c">裏返しの基準となる点。</param>
    /// <returns>点targetを、3次元空間OABCで裏返した点。</returns>
    public Vector4D Reverse(in Vector4D a, in Vector4D b, in Vector4D c)
    {
        var (x, y, z, w) = Cross(a, b, c);

        var dot = X * x + Y * y + Z * z + W * w;
        var normal = x * x + y * y + z * z + w * w;
        var factor = 2d * dot / normal;

        return new Vector4D(X - factor * x, Y - factor * y, Z - factor * z, W - factor * w);
    }

    /// <summary>
    /// 指定した4次元ベクトルから外積を演算し、法線ベクトルを求めます。
    /// </summary>
    /// <param name="origin">原点ベクトル。</param>
    /// <param name="a">ベクトル1。</param>
    /// <param name="b">ベクトル2。</param>
    /// <param name="c">ベクトル3。</param>
    public static Vector4D Cross(in Vector4D a, in Vector4D b, in Vector4D c)
    {
        var p = b.Z * c.W - b.W * c.Z;
        var q = b.Y * c.W - b.W * c.Y;
        var r = b.Y * c.Z - b.Z * c.Y;
        var s = b.X * c.W - b.W * c.X;
        var t = b.X * c.Z - b.Z * c.X;
        var u = b.X * c.Y - b.Y * c.X;

        return new(
         a.Y * p - a.Z * q + a.W * r,
        -a.X * p + a.Z * s - a.W * t,
         a.X * q - a.Y * s + a.W * u,
        -a.X * r + a.Y * t - a.Z * u);
    }

    /// <summary>
    /// ベクトル シーケンスの総和を計算します。
    /// </summary>
    /// <param name="source">対象のシーケンス。</param>
    /// <returns>対象シーケンスの総和。</returns>
    public static Vector4D Sum(IEnumerable<Vector4D> source)
    {
        return source.Aggregate((aqm, val) => aqm + val);
    }

    /// <summary>ベクトルの符号を反転します。</summary>
    public static Vector4D operator -(in Vector4D a) => new(-a.X, -a.Y, -a.Z, -a.W);

    /// <summary>ベクトルを加算します。</summary>
    public static Vector4D operator +(in Vector4D a, in Vector4D b) => new(a.X + b.X, a.Y + b.Y, a.Z + b.Z, a.W + b.W);

    /// <summary>ベクトルを減算します。</summary>
    public static Vector4D operator -(in Vector4D a, in Vector4D b) => new(a.X - b.X, a.Y - b.Y, a.Z - b.Z, a.W - b.W);

    /// <summary>ベクトルのスカラー倍を求めます。</summary>
    public static Vector4D operator *(double s, in Vector4D a) => new(a.X * s, a.Y * s, a.Z * s, a.W * s);

    /// <summary>ベクトルのスカラー倍を求めます。</summary>
    public static Vector4D operator *(in Vector4D a, double s) => new(a.X * s, a.Y * s, a.Z * s, a.W * s);

    /// <summary>ベクトルのスカラー除を求めます。</summary>
    public static Vector4D operator /(in Vector4D a, double s) => new(a.X / s, a.Y / s, a.Z / s, a.W / s);

    /// <summary>ベクトルが等しいか判断します。</summary>
    public static bool operator ==(in Vector4D left, in Vector4D right) => left.Equals(right);

    /// <summary>ベクトルが等しくないか判断します。</summary>
    public static bool operator !=(in Vector4D left, in Vector4D right) => !(left == right);

    // public static IVectorOperator<Vector4D> Operator { get; } = new OperatorClass();
    // 
    // private class OperatorClass : IVectorOperator<Vector4D>
    // {
    //     public Vector4D Zero => default;
    //     public Vector4D Create(double x, double y, double z) => new(x, y, z);
    //     public Vector4D Reverse(Vector4D vector, in SphericalRing ring)
    //     {
    //         var normal = (Vector4D)ring.Normal;
    //         return vector - normal * (2.0 * Dot(normal, vector));
    //     }
    // }
}
