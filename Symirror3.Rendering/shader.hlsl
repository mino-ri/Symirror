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
    float4 LightSource;
    float AmbientFactor;
    float DiffuseFactor;
    float SpecularFactor;
    float SpecularIndex;
};

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
    float3 lightDirection = normalize(input.Posw.xyz - LightSource.xyz);
    float3 cameraDirection = normalize(input.Posw.xyz - Sight.xyz);
    float3 halfVector = normalize(lightDirection + cameraDirection);
    float diffuse = max(0, dot(Normal.xyz, lightDirection)) * DiffuseFactor;
    float specular = pow(max(0, dot(Normal.xyz, halfVector)), SpecularIndex) * SpecularFactor;
    float4 color = MaterialColor;
    color.rgb = color.rgb * (AmbientFactor + diffuse) + specular.rrr;
    return color;
}
