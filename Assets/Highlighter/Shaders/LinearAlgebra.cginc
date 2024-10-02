
int screenWidth;
int screenHeight;

float4x4 worldToCameraMatrix;
float4x4 projectionMatrix;

bool OnScreenSpace(uint space)
{
    return space == 1;
}

bool IsFrontOfCamera(float3 worldPoint)
{
    float4 cameraSpacePosition = mul(worldToCameraMatrix, float4(worldPoint, 1.0));
    return cameraSpacePosition.z <= 0;
}

float2 WorldToScreen(float3 worldPoint)
{
    float4 cameraSpacePosition = mul(worldToCameraMatrix, float4(worldPoint, 1.0f));
    float4 clipSpacePosition = mul(projectionMatrix, cameraSpacePosition);
    float2 normalizeScreenPoint = ((clipSpacePosition / clipSpacePosition.w) * 0.5) + 0.5;
    float2 screenPoint = float2(normalizeScreenPoint.x * screenWidth, normalizeScreenPoint.y * screenHeight);
    
    return screenPoint;
}
