using UnityEngine;

namespace Highlighter
{
    public class PixelatedCircleEffect : Highlightable, IEffectData
    {
        public uint pixelSize = 15;
        public uint radius = 400;
        public uint softEdge = 300;

        private EffectData.PixelatedCircle parameter;

        protected virtual void OnEnable()
        {
            HighlighterManager.Add<EffectData.PixelatedCircle>(this);
        }
        protected virtual void OnDisable()
        {
            HighlighterManager.Remove<EffectData.PixelatedCircle>(this);
        }

        public virtual object GetData()
        {
            parameter.position = transform.position;
            parameter.alpha = alpha;
            parameter.pixelSize = pixelSize;
            parameter.radius = radius;
            parameter.softEdge = softEdge;
            parameter.space = GetCurrentSpace();
            return parameter;
        }
    }
}