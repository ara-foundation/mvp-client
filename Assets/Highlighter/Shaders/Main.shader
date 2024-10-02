Shader"Highlighter/Main"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        backgroundColor("Background Color", Color) = (1, 0, 0, 1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag


            #include "UnityCG.cginc"
            #include "LinearAlgebra.cginc"
            #include "RectangleEffect.cginc"
            #include "CircleEffect.cginc"
            #include "RadialWaveEffect.cginc"
            #include "PixelatedCircleEffect.cginc"
            #include "PixelatedRectangleEffect.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

           
            float time;

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 backgroundColor;



            fixed4 GetRectangularColor(float2 uv, fixed4 inputColor, fixed4 originalColor)
            {
                fixed4 outputColor = inputColor;
                [unroll(20)]
                for (int i = 0; i < rectangleEffectsCount; i++)
                {
                    RectangleEffect param = rectangleEffects[i];
                    bool onScreen = OnScreenSpace(param.space);
                    if (!onScreen)
                        if (!IsFrontOfCamera(param.position))
                            continue;
                    
                    float2 screenPoint = onScreen ? (float2) param.position : WorldToScreen(param.position);
                    param.softEdge = min(min(param.softEdge, param.width), param.height);
                    float it = InsideRect(screenPoint, uv, param.width, param.height);
                    float st = GetSoftEdge(screenPoint, uv, param.width, param.height, param.softEdge);
                    float t = it * st;
                    t *= clamp(param.alpha, 0, 1);
                    outputColor = lerp(outputColor, originalColor, t);
                }
                return outputColor;
            }

            fixed4 GetPixelatedRectangularColor(float2 uv, fixed4 inputColor, fixed4 originalColor)
            {
                fixed4 outputColor = inputColor;
                [unroll(20)]
                for (int i = 0; i < pixelatedRectangleEffectsCount; i++)
                {
                    PixelatedRectangleEffect param = pixelatedRectangleEffects[i];
                    bool onScreen = OnScreenSpace(param.space);
                    if (!onScreen)
                        if (!IsFrontOfCamera(param.position))
                            continue;
                    
                    float2 screenPoint = onScreen ? (float2) param.position : WorldToScreen(param.position);
                    param.softEdge = min(min(param.softEdge, param.width), param.height);
                    float it = InsideRect(screenPoint, uv, param.width, param.height);
                    float st = GetSoftEdge(screenPoint, uv, param.width, param.height, param.softEdge);
                    float t = it * st;
                    t *= clamp(param.alpha, 0, 1);
                    float2 pixelUV = GetRectangularPixelatedUV(uv, screenWidth, screenHeight, param.pixelSize);
                    originalColor = lerp(originalColor, tex2D(_MainTex, pixelUV), t);
                    outputColor = lerp(outputColor, originalColor, t);
                }
                return outputColor;
            }



            fixed4 GetCircularColor(float2 uv, fixed4 inputColor, fixed4 originalColor)
            {
                fixed4 outputColor = inputColor;
                [unroll(20)]
                for (int i = 0; i < circleEffectsCount; i++)
                {
                    CircleEffect param = circleEffects[i];
                    bool onScreen = OnScreenSpace(param.space);
                    if (!onScreen)
                        if (!IsFrontOfCamera(param.position))
                            continue;
        
                    float2 screenPoint = onScreen ? (float2) param.position : WorldToScreen(param.position);
                    float a = InCircle(uv, screenPoint, param.radius, param.softEdge);
                    a *= step(0, a);
                    a *= clamp(param.alpha, 0, 1);
                    outputColor = lerp(outputColor, originalColor, a);
                }
                return outputColor;
            }


            fixed4 GetPixelatedCircularColor(float2 uv, fixed4 inputColor, fixed4 originalColor)
            {
                fixed4 outputColor = inputColor;
                [unroll(20)]
                for (int i = 0; i < pixelatedCircleEffectsCount; i++)
                {
                    PixelatedCircleEffect param = pixelatedCircleEffects[i];
                    bool onScreen = OnScreenSpace(param.space);
                    if (!onScreen)
                        if (!IsFrontOfCamera(param.position))
                            continue;
        
                    float2 screenPoint = onScreen ? (float2) param.position : WorldToScreen(param.position);
                    float a = InCircle(uv, screenPoint, param.radius, param.softEdge);
                    a *= step(0, a);
                    a *= clamp(param.alpha, 0, 1);
                    float2 pixelUV = GetCircularPixelatedUV(uv, screenWidth, screenHeight, param.pixelSize);
                    originalColor = lerp(originalColor, tex2D(_MainTex, pixelUV), a);
                    outputColor = lerp(outputColor, originalColor, a);
                }
                return outputColor;
            }

            fixed4 GetRadialWaveColor(float2 uv, fixed4 inputColor)
            {
                fixed4 pixelColor = inputColor;
                [unroll(20)]
                for (int i = 0; i < radialWaveEffectsCount; i++)
                {
                    RadialWaveEffect param = radialWaveEffects[i];
                    bool onScreen = OnScreenSpace(param.space);
                    if (!onScreen)
                        if (!IsFrontOfCamera(param.position))
                            continue;
                    
                    float2 screenPoint = onScreen ? (float2) param.position : WorldToScreen(param.position);
                    float t = time * param.speed;
                    
                    float et = GetCircleEdgeShade(t, uv, screenPoint.xy, param.radius, param.thickness);
                    float at = GetFadeInAlpha(t) * step(0, et) * param.color.a;
                    float a = et * at * param.alpha;
                    pixelColor = lerp(pixelColor, param.color, a);
                }
                return pixelColor;
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
        
                return o;
            }


            fixed4 frag (v2f input) : SV_Target
            {
                fixed4 originalColor = tex2D(_MainTex, input.uv);
                float4 inputColor = lerp(originalColor, backgroundColor, backgroundColor.a);
                float2 screenPoint = input.uv * float2(screenWidth, screenHeight);
               
                 inputColor = GetRectangularColor(screenPoint, inputColor, originalColor);
                 inputColor = GetCircularColor(screenPoint, inputColor, originalColor);
                 inputColor = GetPixelatedRectangularColor(screenPoint, inputColor, originalColor);
                 inputColor = GetPixelatedCircularColor(screenPoint, inputColor, originalColor);
                 float4 outputColor = GetRadialWaveColor(screenPoint, inputColor);
                return outputColor;
            }
            ENDCG
        }
    }
}
