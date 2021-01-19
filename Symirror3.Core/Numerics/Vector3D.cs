using System;
using System.Runtime.InteropServices;

namespace Symirror3.Core.Numerics
{
    /// <summary>3次元上の座標を表します。</summary>
    [StructLayout(LayoutKind.Sequential)]
    public readonly struct Vector3D : IEquatable<Vector3D>
    {
        /// <summary>X座標。</summary>
        public readonly double X;

        /// <summary>Y座標。</summary>
        public readonly double Y;

        /// <summary>Z座標。</summary>
        public readonly double Z;

        public double Magnitude => Math.Sqrt(X * X + Y * Y + Z * Z);

        public Vector3D Normalize()
        {
            var mag = Magnitude;
            if (mag == 0) return this;
            return this / Magnitude;
        }

        /// <summary><see cref="Vector3D"/>構造体の新しいインスタンスを生成します。</summary>
        /// <param name="x">X成分。</param>
        /// <param name="y">Y成分。</param>
        /// <param name="z">Z成分。</param>
        public Vector3D(double x, double y, double z) => (X, Y, Z) = (x, y, z);

        /// <summary>このオブジェクトを、それと等価な文字列に変換します。</summary>
        /// <returns>現在のオブジェクトを表す<see cref="string"/>。</returns>
        public override string ToString() => $"({X}, {Y}, {Z})";

        public void Deconstruct(out double x, out double y, out double z) => (x, y, z) = (X, Y, Z);

        public bool Equals(Vector3D other) => (X, Y, Z) == (other.X, other.Y, other.Z);

        public override bool Equals(object? obj) => obj is Vector3D other && other.Equals(this);

        public override int GetHashCode() => (X, Y, Z).GetHashCode();

        /// <summary>ベクトルの符号を反転します。</summary>
        public static Vector3D operator -(Vector3D a) => new(-a.X, -a.Y, -a.Z);

        /// <summary>ベクトルを加算します。</summary>
        public static Vector3D operator +(Vector3D a, Vector3D b) => new(a.X + b.X, a.Y + b.Y, a.Z + b.Z);

        /// <summary>ベクトルを減算します。</summary>
        public static Vector3D operator -(Vector3D a, Vector3D b) => new(a.X - b.X, a.Y - b.Y, a.Z - b.Z);

        /// <summary>ベクトルのスカラー倍を求めます。</summary>
        public static Vector3D operator *(double s, Vector3D a) => new(a.X * s, a.Y * s, a.Z * s);

        /// <summary>ベクトルのスカラー倍を求めます。</summary>
        public static Vector3D operator *(Vector3D a, double s) => new(a.X * s, a.Y * s, a.Z * s);

        /// <summary>ベクトルのスカラー除を求めます。</summary>
        public static Vector3D operator /(Vector3D a, double s) => new(a.X / s, a.Y / s, a.Z / s);

        /// <summary>ベクトルの内積を求めます。</summary>
        public static double operator *(Vector3D a, Vector3D b) => a.X * b.X + a.Y * b.Y + a.Z * b.Z;

        /// <summary>ベクトルの外積を求めます。</summary>
        public static Vector3D operator %(Vector3D a, Vector3D b) => new(a.Y * b.Z - a.Z * b.Y, a.Z * b.X - a.X * b.Z, a.X * b.Y - a.Y * b.X);

        /// <summary>ベクトルが等しいか判断します。</summary>
        public static bool operator ==(Vector3D left, Vector3D right) => left.Equals(right);

        /// <summary>ベクトルが等しくないか判断します。</summary>
        public static bool operator !=(Vector3D left, Vector3D right) => !(left == right);

        public static IVectorOperator<Vector3D> Operator { get; } = new OperatorClass();

        private class OperatorClass : IVectorOperator<Vector3D>
        {
            public Vector3D Zero => default;
            public Vector3D Create(double x, double y, double z) => new(x, y, z);
            public Vector3D Negate(Vector3D x) => -x;
            public Vector3D Add(Vector3D x, Vector3D y) => x + y;
            public Vector3D Subtract(Vector3D x, Vector3D y) => x - y;
            public Vector3D Multiply(Vector3D x, double scalar) => x * scalar;
            public Vector3D Divide(Vector3D x, double scalar) => x / scalar;
            public double Dot(Vector3D x, Vector3D y) => x * y;
            public Vector3D Cross(Vector3D x, Vector3D y) => x % y;
            public Vector3D Normalize(Vector3D x) => x.Normalize();
            public bool NearlyEqual(Vector3D x, Vector3D y, double error) =>
                Math.Abs(x.X - y.X) < error &&
                Math.Abs(x.Y - y.Y) < error &&
                Math.Abs(x.Z - y.Z) < error;

        }
    }
}
