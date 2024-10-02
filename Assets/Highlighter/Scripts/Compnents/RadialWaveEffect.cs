using UnityEngine;

namespace Highlighter
{
    public class RadialWaveEffect : Highlightable, IEffectData
    {
        [ColorUsage(true, true)]
        public Color color = new Color(1,0,0,1);
        public uint radius = 400;
        public uint thickness = 100;
        public float speed = 1;

        private EffectData.RadialWave parameter;

        protected virtual void OnEnable()
        {
            HighlighterManager.Add<EffectData.RadialWave>(this);
        }
        protected virtual void OnDisable()
        {
            HighlighterManager.Remove<EffectData.RadialWave>(this);
        }
        public virtual object GetData()
        {
            parameter.position = transform.position;
            parameter.alpha = alpha;
            parameter.color = color;
            parameter.radius = radius;
            parameter.thickness = thickness;
            parameter.speed = speed;
            parameter.space = GetCurrentSpace();
            return parameter;
        }
    }
}
