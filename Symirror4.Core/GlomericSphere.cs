using System;

namespace Symirror4.Core;

public struct GlomericSphere : IEquatable<GlomericSphere>
{
    public GlomericPoint Normal;

    public GlomericSphere(in GlomericPoint normal) => Normal = normal;

    public GlomericSphere(in GlomericPoint a, in GlomericPoint b, in GlomericPoint c) =>
        Normal = GlomericPoint.Cross(in a, in b, in c);

    public readonly bool Equals(GlomericSphere other) => other.Normal == Normal;

    public readonly override bool Equals(object? obj) => obj is GlomericSphere s && Equals(s);

    public readonly override int GetHashCode() => Normal.GetHashCode();

    public readonly override string ToString() => Normal.ToString();

    public readonly GlomericPoint Reverse(in GlomericPoint point) =>
        (GlomericPoint)(point - Normal * (2.0 * GlomericPoint.Dot(in Normal, in point)));

    public static double Angle(in GlomericSphere a, in GlomericSphere b) =>
        GlomericPoint.Distance(in a.Normal, in b.Normal);

    public static bool operator ==(in GlomericSphere a, in GlomericSphere b) => a.Equals(b);

    public static bool operator !=(in GlomericSphere a, in GlomericSphere b) => !a.Equals(b);
}
