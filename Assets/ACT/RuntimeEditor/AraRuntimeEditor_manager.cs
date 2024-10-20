using System.Collections.Generic;
using Rundo.Core.Data;
using Rundo.RuntimeEditor.Data;
using Rundo.Core.Utils;
using Rundo.RuntimeEditor.Tools;
using Rundo.Ui;
using UnityEngine;
using Rundo;
using Rundo.RuntimeEditor.Behaviours;
using static Rundo.RuntimeEditor.Behaviours.RuntimeEditorBehaviour;
using Rundo.RuntimeEditor.Commands;

namespace Ara.RuntimeEditor
{
    /// <summary>
    /// Manage multiple scenes. It must be one across all ara scenes. Load and unload the level scenes
    /// </summary>
    public class AraRuntimeEditor_manager : MonoBehaviour
    {
        public static PersistentDataSet<DataSceneMetaData, DataScene> PersistentDataScenes = new("RuntimeEditorScenes");
        public static PersistentData<RuntimeEditorPrefs> PersistentEditorPrefs = new("RuntimeEditorPrefs", new RuntimeEditorPrefs());
        
        [SerializeField] private AraRuntimeEditor_root RuntimeEditorRoot;
        [SerializeField] private Transform _tabsContent;
        [SerializeField] private List<string> _prefabsResourcesPaths = new();
        [SerializeField] private PrefabScreenshoterBehaviour _prefabScreenshoterBehaviour;
        
        public TGuid<TRuntimeEditorTab> SelectedTab { get; private set; }
        
        /// <summary>
        /// Is true when the input is over the world, is false when the input is over the UI
        /// </summary>
        public static bool IsInputOverWorld { get; set; }
        
        public PrefabScreenshoterBehaviour PrefabScreenshoterBehaviour => _prefabScreenshoterBehaviour;
        
        private List<PrefabIdBehaviour> _prefabs;
        
        public readonly List<AraRuntimeEditorScene_controller> InstantiatedTabs = new();

        private static AraRuntimeEditor_manager _instance;

        public static AraRuntimeEditor_manager Instance
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

        [Header("Customize to adjust into the Ara client. Each scene will be a one tab")]
        public bool showOneTab = false;

        private void Start()
        {
            RundoEngine.DataSerializer.AddDefaultReadConverter(new DataComponentReadJsonConverter());

            var sceneMetaDatas = PersistentDataScenes.LoadData();

            // load scenes
            foreach (var it in PersistentEditorPrefs.LoadData().OpenedScenes)
            {
                if (showOneTab && InstantiatedTabs.Count == 1)
                {
                    break;
                }
                foreach (var sceneMetaData in sceneMetaDatas)
                {
                    if (showOneTab && InstantiatedTabs.Count == 1)
                    {
                        break;
                    }
                    if (sceneMetaData.Guid.ToStringRawValue() == it)
                    {
                        var instance = RuntimeEditorRoot.Load(sceneMetaData, _tabsContent);
                        InstantiatedTabs.Add(instance);
                        
                    }
                }
            }

            if (InstantiatedTabs.Count == 0)
                AddTab();
            
            SelectTab(InstantiatedTabs[0].TabGuid);
        }

        public void SelectTab(TGuid<TRuntimeEditorTab> tabGuid)
        {
            SelectedTab = tabGuid;
            
            foreach (var tab in InstantiatedTabs)
                tab.gameObject.SetActive(tab.TabGuid == SelectedTab);
            
            DispatchUiEventToAllSceneControllers(new OnTabSelectedEvent());
        }

        public AraRuntimeEditorScene_controller GetActiveTab()
        {
            foreach (var tab in InstantiatedTabs)
            {
                if (tab.gameObject.activeSelf) return tab;
            }
            return null;
        }

        /// <summary>
        /// Adds an empty tab
        /// </summary>
        public void AddTab()
        {
            var tabInstance = RuntimeEditorRoot.Load(_tabsContent);
            InstantiatedTabs.Add(tabInstance);
            SelectTab(tabInstance.TabGuid);
            DispatchUiEventToAllSceneControllers(new OnTabAddedEvent());
        }

        public void CloseTab(TGuid<TRuntimeEditorTab> tabGuid)
        {
            foreach (var tab in InstantiatedTabs)
            {
                if (tab.TabGuid == tabGuid)
                {
                    InstantiatedTabs.Remove(tab);

                    if (tab.TabGuid == SelectedTab && InstantiatedTabs.Count > 0)
                        SelectTab(InstantiatedTabs[0].TabGuid);
                    else
                        SelectedTab = default;

                    Destroy(tab.gameObject);
                    break;
                }
            }

            DispatchUiEventToAllSceneControllers(new OnTabRemovedEvent());

            if (InstantiatedTabs.Count == 0)
            {
                AddTab();
                SelectTab(InstantiatedTabs[0].TabGuid);
            }
        }

        public PrefabIdBehaviour GetPrefab(TGuid<TPrefabId> prefabId)
        {
            foreach (var prefab in _prefabs)
                if (prefab.Guid == prefabId)
                    return prefab;

            return null;
        }

        public List<PrefabIdBehaviour> GetPrefabs()
        {
            if (_prefabs != null)
                return _prefabs;
            
            _prefabs = new List<PrefabIdBehaviour>();
            foreach (var path in _prefabsResourcesPaths)
                foreach (var prefab in Resources.LoadAll<PrefabIdBehaviour>(path))
                    _prefabs.Add(prefab);

            return _prefabs;
        }

        public void DispatchUiEventToAllSceneControllers(IUiEvent data)
        {
            foreach (var it in InstantiatedTabs)
                it.GetComponentInChildren<AraRuntimeEditorScene_controller>().UiEvents.Dispatch(data);
        }

        public TGuid<DataScene.TDataSceneId> GetSceneId(TGuid<TRuntimeEditorTab> tabId)
        {
            foreach (var it in InstantiatedTabs)
                if (it.TabGuid == tabId && it.IsSceneLoaded)
                    return it.DataScene.DataSceneMetaData.Guid;
            return default;
        }
    }
}



