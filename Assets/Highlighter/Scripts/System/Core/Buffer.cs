using System;
using UnityEngine;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Object = System.Object;
namespace Highlighter
{
    public sealed class Buffer
    {
        private string bufferID;
        private string countID;
        private ComputeBuffer buffer;
        private byte[] memory;
        private GCHandle handle;
        private IntPtr baseAddress;

        private static uint bufferCount = 0;
        private static Buffer bindedBuffer;
        private static Dictionary<uint, Buffer> bufferMap = new Dictionary<uint, Buffer>();
        public static System.Object GetBufferMap => bufferMap;


        public string GetCountID => countID;
        public string GetBufferID => bufferID;
        public int GetCount
        {
            get
            {
                try { return buffer.count; }
                catch { return 0; }
            }
        }
        public int GetStride
        {
            get
            {
                try { return buffer.stride; }
                catch { return 0; }
            }
        }

        private Buffer(int count, int stride, string bufferID, string countID)
        {
            this.bufferID = bufferID;
            this.countID = countID;
            buffer = new ComputeBuffer(count, stride);
            memory = new byte[count * stride];
            handle = GCHandle.Alloc(memory, GCHandleType.Pinned);
            baseAddress = handle.AddrOfPinnedObject();
        }

        private void ResizeMemory(int count, int stride)
        {
            try
            {
                if (buffer.count == count && buffer.stride == stride)
                    return;
            }catch { }

            ReleaseData();
            if (count <= 0) return;

            buffer = new ComputeBuffer(count, stride);
            memory = new byte[count * stride];
            handle = GCHandle.Alloc(memory, GCHandleType.Pinned);
            baseAddress = handle.AddrOfPinnedObject();
        }

        private bool _WriteDataAt(int index, Object dataChunk, bool deleteOld = false)
        {
           int offsetAddress =  index * buffer.stride;
            if (offsetAddress > buffer.count * buffer.stride)
                return false;

            Marshal.StructureToPtr(dataChunk, baseAddress + offsetAddress, deleteOld);
            return true;
        }

        public Object GetBuffer()
        {
            buffer.SetData(memory);
            return buffer;
        }
        

        private void ReleaseData()
        {
            if(buffer != null)
                buffer.Release();
            if (handle.IsAllocated)
                handle.Free();
            memory = null;
        }


        public static uint CreateBuffer(int count, int stride, string bufferID, string countID)
        {
            bufferCount++;
            bindedBuffer = new Buffer(count, stride, bufferID, countID);
            bufferMap.Add(bufferCount, bindedBuffer);
            return bufferCount;
        }
        public static bool BindBuffer(uint buffer)
        { 
            if(!bufferMap.ContainsKey(buffer))
                return false;

            bindedBuffer = bufferMap[buffer];
            return true;
        }
        public static bool ResizeBufferMemory(int count, int stride)
        {
            if (bindedBuffer == null) return false;

            bindedBuffer.ResizeMemory(count, stride);
            return true;
        }


        public static bool WriteDataAt(int index, Object dataChunk)
        {
            if (bindedBuffer == null)
                return false;

            return bindedBuffer._WriteDataAt(index, dataChunk);
        }
        public static bool SetBuffferData(Object[] data)
        {
            if (bindedBuffer == null)
                return false;

            bool success = true;
            for (int i = 0; i < data.Length; i++)
                success &= bindedBuffer._WriteDataAt(i, data[i]);

            return success;
        }
   
        public static bool ReleaseBuffer(ref uint buffer)
        {
            if (!bufferMap.ContainsKey(buffer))
                return false;

            var bufferObj = bufferMap[buffer];
            if (bufferObj != null)
            {
                bufferObj.ReleaseData();
                bufferObj.countID = string.Empty;
                bufferObj.bufferID = string.Empty;
            }
            bufferMap.Remove(buffer);
            buffer = 0;
            return true;
        }
    }
}