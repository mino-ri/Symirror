using System;

namespace Symirror3.Core
{
    public struct SphericalRing : IEquatable<SphericalRing>
    {
        public SphericalPoint Normal;

        public SphericalRing(in SphericalPoint normal) => Normal = normal;

        public SphericalRing(in SphericalPoint a, in SphericalPoint b) =>
            Normal = SphericalPoint.Cross(in a, in b);

        public readonly bool Equals(SphericalRing other) => other.Normal == Normal;

        public readonly override bool Equals(object? obj) => obj is SphericalRing s && Equals(s);

        public readonly override int GetHashCode() => Normal.GetHashCode();

        public readonly override string ToString() => Normal.ToString();

        public readonly SphericalPoint Reverse(in SphericalPoint point) =>
            (SphericalPoint)(point - Normal * (2.0 * SphericalPoint.Dot(in Normal, in point)));

        public readonly T Reverse<T>(T vector, IVectorOperator<T> opr)
        {
            var n = opr.Convert(Normal);
            return opr.Subtract(vector, opr.Multiply(n, 2.0 * opr.Dot(n, vector)));
        }

        public static double Angle(in SphericalRing a, in SphericalRing b) =>
            SphericalPoint.Distance(in a.Normal, in b.Normal);
    }
}
