using System;

namespace Symirror3.Core;

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

    public static double Angle(in SphericalRing a, in SphericalRing b) =>
        SphericalPoint.Distance(in a.Normal, in b.Normal);

    public static bool operator ==(in SphericalRing a, in SphericalRing b) => a.Equals(b);

    public static bool operator !=(in SphericalRing a, in SphericalRing b) => !a.Equals(b);
}
