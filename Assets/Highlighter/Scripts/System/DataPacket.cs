using System.Collections.Generic;

namespace Highlighter
{
    public class DataPacket
    {
        private List<IEffectData> componentsData = new List<IEffectData>();

        private uint buffer = 0;

        public DataPacket()
        {
            componentsData = new List<IEffectData>();
        }

        public bool isEmpty => componentsData.Count == 0;

        #region Data add and removing
        /// <summary>
        /// Add new component effect data
        /// </summary>
        /// <typeparam name="T">Component effect data type</typeparam>
        /// <param name="effectData">Component effect data</param>
        /// <returns>Return true if given data added successfully</returns>
        public bool Add<T>(IEffectData effectData) where T : struct
        {
            if (componentsData.Contains(effectData))
                return false;

            componentsData.Add(effectData);
            UpdatePacketSize<T>();
            return true;
        }

        /// <summary>
        /// Remove existing component effect data from the data layer
        /// </summary>
        /// <typeparam name="T">Component effect data type</typeparam>
        /// <param name="effectData">Component effect data</param>
        /// <returns>Return true if given effect data remove successfully</returns>
        public bool Remove<T>(IEffectData effectData) where T : struct
        {
            if(componentsData.Contains(effectData))
                if (componentsData.Remove(effectData))
                { 
                    UpdatePacketSize<T>();
                    return true;
                }

            return false;
        }

        /// <summary>
        /// Resizing buffer size
        /// </summary>
        /// <typeparam name="T">Component effect data type</typeparam>
        private void UpdatePacketSize<T>() where T : struct
        {
            if (isEmpty)
            { 
                Buffer.ReleaseBuffer(ref buffer);
                return;
            }

            int count = componentsData.Count;
            int stride = EffectData.GetStrideOf<T>();
            if (buffer <= 0)
                buffer = Buffer.CreateBuffer(count, stride, EffectData.GetBufferIDOf<T>(), EffectData.GetCountIDOf<T>());

            if (Buffer.BindBuffer(buffer))
                Buffer.ResizeBufferMemory(count, stride);
        }
        #endregion Data add and removing
        

        /// <summary>
        /// Update the buffer data in GPU memory
        /// </summary>
        public void UpdateBuffer()
        {
            if(Buffer.BindBuffer(buffer))
                for (int i = 0; i < componentsData.Count; i++)
                    Buffer.WriteDataAt(i, componentsData[i].GetData());
        }
    }
}
