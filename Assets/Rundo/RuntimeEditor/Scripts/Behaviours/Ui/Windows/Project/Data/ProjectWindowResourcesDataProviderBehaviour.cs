using Ara.RuntimeEditor;
using System.Collections.Generic;

namespace Rundo.RuntimeEditor.Behaviours.UI
{
    public class ProjectWindowResourcesDataProviderBehaviour : ProjectWindowBaseDataProviderBehaviour
    {
        public override List<ProjectItemMetaData> GetData()
        {
            var res = new List<ProjectItemMetaData>();
            
            foreach (var prefab in AraRuntimeEditor_manager.Instance.GetPrefabs())
                if (prefab.HideInPrefabWindow == false)
                    res.Add(new ProjectItemMetaData{GameObject = prefab.gameObject});

            return res;
        }
    }
}
