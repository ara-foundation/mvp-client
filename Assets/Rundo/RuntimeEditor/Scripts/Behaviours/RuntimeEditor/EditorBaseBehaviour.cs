using Ara.RuntimeEditor;

namespace Rundo.RuntimeEditor.Behaviours
{
    /// <summary>
    /// Base behaviour class for editor specific behaviours - provides a shortcut to a runtime editor context.
    /// </summary>
    public class EditorBaseBehaviour : BaseBehaviour
    {
        protected AraRuntimeEditorScene_controller RuntimeEditorController
        {
            get
            {
                return AraRuntimeEditor_manager.Instance.GetActiveTab();
            }
        }
    }
}


