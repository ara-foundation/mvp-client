  
struct PixelatedCircleEffect
{
    float3 position;
    float alpha;
    uint pixelSize;
    uint radius;
    uint softEdge;
    uint space;
};

StructuredBuffer<PixelatedCircleEffect> pixelatedCircleEffects;
int pixelatedCircleEffectsCount;



float2 GetCircularPixelatedUV(float2 screenPoint, float screenWidth, float screenHeight, uint pixelSize)
{
    if (pixelSize > 0)
    {
        screenPoint += pixelSize * 0.5;
        screenPoint = (uint2) (screenPoint / pixelSize);
        screenPoint *= pixelSize;
    }
    
    screenPoint.x /= screenWidth;
    screenPoint.y /= screenHeight;
    return screenPoint;
}
