using System;
using Rundo.RuntimeEditor.Behaviours;
using Rundo.RuntimeEditor.Behaviours.UI;
using Rundo.RuntimeEditor.Data;

namespace Ara.RuntimeEditor
{
    /// <summary>
    /// Ara Runtime Editor UI handles the UI of the editor. The UI includes the windows for inspector,
    /// hierchy, tabs of various scenes in the original Rundo package and a UI for the control of the transform
    /// </summary>
    public class AraRuntimeEditor_ui : EditorBaseBehaviour
    {
        public InspectorWindowBehaviour InspectorWindow => GetComponentInChildren<InspectorWindowBehaviour>(true);
        public HierarchyWindowBehaviour HierarchyWindow => GetComponentInChildren<HierarchyWindowBehaviour>(true);
        public ProjectWindowBehaviour ProjectWindow => GetComponentInChildren<ProjectWindowBehaviour>(true);

        private void Start()
        {
            RegisterUiEvent<EditorUiBehaviour.ToggleWindowEvent>(OnToggleWindowEvent);
            RegisterUiEvent<EditorUiBehaviour.ShowTargetWindowEvent>(OnShowTargetWindowEvent);
            RegisterUiEvent<EditorUiBehaviour.SetHierarchyExpandedStateEvent>(OnSetHierarchyExpandedStateEvent).SetPriority(999);
            RegisterUiEvent<RuntimeEditorBehaviour.OnSceneSetToTabEvent>(Redraw);
            RegisterUiEvent<RuntimeEditorSceneControllerBehaviour.OnPlayModeChanged>(Redraw);
            Redraw();
        }

        private void Redraw()
        {
            if (RuntimeEditorController.IsEditorMode == false)
            {
                gameObject.SetActive(false);
                return;
            }
            
            gameObject.SetActive(true);
            
            //InspectorWindow.gameObject.SetActive(RuntimeEditorController.IsSceneLoaded);
            //HierarchyWindow.gameObject.SetActive(RuntimeEditorController.IsSceneLoaded);
            //ProjectWindow.gameObject.SetActive(RuntimeEditorController.IsSceneLoaded);
        }

        private void OnSetHierarchyExpandedStateEvent(EditorUiBehaviour.SetHierarchyExpandedStateEvent data)
        {
            var prefs = RuntimeEditorBehaviour.PersistentEditorPrefs.LoadData();
            
            if (data.IsExpanded)
            {
                if (prefs.ExpandedDataGameObjectsInHierarchyWindow.Contains(data.DataGameObjectId) == false)
                {
                    prefs.ExpandedDataGameObjectsInHierarchyWindow.Add(data.DataGameObjectId);
                    RuntimeEditorBehaviour.PersistentEditorPrefs.SaveData(prefs);
                }
            }
            else
            {
                if (prefs.ExpandedDataGameObjectsInHierarchyWindow.Contains(data.DataGameObjectId))
                {
                    prefs.ExpandedDataGameObjectsInHierarchyWindow.Remove(data.DataGameObjectId);
                    RuntimeEditorBehaviour.PersistentEditorPrefs.SaveData(prefs);
                }
            }
        }

        private void OnToggleWindowEvent(EditorUiBehaviour.ToggleWindowEvent data)
        {
            var window = GetComponentInChildren(data.Window, true);
            window.gameObject.SetActive(!window.gameObject.activeSelf);
        }

        private void OnShowTargetWindowEvent(EditorUiBehaviour.ShowTargetWindowEvent data)
        {
            var window = GetComponentInChildren<TargetViewBehaviour>(true);
            window.gameObject.SetActive(true);
            window.SetData(data.ObjectPickerBehaviour);
        }
    }
}


