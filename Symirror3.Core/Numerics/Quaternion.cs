using System;
using static System.Math;

namespace Symirror3.Core.Numerics
{
    public struct Quaternion : IEquatable<Quaternion>
    {
        public double X;
        public double Y;
        public double Z;
        public double W;

        public Quaternion(double x, double y, double z, double w) => (X, Y, Z, W) = (x, y, z, w);

        public void Deconstruct(out double x, out double y, out double z, out double w) => (x, y, z, w) = (X, Y, Z, W);

        public static readonly Quaternion Identity = new Quaternion(0.0, 0.0, 0.0, 1.0);

        public double LengthSquared => X * X + Y * Y + Z * Z + W * W;

        public double Length => Sqrt(LengthSquared);

        public Quaternion Normalize()
        {
            var invNorm = 1.0 / Length;
            return new Quaternion(X * invNorm, Y * invNorm, Z * invNorm, W * invNorm);
        }

        public Quaternion Conjugate => new Quaternion(-X, -Y, -Z, W);

        public Quaternion Inverse()
        {
            var invNorm = 1.0f / LengthSquared;
            return new Quaternion(-X * invNorm, -Y * invNorm, -Z * invNorm, W * invNorm);
        }

        public static Quaternion FromAxisAngle(Vector3 axis, double angle)
        {
            var halfAngle = angle * 0.5;
            var sin = Sin(halfAngle);
            var cos = Cos(halfAngle);
            return new Quaternion(axis.X * sin, axis.Y * sin, axis.Z * sin, cos);
        }

        public static Quaternion RotationX(double angle)
        {
            var halfAngle = angle * 0.5;
            return new Quaternion(Sin(halfAngle), 0.0, 0.0, Cos(halfAngle));
        }

        public static Quaternion RotationY(double angle)
        {
            var halfAngle = angle * 0.5;
            return new Quaternion(0.0, Sin(halfAngle), 0.0, Cos(halfAngle));
        }

        public static Quaternion RotationZ(double angle)
        {
            var halfAngle = angle * 0.5;
            return new Quaternion(0.0, 0.0, Sin(halfAngle), Cos(halfAngle));
        }

        public static double Dot(Quaternion a, Quaternion b) =>
            a.X * b.X + a.Y * b.Y + a.Z * b.Z + a.W * b.W;

        public bool Equals(Quaternion other) => X == other.X && Y == other.Y && Z == other.Z && W == other.W;

        public override bool Equals(object? obj) => obj is Quaternion other && Equals(other);

        public override int GetHashCode() => (X, Y, Z, W).GetHashCode();

        public override string ToString() => $"[{X}, {Y}, {Z}, {W}]";

        public Vector3 ToBasePoint()
        {
            return new Vector3(
                2.0 * (X * Z + Y * W),
                2.0 * (Y * Z - X * W),
                Z * Z + W * W - X * X - Y * Y);
        }

        public Vector3F ToBasePointF()
        {
            return new Vector3F(
                (float)(2.0 * (X * Z + Y * W)),
                (float)(2.0 * (Y * Z - X * W)),
                (float)(Z * Z + W * W - X * X - Y * Y));
        }

        public static Vector3 operator *(Quaternion a, Vector3 v)
        {
            var sx = a.W * v.X + a.Y * v.Z - a.Z * v.Y;
            var sy = a.W * v.Y + a.Z * v.X - a.X * v.Z;
            var sz = a.W * v.Z + a.X * v.Y - a.Y * v.X;
            var sw = a.X * v.X + a.Y * v.Y + a.Z * v.Z;

            return new Vector3(
                sx * a.W + sw * a.X + sz * a.Y - sy * a.Z,
                sy * a.W + sw * a.Y + sx * a.Z - sz * a.X,
                sz * a.W + sw * a.Z + sy * a.X - sx * a.Y);
        }

        public static Vector3F operator *(Quaternion a, Vector3F v)
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

        public static bool operator ==(Quaternion a, Quaternion b) => a.Equals(b);

        public static bool operator !=(Quaternion a, Quaternion b) => !a.Equals(b);

        public static Quaternion operator +(Quaternion a) => a;

        public static Quaternion operator -(Quaternion a) => new Quaternion(-a.X, -a.Y, -a.Z, -a.W);

        public static Quaternion operator +(Quaternion a, Quaternion b) =>
            new Quaternion(a.X + b.X, a.Y + b.Y, a.Z + b.Z, a.W + b.W);

        public static Quaternion operator -(Quaternion a, Quaternion b) =>
            new Quaternion(a.X - b.X, a.Y - b.Y, a.Z - b.Z, a.W - b.W);

        public static Quaternion operator *(Quaternion a, Quaternion b)
        {
            return new Quaternion(
                a.X * b.W + b.X * a.W + a.Y * b.Z - a.Z * b.Y,
                a.Y * b.W + b.Y * a.W + a.Z * b.X - a.X * b.Z,
                a.Z * b.W + b.Z * a.W + a.X * b.Y - a.Y * b.X,
                a.W * b.W - a.X * b.X - a.Y * b.Y - a.Z * b.Z);
        }

        public static Quaternion operator *(Quaternion a, double f) =>
            new Quaternion(a.X * f, a.Y * f, a.Z * f, a.W * f);

        public static Quaternion operator /(Quaternion a, Quaternion b) => a * b.Inverse();

        public static Quaternion operator /(Quaternion a, double f) =>
            new Quaternion(a.X / f, a.Y / f, a.Z / f, a.W / f);
    }
}
