using UnityEngine;

namespace Highlighter
{
    public class RectangleEffect : Highlightable, IEffectData
    {
        public uint width = 300;
        public uint height = 300;
        public uint softEdge = 80;

        private EffectData.Rectangle parameter;

        protected virtual void OnEnable()
        {
            HighlighterManager.Add<EffectData.Rectangle>(this);
        }
        protected virtual void OnDisable()
        {
            HighlighterManager.Remove<EffectData.Rectangle>(this);
        }
        public virtual object GetData()
        {
            parameter.position = transform.position;
            parameter.alpha = alpha;
            parameter.width = width;
            parameter.height = height;
            parameter.softEdge = softEdge;
            parameter.space = GetCurrentSpace();
            return parameter;
        }
    }
}
