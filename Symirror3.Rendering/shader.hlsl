cbuffer cbWorldTransform : register(b0)
{
    matrix World;
    matrix ViewProj;
};

cbuffer cbMaterial : register(b1)
{
    float4 MaterialColor;
    float4 Normal;
};

cbuffer cbLight : register(b2)
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
};

PS_INPUT VS(VS_INPUT input)
{
    PS_INPUT output;

    output.Posw = mul(input.Pos, World);
    output.Pos = mul(output.Posw, ViewProj);

    return output;
}

float4 PS(PS_INPUT input) : SV_TARGET
{
    float4 t = mul(input.Posw, LightViewProj);
    t.xyz = t.xyz / t.w;
    t.xy = float2(0.5 + t.x * 0.5, 0.5 - t.y * 0.5) - Normal.xy / ViewSize;
    float shadow = ShadowMap.SampleCmpLevelZero(ShadowMapSampler, t.xy, t.z - Normal.z / ViewSize);
    shadow = 0.5 + shadow * 0.5;
    
    float3 lightDirection = normalize(input.Posw.xyz - LightSource.xyz);
    float3 cameraDirection = normalize(input.Posw.xyz - Sight.xyz);
    float3 halfVector = normalize(lightDirection + cameraDirection);
    float diffuse = max(0, dot(Normal.xyz, lightDirection)) * DiffuseFactor;
    float specular = pow(max(0, dot(Normal.xyz, halfVector)), SpecularIndex) * SpecularFactor;
    float4 color = MaterialColor;
    color.rgb = color.rgb * (AmbientFactor + diffuse * shadow) + specular.rrr * shadow;
    return color;
}

PS_INPUT VS_ShadowMap(VS_INPUT input)
{
    PS_INPUT output;
    output.Posw = mul(input.Pos, World);
    output.Pos = mul(output.Posw, LightViewProj);
    return output;
}

float4 PS_ShadowMap(PS_INPUT input) : SV_TARGET
{
    float4 color;
    color.a = 1;
    color.rgb = input.Pos.zzz;
    return color;
}
