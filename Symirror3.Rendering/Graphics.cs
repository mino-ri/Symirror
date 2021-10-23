using System;
using System.Linq;
using IndirectX;
using IndirectX.Dxgi;
using IndirectX.D3D11;
using IndirectX.EasyRenderer;
using System.Reflection;
using System.Numerics;

namespace Symirror3.Rendering
{
    internal sealed class Graphics : IDisposable
    {
        public const int TriangleCount = 28;
        private static readonly ushort[] _stripIndices;
        private static readonly ushort[] _fanIndices;

        private readonly IndirectX.EasyRenderer.Graphics _graphics;
        private readonly ArrayBuffer<ushort> _indexBuffer;
        private readonly ArrayBuffer<Vertex> _vertexBuffer;
        private readonly ValueBuffer<TransformBuffer> _matrixBuffer;
        private readonly ValueBuffer<MaterialBuffer> _materialBuffer;
        private readonly ValueBuffer<LightBuffer> _lightBuffer;
        private readonly DepthStencilState _ignoreStencilState;
        private readonly DepthStencilState _stencilMaskState;
        private readonly DepthStencilState _writeStencilState;

        public ref Matrix4 World => ref _matrixBuffer.Value.World;
        private ref Matrix4 ViewProj => ref _matrixBuffer.Value.ViewProj;
        public Vertex[] Vertices => _vertexBuffer.Buffer;
        public ref LightBuffer LightBuffer => ref _lightBuffer.Value;

        internal Graphics(IntPtr surfaceHandle, int width, int height)
        {
            _graphics = new IndirectX.EasyRenderer.Graphics(surfaceHandle, width, height, true, 60, 1, useStencil: true);
            using (var shader = new ShaderSource(ShaderCode,
                new()
                {
                    [ShaderStages.VertexShader] = ("VS", "vs_5_0"),
                    [ShaderStages.PixelShader] = ("PS", "ps_5_0"),
                },
                new InputElementDesc { SemanticName = "POSITION", Format = Format.R32G32B32A32Float },
                new InputElementDesc { SemanticName = "COLOR", Format = Format.R32G32B32A32Float, AlignedByteOffset = 16 }))
                _graphics.Set(shader);

            _vertexBuffer = _graphics.RegisterVertexBuffer<Vertex>(0, TriangleCount * 3);
            _indexBuffer = _graphics.RegisterIndexBuffer(TriangleCount * 3);
            _matrixBuffer = _graphics.RegisterConstantBuffer<TransformBuffer>(0, ShaderStages.VertexShader);
            _materialBuffer = _graphics.RegisterConstantBuffer<MaterialBuffer>(1, ShaderStages.PixelShader);
            _lightBuffer = _graphics.RegisterConstantBuffer<LightBuffer>(2, ShaderStages.PixelShader);

            var size = Math.Min(width, height) * 0.4f;
            ViewProj = Matrix4.LookAtLH(0f, 0f, 0f, 5f) *
                        Matrix4.Scaling(size, size, 1f, 1f) *
                        Matrix4.PerspectiveLH(5f, 0.3f, 10f, width, height);
            World = Matrix4.Identity;
            _matrixBuffer.Flush();

            _materialBuffer.Value.NormalRhw = 1f;

            _lightBuffer.Value.AmbientFactor = 0.125f;
            _lightBuffer.Value.DiffuseFactor = 1f;
            _lightBuffer.Value.SpecularFactor = 0.25f;
            _lightBuffer.Value.SpecularIndex = 5f;
            _lightBuffer.Value.Sight = new Vector4(0f, 0f, -5f, 1f);
            _lightBuffer.Value.LightSource = new Vector3(-2f, -2f, -4f);
            _lightBuffer.Value.LightSourceRhw = 1f;
            _lightBuffer.Flush();

            _graphics.PrimitiveTopology = PrimitiveTopology.TriangleList;
            for (var i = 0; i < _vertexBuffer.Buffer.Length; i++)
                _vertexBuffer.Buffer[i].Rhw = 1f;

            _ignoreStencilState = _graphics.CreateDepthStencilState(new DepthStencilDesc
            {
                DepthEnable = true,
                DepthFunc = ComparisonFunc.Less,
                DepthWriteMask = DepthWriteMask.All,
                StencilEnable = false,
                StencilReadMask = 0xff,
                StencilWriteMask = 0xff,
                FrontFace =
                {
                    StencilFunc = ComparisonFunc.Never,
                    StencilPassOp = StencilOp.Keep,
                    StencilDepthFailOp = StencilOp.Keep,
                    StencilFailOp = StencilOp.Keep,
                },
                BackFace =
                {
                    StencilFunc = ComparisonFunc.Never,
                    StencilPassOp = StencilOp.Keep,
                    StencilDepthFailOp = StencilOp.Keep,
                    StencilFailOp = StencilOp.Keep,
                },
            });

            _stencilMaskState = _graphics.CreateDepthStencilState(new DepthStencilDesc
            {
                DepthEnable = true,
                DepthFunc = ComparisonFunc.Less,
                DepthWriteMask = DepthWriteMask.All,
                StencilEnable = true,
                StencilReadMask = 0xff,
                StencilWriteMask = 0xff,
                FrontFace =
                {
                    StencilFunc = ComparisonFunc.NotEqual,
                    StencilPassOp = StencilOp.Zero,
                    StencilDepthFailOp = StencilOp.Zero,
                    StencilFailOp = StencilOp.Keep,
                },
                BackFace =
                {
                    StencilFunc = ComparisonFunc.NotEqual,
                    StencilPassOp = StencilOp.Zero,
                    StencilDepthFailOp = StencilOp.Zero,
                    StencilFailOp = StencilOp.Keep,
                },
            });

            _writeStencilState = _graphics.CreateDepthStencilState(new DepthStencilDesc
            {
                DepthEnable = true,
                DepthFunc = ComparisonFunc.Never,
                DepthWriteMask = DepthWriteMask.All,
                StencilEnable = true,
                StencilReadMask = 0xff,
                StencilWriteMask = 0xff,
                FrontFace =
                {
                    StencilFunc = ComparisonFunc.Always,
                    StencilPassOp = StencilOp.Invert,
                    StencilDepthFailOp = StencilOp.Invert,
                    StencilFailOp = StencilOp.Keep,
                },
                BackFace =
                {
                    StencilFunc = ComparisonFunc.Always,
                    StencilPassOp = StencilOp.Invert,
                    StencilDepthFailOp = StencilOp.Invert,
                    StencilFailOp = StencilOp.Keep,
                },
            });

            _graphics.SetDepthStencilState(_ignoreStencilState, 0);
        }

        public void FlushWorld() => _matrixBuffer.Flush();

        public void SetWorld(in Matrix4 world)
        {
            World = world;
            _matrixBuffer.Flush();
        }

        public void SetMaterial(Color color, Vector3 normal)
        {
            _materialBuffer.Value.MaterialColor = color;
            _materialBuffer.Value.Normal = normal;
            _materialBuffer.Flush();
        }

        public void FlushLight() => _lightBuffer.Flush();

        public void SetStripIndices() => _indexBuffer.Write(_stripIndices);

        public void SetFanIndices() => _indexBuffer.Write(_fanIndices);

        public void IgnoreStencil() => _graphics.SetDepthStencilState(_ignoreStencilState, 0);

        public void BeginWriteStencil() => _graphics.SetDepthStencilState(_writeStencilState, 0);

        public void UseStencilMask() => _graphics.SetDepthStencilState(_stencilMaskState, 0);

        public void DrawList(int triangleCount)
        {
            _vertexBuffer.Flush();
            _graphics.DrawList(triangleCount * 3);
        }

        public void DrawIndexed(int triangleCount)
        {
            _vertexBuffer.Flush();
            _graphics.DrawIndexedList(triangleCount * 3);
        }

        public void Clear() => _graphics.Clear(Color.Black);

        public void ClearStencil() => _graphics.ClearStencil(0);

        public void Present() => _graphics.Present();

        public void Dispose()
        {
            _ignoreStencilState.Dispose();
            _stencilMaskState.Dispose();
            _writeStencilState.Dispose();
            _graphics.Dispose();
        }

        static Graphics()
        {
            _stripIndices = Enumerable.Range(0, TriangleCount)
                .SelectMany(i => new[] { (ushort)i, (ushort)(i + 1), (ushort)(i + 2) })
                .ToArray();
            _fanIndices = Enumerable.Range(0, TriangleCount)
                .SelectMany(i => new[] { (ushort)0, (ushort)(i + 1), (ushort)(i + 2) })
                .ToArray();
        }

        private static byte[] ShaderCode
        {
            get
            {
                using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Symirror3.Rendering.shader.hlsl");
                if (stream is null) return Array.Empty<byte>();
                // skip BOM
                stream.Seek(3, System.IO.SeekOrigin.Begin);
                // 1 longer because appending '\0' end
                var buffer = new byte[stream.Length - 2];
                stream.Read(buffer.AsSpan());
                return buffer;
            }
        }
    }
}
