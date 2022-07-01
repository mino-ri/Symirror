cbuffer cbWorldTransform : register(b0)
{
    matrix World;
};

cbuffer cbProjectionTransform : register(b1)
{
    matrix Projection;
    float4 Translate;
    float4 ViewPoint;
};

cbuffer cbMaterial : register(b2)
{
    float4 MaterialColor;
};

cbuffer cbLight : register(b3)
{
    float4 Sight;
    float3 LightSource;
    float ViewSize;
    float AmbientFactor;
    float DiffuseFactor;
    float SpecularFactor;
    float SpecularIndex;
    matrix LightViewProj;
};

Texture2D ShadowMap : register(t0);
SamplerComparisonState ShadowMapSampler : register(s0);

struct VS_INPUT
{
    float4 Pos : POSITION;
};

struct PS_INPUT
{
    float4 Pos : SV_POSITION;
    float4 Posw : POSITION0;
    float4 Normal : NORMAL;
};

VS_INPUT VS(VS_INPUT input)
{
    VS_INPUT output;

    output.Pos = mul(input.Pos, World) + Translate;

    return output;
}

[maxvertexcount(3)]
void GS(triangle VS_INPUT input[3], inout TriangleStream<PS_INPUT> TriStream)
{
    PS_INPUT output;

    float3 faceEdge0 = input[0].Pos.xyz * Projection._34 / input[0].Pos.w;
    float3 faceEdge1 = input[1].Pos.xyz * Projection._34 / input[1].Pos.w - faceEdge0;
    float3 faceEdge2 = input[2].Pos.xyz * Projection._34 / input[2].Pos.w - faceEdge0;
    float3 norm = normalize(cross(faceEdge1, faceEdge2));

    [unroll(3)]
    for (int i = 0; i < 3; ++i)
    {
        output.Posw.xyz = input[0].Pos.xyz * Projection._34;
        output.Posw.w = input[0].Pos.w;
        output.Normal.xyz = norm;
        output.Normal.w = 1;
        output.Pos = mul(input[i].Pos, Projection);
        TriStream.Append(output);
    }

    TriStream.RestartStrip();
}

float4 PS(PS_INPUT input) : SV_TARGET
{
    float4 t = mul(input.Posw, LightViewProj);
    t.xyz = t.xyz / t.w;
    t.xy = float2(0.5 + t.x * 0.5, 0.5 - t.y * 0.5) - input.Normal.xy / ViewSize;
    float shadow = ShadowMap.SampleCmpLevelZero(ShadowMapSampler, t.xy, t.z - input.Normal.z / ViewSize);
    shadow = 0.5 + shadow * 0.5;
    
    float3 lightDirection = normalize(input.Posw.xyz - LightSource.xyz);
    float3 cameraDirection = normalize(input.Posw.xyz - Sight.xyz);
    float3 halfVector = normalize(lightDirection + cameraDirection);
    float diffuse = max(0, dot(input.Normal.xyz, lightDirection)) * DiffuseFactor;
    float specular = pow(max(0, dot(input.Normal.xyz, halfVector)), SpecularIndex) * SpecularFactor;
    float4 color = MaterialColor;
    color.rgb = color.rgb * (AmbientFactor + diffuse * shadow) + specular.rrr * shadow;
    return color;
}
