using System.Numerics;
using System.Runtime.InteropServices;
using IndirectX;

namespace Symirror3.Rendering
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Vertex
    {
        public Vector3 Vector;
        public float Rhw;
        public Color Color;

        public override string ToString() => $"({Vector.X}, {Vector.Y}, {Vector.Z})#{Color}";
    }
}
