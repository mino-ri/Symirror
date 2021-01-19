using System;
using System.Runtime.InteropServices;

namespace Symirror3.Core.Numerics
{
    /// <summary>3次元上の座標を表します。</summary>
    [StructLayout(LayoutKind.Sequential)]
    public readonly struct Vector3F : IEquatable<Vector3F>
    {
        /// <summary>X座標。</summary>
        public readonly float X;

        /// <summary>Y座標。</summary>
        public readonly float Y;

        /// <summary>Z座標。</summary>
        public readonly float Z;

        public float Magnitude => (float)Math.Sqrt(X * X + Y * Y + Z * Z);

        public Vector3F Normalize()
        {
            var mag = Magnitude;
            if (mag == 0) return this;
            return this / Magnitude;
        }

        /// <summary><see cref="Vector3F"/>構造体の新しいインスタンスを生成します。</summary>
        /// <param name="x">X成分。</param>
        /// <param name="y">Y成分。</param>
        /// <param name="z">Z成分。</param>
        public Vector3F(float x, float y, float z) => (X, Y, Z) = (x, y, z);

        /// <summary>このオブジェクトを、それと等価な文字列に変換します。</summary>
        /// <returns>現在のオブジェクトを表す<see cref="string"/>。</returns>
        public override string ToString() => $"({X}, {Y}, {Z})";

        public void Deconstruct(out float x, out float y, out float z) => (x, y, z) = (X, Y, Z);

        public bool Equals(Vector3F other) => (X, Y, Z) == (other.X, other.Y, other.Z);

        public override bool Equals(object? obj) => obj is Vector3F other && other.Equals(this);

        public override int GetHashCode() => (X, Y, Z).GetHashCode();

        /// <summary>ベクトルの内積を求めます。</summary>
        public static float Dot(Vector3F a, Vector3F b) => a.X * b.X + a.Y * b.Y + a.Z * b.Z;

        /// <summary>ベクトルの外積を求めます。</summary>
        public static Vector3F Cross(Vector3F a, Vector3F b) => new(a.Y * b.Z - a.Z * b.Y, a.Z * b.X - a.X * b.Z, a.X * b.Y - a.Y * b.X);

        /// <summary>ベクトルの符号を反転します。</summary>
        public static Vector3F operator -(Vector3F a) => new(-a.X, -a.Y, -a.Z);

        /// <summary>ベクトルを加算します。</summary>
        public static Vector3F operator +(Vector3F a, Vector3F b) => new(a.X + b.X, a.Y + b.Y, a.Z + b.Z);

        /// <summary>ベクトルを減算します。</summary>
        public static Vector3F operator -(Vector3F a, Vector3F b) => new(a.X - b.X, a.Y - b.Y, a.Z - b.Z);

        /// <summary>ベクトルのスカラー倍を求めます。</summary>
        public static Vector3F operator *(float s, Vector3F a) => new(a.X * s, a.Y * s, a.Z * s);

        /// <summary>ベクトルのスカラー倍を求めます。</summary>
        public static Vector3F operator *(Vector3F a, float s) => new(a.X * s, a.Y * s, a.Z * s);

        /// <summary>ベクトルのスカラー除を求めます。</summary>
        public static Vector3F operator /(Vector3F a, float s) => new(a.X / s, a.Y / s, a.Z / s);

        /// <summary>ベクトルが等しいか判断します。</summary>
        public static bool operator ==(Vector3F left, Vector3F right) => left.Equals(right);

        /// <summary>ベクトルが等しくないか判断します。</summary>
        public static bool operator !=(Vector3F left, Vector3F right) => !(left == right);

        public static IVectorOperator<Vector3F> Operator { get; } = new OperatorClass();

        private class OperatorClass : IVectorOperator<Vector3F>
        {
            public Vector3F Zero => default;
            public Vector3F Create(double x, double y, double z) => new((float)x, (float)y, (float)z);
            public Vector3F Reverse(Vector3F vector, in SphericalRing ring)
            {
                var normal = (Vector3F)ring.Normal;
                return vector - normal * (2f * Dot(normal, vector));
            }
        }
    }
}
