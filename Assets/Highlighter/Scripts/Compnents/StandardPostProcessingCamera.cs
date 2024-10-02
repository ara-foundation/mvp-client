using UnityEngine;

namespace Highlighter
{
#if UNITY_EDITOR
    [ExecuteInEditMode]
#endif
    [RequireComponent(typeof(Camera))]

    public class StandardPostProcessingCamera : MonoBehaviour
    {
        private Camera cam;

        private void OnEnable()
        {
            if (cam == null)
                cam = GetComponent<Camera>();
        }


        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            HighlighterManager.UpdateData(cam);
            HighlighterManager.Render(source, destination);
        }
    }
}