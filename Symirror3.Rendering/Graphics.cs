using System;
using System.Numerics;
using System.Linq;
using IndirectX;
using IndirectX.EasyRenderer;
using IndirectX.D3D11;
using IndirectX.Dxgi;

namespace Symirror3.Rendering
{
    internal sealed class Graphics : IDisposable
    {
        private const int TriangleCount = 28;
        private static readonly ushort[] _stripIndices;
        private static readonly ushort[] _fanIndices;

        private readonly IndirectX.EasyRenderer.Graphics _graphics;
        private readonly ArrayBuffer<ushort> _indexBuffer;
        private readonly ArrayBuffer<Vertex> _vertexBuffer;
        private readonly ValueBuffer<Matrix4> _matrixBuffer;
        private Matrix4 _world;
        private Matrix4 _viewProj;

        public ref Matrix4 World => ref _world;
        public Vertex[] Vertices => _vertexBuffer.Buffer;

        internal Graphics(IntPtr surfaceHandle, int width, int height)
        {
            _graphics = new IndirectX.EasyRenderer.Graphics(surfaceHandle, width, height, true, 60, 1);
            using (var shader = new ShaderSource(ShaderCode,
                new()
                {
                    [ShaderStages.VertexShader] = ("VS", "vs_5_0"),
                    [ShaderStages.PixelShader] = ("PS", "ps_5_0"),
                },
                new InputElementDesc { SemanticName = "POSITION", Format = Format.R32G32B32A32Float },
                new InputElementDesc { SemanticName = "COLOR", Format = Format.R32G32B32A32Float, AlignedByteOffset = 16 }))
                _graphics.Set(shader);

            _vertexBuffer = _graphics.RegisterVertexBuffer<Vertex>(0, TriangleCount + 2);
            _indexBuffer = _graphics.RegisterIndexBuffer(TriangleCount * 3);
            _matrixBuffer = _graphics.RegisterConstantBuffer<Matrix4>(0, ShaderStages.VertexShader);
            var size = Math.Min(width, height) * 0.4f;
            _viewProj = Matrix4.LookAtLH(0f, 0f, 0f, 5f) *
                        Matrix4.Scaling(size, size, 1f, 1f) *
                        Matrix4.PerspectiveLH(5f, 0.3f, 10f, width, height);
            _world = Matrix4.Identity;
            _matrixBuffer.WriteByRef(_world * _viewProj);
            _graphics.PrimitiveTopology = PrimitiveTopology.TriangleList;
            for (var i = 0; i < _vertexBuffer.Buffer.Length; i++)
                _vertexBuffer.Buffer[i].Rhw = 1f;
        }

        public void FlushWorld() => _matrixBuffer.WriteByRef(_world * _viewProj);

        public void SetWorld(in Matrix4 world)
        {
            _world = world;
            _matrixBuffer.WriteByRef(_world * _viewProj);
        }

        public void SetStripIndices() => _indexBuffer.Write(_stripIndices);

        public void SetFanIndices() => _indexBuffer.Write(_fanIndices);

        public void Draw(int triangleCount)
        {
            _vertexBuffer.Flush();
            _graphics.DrawIndexedList(triangleCount * 3);
            var v = new Vector4(Vertices[0].Vector, 1f) * (_world * _viewProj);
            Console.WriteLine($"{v.X}, {v.Y}, {v.Z}, {v.W}");
        }

        public void Clear() => _graphics.Clear(Color.Black);

        public void Present() => _graphics.Present();

        public void Dispose() => _graphics.Dispose();

        static Graphics()
        {
            _stripIndices = Enumerable.Range(0, TriangleCount)
                .SelectMany(i => new[] { (ushort)i, (ushort)(i + 1), (ushort)(i + 2) })
                .ToArray();
            _fanIndices = Enumerable.Range(0, TriangleCount)
                .SelectMany(i => new[] { (ushort)0, (ushort)(i + 1), (ushort)(i + 2) })
                .ToArray();
        }

        private const string ShaderCode =
@"cbuffer cbWorldTransform : register(b0) {
    matrix WorldViewProj;
};

struct VS_INPUT {
    float4 Pos : POSITION;
    float4 Col : COLOR;
};

struct PS_INPUT {
    float4 Pos : SV_POSITION;
    float4 Col : COLOR;
};

PS_INPUT VS(VS_INPUT input)
{
    PS_INPUT output;

    output.Pos = mul(input.Pos, WorldViewProj);
    output.Col = input.Col;

    return output;
}

float4 PS(PS_INPUT input) : SV_TARGET
{
    return input.Col;
}";
    }
}
