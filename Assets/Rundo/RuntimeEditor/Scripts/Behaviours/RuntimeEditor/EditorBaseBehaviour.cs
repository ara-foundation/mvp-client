using Ara.RuntimeEditor;

namespace Rundo.RuntimeEditor.Behaviours
{
    /// <summary>
    /// Base behaviour class for editor specific behaviours - provides a shortcut to a runtime editor context.
    /// </summary>
    public class EditorBaseBehaviour : BaseBehaviour
    {
        private RuntimeEditorBehaviour _runtimeEditor;

        private static AraRuntimeEditor_manager _instance;

        public static AraRuntimeEditor_manager RuntimeEditor
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<AraRuntimeEditor_manager>();
                }
                return _instance;
            }
        }

        private AraRuntimeEditorScene_controller _runtimeEditorController;

        protected AraRuntimeEditorScene_controller RuntimeEditorController
        {
            get
            {
                if (_runtimeEditorController == null)
                    _runtimeEditorController = GetComponentInParent<AraRuntimeEditorScene_controller>();
                return _runtimeEditorController;
            }
        }
    }
}


