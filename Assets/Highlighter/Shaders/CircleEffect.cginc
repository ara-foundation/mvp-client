struct CircleEffect
{
    float3 position;
    float alpha;
    uint radius;
    uint softEdge;
    uint space;
};

StructuredBuffer<CircleEffect> circleEffects;
int circleEffectsCount;

float InCircle(float2 pt, float2 center, float radius, float blur)
{
    float len = length(pt - center);
    return 1 - smoothstep(radius - blur, radius, len);
}
