using Ara.RuntimeEditor;
using Rundo.Core.Data;
using Rundo.RuntimeEditor.Data;
using UnityEngine;
using UnityEngine.UI;

namespace Rundo.RuntimeEditor.Behaviours
{
    public class RuntimeEditorTabButtonBehaviour : EditorBaseBehaviour
    {
        [SerializeField] private ButtonBehaviour _button;
        [SerializeField] private Button _close;

        private TGuid<RuntimeEditorBehaviour.TRuntimeEditorTab> _tabGuid;
        private TGuid<DataScene.TDataSceneId> _sceneGuid;
        
        private void Start()
        {
            _button.OnClick(OnClick);
            _close.onClick.AddListener(() =>
            {
                AraRuntimeEditor_manager.Instance.CloseTab(_tabGuid);
            });
            RegisterCommandListener<DataScene>(OnDataScene);
            RegisterUiEvent<RuntimeEditorBehaviour.OnTabSelectedEvent>(Redraw);
            RegisterUiEvent<RuntimeEditorBehaviour.OnSceneSetToTabEvent>(Redraw);
            Redraw();
        }

        private void OnDataScene(DataScene dataScene)
        {
            if (dataScene.DataSceneMetaData.Guid == AraRuntimeEditor_manager.Instance.GetSceneId(_tabGuid))
                Redraw();
        }
        
        private void Redraw()
        {
            _button.Label = "(empty)";
            _button.Unselect();
            
            foreach (var it in AraRuntimeEditor_manager.Instance.InstantiatedTabs)
                if (it.TabGuid == _tabGuid)
                {
                    if (it.IsSceneLoaded)
                        _button.Label = it.DataSceneMetaData.Name;
                    break;
                }
            
            if (AraRuntimeEditor_manager.Instance.SelectedTab == _tabGuid)
                _button.Select();
        }

        public void SetData(TGuid<RuntimeEditorBehaviour.TRuntimeEditorTab> tabGuid)
        {
            _tabGuid = tabGuid;
            Redraw();
        }

        private void OnClick()
        {
            AraRuntimeEditor_manager.Instance.SelectTab(_tabGuid);
        }

    }
}


