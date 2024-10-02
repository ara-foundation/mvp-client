using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace Highlighter
{
#if UNITY_EDITOR
    [ExecuteAlways]        
    #endif
    public sealed class HighlighterManager : MonoBehaviour
    {
        [SerializeField] private Color m_backgroundColor = new Color(0, 0, 1f, 0.6f);

        private static HighlighterManager instance = null;
        private static Dictionary<Type, DataPacket> dataMap = new Dictionary<Type, DataPacket>();
        [SerializeField] private static Color backgroundColor = new Color(0, 0, 1f, 0.6f);

        /// <summary>
        /// Backgournd fill color of the game view
        /// </summary>
        public static Color BackgroundColor
        {
            get => instance ? backgroundColor = instance.m_backgroundColor : backgroundColor;

            set
            {
                backgroundColor = value;
                if (instance)
                    instance.m_backgroundColor = value;
            }
        }
        private void OnEnable()
        {
            if (instance == null)
            { 
                instance = this;
                if(Application.isPlaying)
                    DontDestroyOnLoad(gameObject);
            }
#if UNITY_EDITOR
            else if(instance != this)
                Debug.LogWarning("Multiple instance found\n Only one instance of" + GetType().Name +" is allowed in the scene");
#endif
        }

        private void OnDisable()
        {
            if (instance == this)
                instance = null;
        }

        /// <summary>
        /// Add new component effect data to the data layer
        /// </summary>
        /// <typeparam name="T">Component effect data type</typeparam>
        /// <param name="effectData">Component effect data</param>
        /// <returns>Return true if given effect data added successfully</returns>
        public static bool Add<T>(IEffectData effectData) where T : struct
        { 
            var type = typeof(T);
            if(!dataMap.ContainsKey(type))
                dataMap.Add(type, new DataPacket());

            return  dataMap[type].Add<T>(effectData);
        }

        /// <summary>
        /// Remove existing component effect data from the data layer
        /// </summary>
        /// <typeparam name="T">Component effect data type</typeparam>
        /// <param name="effectData">Component effect data</param>
        /// <returns>Return true if given effect data remove successfully</returns>
        public static bool Remove<T>(IEffectData effectData) where T : struct
        {
            var type = typeof(T);
            if (!dataMap.ContainsKey(type))
                return false;

            var dataPacket = dataMap[type];
            if (dataPacket.Remove<T>(effectData))
            {
                if (dataMap[type].isEmpty)
                    dataMap.Remove(type);

                return true;
            }

            return false;
        }

  
        /// <summary>
        /// Update all of the component effect data to render perfectly with the updated data
        /// </summary>
        /// <param name="camera"></param>
        /// <param name="backgroundColor"></param>
        public static void UpdateData(Camera camera)
        {
            if (dataMap == null)
                return;

            Graphics.SetUniform("screenWidth", camera.pixelWidth);
            Graphics.SetUniform("screenHeight", camera.pixelHeight);
            Graphics.SetUniform("worldToCameraMatrix", camera.worldToCameraMatrix);
            Graphics.SetUniform("projectionMatrix", camera.projectionMatrix);
            Graphics.SetUniform("backgroundColor", BackgroundColor);
            Graphics.SetUniform("time", Time.time);

            foreach (var dataPacket in dataMap)
                dataPacket.Value.UpdateBuffer();

            Graphics.BindDataToRender();
        }

        /// <summary>
        /// Render the actual effect on the Render image
        /// </summary>
        /// <param name="input">input camera render image</param>
        /// <param name="output">final output render image of Highlighter</param>
        public static void Render(RenderTexture input, RenderTexture output) => Graphics.Render(input, output);

        /// <summary>
        /// Render the actual effect
        /// </summary>
        /// <param name="cmd">Input render texture buffer</param>
        /// <param name="input">Input Render Target Identifier</param>
        /// <param name="output">Output Render Target Identifier</param>
        public static void Render(CommandBuffer cmd, RenderTargetIdentifier input, ref RenderTargetIdentifier output) => Graphics.Render(cmd,input, ref output);
    }
}