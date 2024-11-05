using Rundo.RuntimeEditor.Behaviours;
using Rundo.RuntimeEditor.Behaviours.UI;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectionWindowItem : EditorBaseBehaviour
{
    [SerializeField] private TMP_Text _prefabName;
    [SerializeField] private Button _button;
    [SerializeField] private RawImage _icon;

    private Action<ProjectItemMetaData> OnSelectCallback;

    public ProjectItemMetaData ProjectItemMetaData { get; private set; }
        
    private void Start()
    {
        _button.onClick.AddListener(() =>
        {
            if (ProjectItemMetaData.GameObject != null)
            {
                OnSelectCallback?.Invoke(ProjectItemMetaData);
                    /*if (ProjectItemMetaData.GameObject.TryGetComponent<PrefabIdBehaviour>(out var prefabIdBehaviour))
                    {
                        var dataGameObject = DataScene.InstantiateDataGameObjectFromPrefab(prefabIdBehaviour);

                        dataGameObject.GetComponent<DataGameObjectBehaviour>().DataComponentPrefab.OverridePrefabComponent = true;
                        dataGameObject.GetComponent<DataTransformBehaviour>().DataComponentPrefab.OverridePrefabComponent = true;

                        RuntimeEditorController.SetMode<PlaceObjectsEditorModeBehaviour>().SetData(dataGameObject);
                    }*/
            }
        });
    }

    public void SetData(ProjectItemMetaData projectItemMetaData, Action<ProjectItemMetaData> OnSelectCallback)
    {
        this.OnSelectCallback = OnSelectCallback;
        ProjectItemMetaData = projectItemMetaData;

        if (ProjectItemMetaData.GameObject != null)
        {
            if (projectItemMetaData.DiosType != null && projectItemMetaData.DiosType.Count > 0)
            {
                _prefabName.text = DIOSData.TypeAndName(projectItemMetaData.GameObject, projectItemMetaData.DiosType[0]);
            } else
            {
                _prefabName.text = ProjectItemMetaData.GameObject.name;
            }
        }
        else
            _prefabName.text = ProjectItemMetaData.FolderName;
    }

    public void UpdateScreenshot(Texture2D screenshot)
    {
        _icon.texture = screenshot;
    }
}
