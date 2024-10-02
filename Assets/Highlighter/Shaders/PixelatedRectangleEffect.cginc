
struct PixelatedRectangleEffect
{
    float3 position;
    float alpha;
    uint pixelSize;
    uint width;
    uint height;
    uint softEdge;
    uint space;
};

StructuredBuffer<PixelatedRectangleEffect> pixelatedRectangleEffects;
int pixelatedRectangleEffectsCount;

float2 GetRectangularPixelatedUV(float2 screenPoint, float screenWidth, float screenHeight, uint pixelSize)
{
    if (pixelSize == 0)
        pixelSize = 1;
    
    float fx = screenPoint.x % pixelSize;
    float fy = screenPoint.y % pixelSize;
    screenPoint.x = screenPoint.x - fx;
    screenPoint.y = screenPoint.y - fy;
    screenPoint.x /= screenWidth;
    screenPoint.y /= screenHeight;
    return screenPoint;
}
