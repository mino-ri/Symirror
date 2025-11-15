using System.Numerics;
using System.Runtime.InteropServices;

namespace Symirror3.Rendering;

[StructLayout(LayoutKind.Sequential)]
public struct Vertex
{
    public Vector3 Vector;
    public float Rhw;

    public override readonly string ToString() => $"({Vector.X}, {Vector.Y}, {Vector.Z})";
}
