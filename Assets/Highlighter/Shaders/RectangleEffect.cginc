
struct RectangleEffect
{
    float3 position;
    float alpha;
    uint width;
    uint height;
    uint softEdge;
    uint space;
};


StructuredBuffer<RectangleEffect> rectangleEffects;
int rectangleEffectsCount;

int InsideRect(float2 uvPoint, float2 center, uint width, uint height)
{
    float2 uDir = abs(uvPoint - center) * 2;
    return step(uDir.x, width) * step(uDir.y, height);
}


float GetSoftEdge(float2 uvPoint, float2 center, uint width, uint height, uint softEdge)
{
    float2 uDir = abs(uvPoint - center) * 2;
    softEdge *= 0.5;
    float2 inner = float2(width - softEdge, height - softEdge);
    float2 nc = float2(smoothstep(inner.x, width, uDir.x),
                       smoothstep(inner.y, height, uDir.y));
    
    float t = clamp(length(nc), 0.0, 1.0);
  
    return 1- t * sin(t * 1.570795);
}