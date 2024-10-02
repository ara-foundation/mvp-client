using UnityEngine;

namespace Highlighter
{
#if UNITY_EDITOR
    [ExecuteInEditMode]
#endif
    public abstract class Highlightable : MonoBehaviour
    {
        [Range(0f, 1f)]
        public float alpha = 1.0f;

        private Transform selfTransform = null;
        private RectTransform selfRectTransform = null;
        private Transform parent = null;
        private Canvas canvas = null;
        private RenderMode canvasRenderMode;
        private Camera worldCamera;


        protected virtual void Awake()
        {
            Init();
        }

        private void Init()
        {
            selfTransform = transform;
            selfRectTransform = transform as RectTransform;
            parent = transform.parent;
            canvas = GetComponentInParent<Canvas>();
            if (canvas)
            {
                canvasRenderMode = canvas.renderMode;
                worldCamera = canvas.worldCamera;
            }
        }
        private bool HasStatusChanged()
        {
            if (parent != transform.parent)
                return true;
            else if (canvas)
            {
                if (canvas.renderMode != canvasRenderMode)
                    return true;
                else if (canvas.worldCamera != worldCamera)
                    return true;

            }
            else if (selfTransform != transform)
                return true;

            return false;
        }

        private bool IsOnUISpace()
        {
            if (HasStatusChanged())
                Init();
            if (selfRectTransform == null)
                return false;
            if (canvas == null)
                return false;
            return canvas.renderMode == RenderMode.ScreenSpaceOverlay || (canvas.renderMode == RenderMode.ScreenSpaceCamera && canvas.worldCamera == null);
        }

        protected uint GetCurrentSpace()
        {
            if (IsOnUISpace())
                return 1;
            return 0;
        }
    }
}