using System.Numerics;
using System.Runtime.InteropServices;
using IndirectX;

namespace Symirror3.Rendering
{
    [StructLayout(LayoutKind.Explicit)]
    public struct Vertex
    {
        [FieldOffset(0)]
        public Vector3 Vector;
        [FieldOffset(12)]
        public float Rhw;
        [FieldOffset(16)]
        public Color Color;

        public override string ToString() => $"({Vector.X}, {Vector.Y}, {Vector.Z})#{Color}";
    }
}
