using System.Numerics;
using System.Runtime.InteropServices;

namespace Symirror4.Rendering;

[StructLayout(LayoutKind.Sequential)]
public struct Vertex
{
    public Vector4 Vector;

    public override string ToString() => $"({Vector.X}, {Vector.Y}, {Vector.Z}, {Vector.W})";
}
