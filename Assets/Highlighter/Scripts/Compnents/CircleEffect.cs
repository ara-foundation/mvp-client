using UnityEngine;

namespace Highlighter
{
    public class CircleEffect : Highlightable, IEffectData
    {
        [Tooltip("Radius of the highlightable circle")]
        public uint radius = 400;
        [Tooltip("Circle edge softness")]
        public uint softEdge = 300;
    
        private EffectData.Circule parameter;

        protected virtual void OnEnable()
        {
            HighlighterManager.Add<EffectData.Circule>(this);
        }
        protected virtual void OnDisable()
        {
            HighlighterManager.Remove<EffectData.Circule>(this);
        }

        public virtual object GetData()
        {
            parameter.position = transform.position;
            parameter.alpha = alpha;
            parameter.radius = radius;
            parameter.softEdge = softEdge;
            parameter.space = GetCurrentSpace();
            return parameter;
        }
    }
}