using IndirectX;
using System.Numerics;
using System.Runtime.InteropServices;

namespace Symirror3.Rendering
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct TransformBuffer
    {
        public Matrix4 World;
        public Matrix4 ViewProj;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct MaterialBuffer
    {
        public Color MaterialColor;
        public Vector3 Normal;
        public float NormalRhw;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct LightBuffer
    {
        public Vector4 Sight;
        public Vector3 LightSource;
        public float LightSourceRhw;
        public float AmbientFactor;
        public float DiffuseFactor;
        public float SpecularFactor;
        public float SpecularIndex;
    }
}
