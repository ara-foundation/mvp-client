using System.Collections.Generic;
using UnityEngine.Rendering;
using UnityEngine;

namespace Highlighter
{
    public sealed class Graphics 
    {
        private static Material material = null;

        private static void Init() => material = new Material(Shader.Find("Highlighter/Main"));
        public static bool IsInitialized => material != null;

        public static void Render(RenderTexture input, RenderTexture output)
        {
            UnityEngine.Graphics.Blit(input, output, material, 0);
        }
        public static void Render(CommandBuffer cmd, RenderTargetIdentifier input, ref RenderTargetIdentifier output)
        {
            cmd.Blit(input, output, material, 0);
        }

        public static void BindDataToRender()
        {
            if(!IsInitialized)
                Init();

            // pushing all of the buffer data in GPU memory
            var bufferMap = Buffer.GetBufferMap as Dictionary<uint, Buffer>;
            foreach (var buffer in bufferMap)
                SetBuffer(buffer.Value);
        }

        public static void SetUniform(string key, int value)
        {
            if (!IsInitialized)
                Init();

            material.SetInt(key, value);
        }
        public static void SetUniform(string key, float value)
        {
            if (!IsInitialized)
                Init();

            material.SetFloat(key, value);
        }   
        
        public static void SetUniform(string key, Color value)
        {
            if (!IsInitialized)
                Init();

            material.SetColor(key, value);
        }
        public static void SetUniform(string key, Matrix4x4 value)
        {
            if (!IsInitialized)
                Init();

            material.SetMatrix(key, value);
        }

        public static void SetBuffer(Buffer buffer)
        {
            if (!IsInitialized)
                Init();

            if (buffer == null)
                return;
            
            int elementCount = buffer.GetCount;
            material.SetInt(buffer.GetCountID, elementCount);
            material.SetBuffer(buffer.GetBufferID, buffer.GetBuffer() as ComputeBuffer);
        }
    }
}
