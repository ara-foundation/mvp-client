using UnityEngine;

namespace Highlighter
{
    public sealed class EffectData
    {
        public struct Rectangle
        {
            public Vector3 position;
            public float alpha;
            public uint width;
            public uint height;
            public uint softEdge;
            public uint space;
        }


        public struct Circule
        {
            public Vector3 position;
            public float alpha;
            public uint radius;
            public uint softEdge;
            public uint space;
        }

        public struct RadialWave
        {
            public Vector3 position;
            public float alpha;
            public Color color;
            public uint radius;
            public uint thickness;
            public float speed;
            public uint space;
        }

        public struct PixelatedCircle
        {
            public Vector3 position;
            public float alpha;
            public uint pixelSize;
            public uint radius;
            public uint softEdge;
            public uint space;
        }

        public struct PixelatedRectangle
        {
            public Vector3 position;
            public float alpha;
            public uint pixelSize;
            public uint width;
            public uint height;
            public uint softEdge;
            public uint space;
        }

        public static int GetStrideOf<T>() where T : struct
        {
            if (typeof(T) == typeof(Circule))
                return 4 * sizeof(float) + 3 * sizeof(uint);
            else if (typeof(T) == typeof(RadialWave))
                return 9 * sizeof(float) + 3 * sizeof(uint);   
            else if (typeof(T) == typeof(Rectangle))
                return 4 * sizeof(float) + 4 * sizeof(uint); 
            else if (typeof(T) == typeof(PixelatedCircle))
                return 4 * sizeof(float) + 4 * sizeof(uint); 
            else if (typeof(T) == typeof(PixelatedRectangle))
                return 4 * sizeof(float) + 5 * sizeof(uint);
            else
                return 0;
        }

        public static string GetBufferIDOf<T>() where T : struct
        {
            if (typeof(T) == typeof(Circule))
                return "circleEffects";
            else if (typeof(T) == typeof(RadialWave))
                return "radialWaveEffects";
            else if (typeof(T) == typeof(Rectangle))
                return "rectangleEffects";
            else if (typeof(T) == typeof(PixelatedCircle))
                return "pixelatedCircleEffects";   
            else if (typeof(T) == typeof(PixelatedRectangle))
                return "pixelatedRectangleEffects";
            else
                return "";
        }
        public static string GetCountIDOf<T>() where T : struct
        {
            if (typeof(T) == typeof(Circule))
                return "circleEffectsCount";
            else if (typeof(T) == typeof(RadialWave))
                return "radialWaveEffectsCount";
            else if (typeof(T) == typeof(Rectangle))
                return "rectangleEffectsCount";
            else if (typeof(T) == typeof(PixelatedCircle))
                return "pixelatedCircleEffectsCount";  
            else if (typeof(T) == typeof(PixelatedRectangle))
                return "pixelatedRectangleEffectsCount";
            else
                return "";
        }
    }
}
