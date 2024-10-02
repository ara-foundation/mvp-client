using UnityEngine;

namespace Highlighter
{
    public class PixelatedRectangleEffect : Highlightable, IEffectData
    {
        public uint pixelSize = 15;
        public uint width = 300;
        public uint height = 300;
        public uint softEdge = 80;

        private EffectData.PixelatedRectangle parameter;

        protected virtual void OnEnable()
        {
            HighlighterManager.Add<EffectData.PixelatedRectangle>(this);
        }
        protected virtual void OnDisable()
        {
            HighlighterManager.Remove<EffectData.PixelatedRectangle>(this);
        }
        public virtual object GetData()
        {
            parameter.position = transform.position;
            parameter.alpha = alpha;
            parameter.pixelSize = pixelSize;
            parameter.width = width;
            parameter.height = height;
            parameter.softEdge = softEdge;
            parameter.space = GetCurrentSpace();
            return parameter;
        }
    }
}
