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
using NBitcoin.Protocol;

namespace Ara.RuntimeEditor
{
    /// <summary>
    /// The root class of the runtime editor. Handles views (tabs) of scenes.
    /// </summary>
    public class AraRuntimeEditor_root : MonoBehaviour
    {
        [SerializeField] private GameObject RuntimeEditorController;
        
        /// <summary>
        /// Load the Given Scene on the List
        /// </summary>
        /// <param name="sceneMetaData"></param>
        /// <param name="_tabsContent"></param>
        /// <returns></returns>
        public AraRuntimeEditorScene_controller Load(DataSceneMetaData sceneMetaData, Transform _tabsContent)
        {
            var obj = Instantiate(RuntimeEditorController, _tabsContent);
            var instance = obj.GetComponent<AraRuntimeEditorScene_controller>();
            instance.LazyLoadScene(sceneMetaData);
            return instance;
        }

        /// <summary>
        /// Load the empty scene
        /// </summary>
        /// <param name="_tabsContent"></param>
        public AraRuntimeEditorScene_controller Load(Transform _tabsContent)
        {
            var obj = Instantiate(RuntimeEditorController, _tabsContent);
            var instance = obj.GetComponent<AraRuntimeEditorScene_controller>();
            return instance;
        }
    }
}



