using IndirectX;
using IndirectX.D3D11;
using IndirectX.Dxgi;
using IndirectX.Helper;
using System;
using System.Linq;
using System.Numerics;
using System.Threading;

namespace Symirror3.Rendering;

internal sealed class Graphics : IDisposable
{
    public const int TriangleCount = 28;
    private static readonly ushort[] _stripIndices;
    private static readonly ushort[] _fanIndices;
    private int _width;
    private int _height;
    private float _lightDistance = 2f;

    private readonly IndirectX.Helper.Graphics _graphics;
    private readonly ArrayBuffer<ushort> _indexBuffer;
    private readonly ArrayBuffer<Vertex> _vertexBuffer;
    private readonly ValueBuffer<TransformBuffer> _matrixBuffer;
    private readonly ValueBuffer<MaterialBuffer> _materialBuffer;
    private readonly ValueBuffer<LightBuffer> _lightBuffer;
    private readonly DepthStencilState _ignoreStencilState;
    private readonly DepthStencilState _stencilMaskState;
    private readonly DepthStencilState _writeStencilState;
    private DepthStencilView _shadowMapView;
    private readonly VertexShader _vertexShader;
    private readonly VertexShader _shadowMapVertexShader;
    private readonly PixelShader _pixelShader;
#if SHADOW_DEBUG
    private readonly PixelShader _shadowMapPixelShader;
#endif
    private ShaderResourceView _shadowMapResourceView;

    public ref Matrix4 World => ref _matrixBuffer.Value.World;
    public ref Matrix4 ViewProj => ref _matrixBuffer.Value.ViewProj;
    public Vertex[] Vertices => _vertexBuffer.Buffer;
    public ref LightBuffer LightBuffer => ref _lightBuffer.Value;

    internal Graphics(IntPtr surfaceHandle, int width, int height)
    {
        _width = width;
        _height = height;
        _lightDistance = 2f;
        _graphics = new IndirectX.Helper.Graphics(surfaceHandle, _width, _height, true, 60, 1, useStencil: true);
        _vertexShader = ShaderSource.LoadVertexShader(_graphics.Device);
        _shadowMapVertexShader = ShaderSource.LoadShadowVertexShader(_graphics.Device);
        _pixelShader = ShaderSource.LoadPixelShader(_graphics.Device);
#if SHADOW_DEBUG
        _shadowMapPixelShader = _graphics.CompilePixelShader(GetResource("ps_shadow.cfx"));
#endif
        _graphics.SetInputLayout(ShaderSource.LoadInputLayout,
            new InputElementDesc { SemanticName = "POSITION", Format = Format.R32G32B32A32Float },
            new InputElementDesc { SemanticName = "COLOR", Format = Format.R32G32B32A32Float, AlignedByteOffset = 16 });
        _graphics.Context.VertexShader.Shader = _vertexShader;
        _graphics.Context.PixelShader.Shader = _pixelShader;

        _vertexBuffer = _graphics.RegisterVertexBuffer<Vertex>(0, TriangleCount * 3);
        _indexBuffer = _graphics.RegisterIndexBuffer(TriangleCount * 3);
        _matrixBuffer = _graphics.RegisterConstantBuffer<TransformBuffer>(0, ShaderStages.VertexShader);
        _materialBuffer = _graphics.RegisterConstantBuffer<MaterialBuffer>(1, ShaderStages.PixelShader);
        _lightBuffer = _graphics.RegisterConstantBuffer<LightBuffer>(2, ShaderStages.VertexShader | ShaderStages.PixelShader);

        _lightBuffer.Value.AmbientFactor = 0.125f;
        _lightBuffer.Value.DiffuseFactor = 1f;
        _lightBuffer.Value.SpecularFactor = 0.25f;
        _lightBuffer.Value.SpecularIndex = 5f;
        _lightBuffer.Value.Sight = new Vector3(0f, 0f, -5f);

        World = Matrix4.Identity;
        SetSurfaceSize();

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

        (_shadowMapView, _shadowMapResourceView) = CreateShadowMap();

        using var shadowMapSamplerState = _graphics.Device.CreateSamplerState(new SamplerDesc
        {
            AddressU = TextureAddressMode.Border,
            AddressV = TextureAddressMode.Border,
            AddressW = TextureAddressMode.Border,
            BorderColor = new Float4([1f, 1f, 1f, 1f]),
            ComparisonFunc = ComparisonFunc.LessEqual,
            Filter = Filter.ComparisonMinMagMipPoint,
            MaxAnisotropy = 0,
            MipLODBias = 0f,
            MinLOD = 0f,
            MaxLOD = 3.402823466E+38f,
        });
        _graphics.Context.PixelShader.SetSamplers(0, shadowMapSamplerState);
    }

    private void SetSurfaceSize()
    {
        var size = Math.Min(_width, _height) * 0.4f;
        ViewProj = Matrix4.Scaling(size, size, 1f, 1f) *
                   Matrix4.LookAtLH(0f, 0f, 0f, 5f) *
                   Matrix4.PerspectiveLH(5f, 3f, 7f, _width, _height);
        _matrixBuffer.Flush();

        _materialBuffer.Value.NormalRhw = 1f;

        _lightBuffer.Value.ViewSize = Math.Min(_width, _height) / 2f;
        SetLightDistance(_lightDistance);
        _lightBuffer.Flush();
    }

    private (DepthStencilView shadowMapView, ShaderResourceView shadowMapResourceView) CreateShadowMap()
    {
        using var shadowMap = _graphics.Device.CreateTexture2D(new Texture2DDesc
        {
            Format = Format.R24G8Typeless,
            ArraySize = 1,
            MipLevels = 1,
            Width = _width,
            Height = _height,
            SampleDesc = { Count = 1, Quality = 0 },
            BindFlags = BindFlags.DepthStencil | BindFlags.ShaderResource,
        });

        var shadowMapView = _graphics.Device.CreateDepthStencilView(shadowMap, new DepthStencilViewDesc
        {
            Format = Format.D24UNormS8UInt,
            ViewDimension = DsvDimension.Texture2D,
            UnionPart0 = { Texture2D = { MipSlice = 0 } },
        });

        var shadowMapResourceView = _graphics.Device.CreateShaderResourceView(shadowMap, new ShaderResourceViewDesc
        {
            Format = Format.R24UNormX8Typeless,
            ViewDimension = SrvDimension.Texture2D,
            UnionPart0 = { Texture2D = { MipLevels = 1 } },
        });

        return (shadowMapView, shadowMapResourceView);
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

    private static readonly Vector3 _lightDirection =
        new Vector3(-0f, -0f, -1f) *
        Matrix4.RotationX(-0.6f) *
        Matrix4.RotationY(0.2f);
    public void SetLightDistance(float d)
    {
        _lightDistance = d;
        LightBuffer.LightSource = _lightDirection * d;
        var size = Math.Min(_width, _height) * 0.5f;
        LightBuffer.LightViewProj =
            Matrix4.RotationY(-0.2f) *
            Matrix4.RotationX(0.6f) *
            Matrix4.Scaling(size, size, 1f, 1f) *
            Matrix4.LookAtLH(0f, 0f, 0f, d) *
            Matrix4.PerspectiveLH(MathF.Sqrt(d * d - 1f), d - 1f, d + 1f, _width, _height);
    }

    public void FlushLight() => _lightBuffer.Flush();

    public void SetStripIndices() => _indexBuffer.Write(_stripIndices);

    public void SetFanIndices() => _indexBuffer.Write(_fanIndices);

    public void SetIndices(ushort[] indices) => _indexBuffer.Write(indices);

    public void SetIndices(Span<ushort> indices)
    {
        indices.CopyTo(_indexBuffer.Buffer);
        _indexBuffer.Flush();
    }

    public void IgnoreStencil() => _graphics.SetDepthStencilState(_ignoreStencilState, 0);

    public void BeginWriteStencil() => _graphics.SetDepthStencilState(_writeStencilState, 0);

    public void UseStencilMask() => _graphics.SetDepthStencilState(_stencilMaskState, 0);

    public void BeginShadowMap()
    {
        _graphics.Context.VertexShader.Shader = _shadowMapVertexShader;
#if SHADOW_DEBUG
            _graphics.Context.PixelShader.Shader = _shadowMapPixelShader;
            _graphics.Context.OutputMerger.SetRenderTarget(_graphics.RenderView, _shadowMapView);
#else
        _graphics.Context.PixelShader.Shader = null;
        _graphics.Context.OutputMerger.SetRenderTargets([], _shadowMapView);
#endif
    }

    public void EndShadowMap()
    {
        _graphics.Context.VertexShader.Shader = _vertexShader;
        _graphics.Context.PixelShader.Shader = _pixelShader;
        _graphics.Context.OutputMerger.SetRenderTarget(_graphics.RenderView, _graphics.DepthView);
        _graphics.Context.PixelShader.SetShaderResources(0, _shadowMapResourceView);
    }

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

    public void Clear() => _graphics.Clear(Color.Black, 1f);

    public void ClearShadowDepth() => _graphics.Context.ClearDepthStencilView(_shadowMapView, ClearFlags.Depth, 1f, 0);

    public void ClearStencil() => _graphics.ClearStencil(0);

    public void Present() => _graphics.Present();

    public void Resize(int width, int height)
    {
        if (width == _width && height == _height)
            return;

        _width = width;
        _height = height;
        _graphics.Resize(width, height);
        SetSurfaceSize();
        var (shadowMapView, shadowMapResourceView) = CreateShadowMap();
        Interlocked.Exchange(ref _shadowMapView, shadowMapView)?.Dispose();
        Interlocked.Exchange(ref _shadowMapResourceView, shadowMapResourceView)?.Dispose();
    }

    public void Dispose()
    {
        _ignoreStencilState.Dispose();
        _stencilMaskState.Dispose();
        _writeStencilState.Dispose();
        _shadowMapView.Dispose();
        _vertexShader.Dispose();
        _pixelShader.Dispose();
        _shadowMapVertexShader.Dispose();
#if SHADOW_DEBUG
            _shadowMapPixelShader.Dispose();
#endif
        _shadowMapResourceView.Dispose();
        _graphics.Dispose();
    }

    static Graphics()
    {
        _stripIndices =
            [ .. Enumerable
                .Range(0, TriangleCount)
                .SelectMany(i => new[] { (ushort)i, (ushort)(i + 1), (ushort)(i + 2) })
            ];
        _fanIndices =
            [.. Enumerable
                .Range(0, TriangleCount)
                .SelectMany(i => new[] { (ushort)0, (ushort)(i + 1), (ushort)(i + 2) })
            ];
    }
}
