struct RadialWaveEffect
{
    float3 position;
    float alpha;
    float4 color;
    uint radius;
    uint thickness;
    float speed;
    uint space;
};

StructuredBuffer<RadialWaveEffect> radialWaveEffects;
int radialWaveEffectsCount;

float GetCircleEdgeShade(float time, float2 pt, float2 center, uint radius, uint thickness)
{
    float pi = 3.14159;
    float t = frac(time) * pi / 2;
    float curentRad = radius * abs(sin(t));
    float currentThick = curentRad * (thickness / curentRad);
    
    float len = length(pt - center);
    t = smoothstep(curentRad - currentThick, curentRad, len);
    
    t = lerp(-1, 1, t);
    return (cos(t * t * pi) + 1) / 2;
}


float GetFadeInAlpha(float time)
{
    float pi = 3.14159;
    time = frac(time);
    return cos(time * time * pi / 2);
}