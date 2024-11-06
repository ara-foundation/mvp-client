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

    public ProjectItemMetaData Data;
        
    private void Start()
    {
        _button.onClick.AddListener(() =>
        {
            if (Data.GameObject != null)
            {
                OnSelectCallback?.Invoke(Data);
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
        Data = projectItemMetaData;

        if (Data.GameObject != null)
        {
            if (projectItemMetaData.DiosType != null && projectItemMetaData.DiosType.Count > 0)
            {
                _prefabName.text = DIOSData.TypeAndName(projectItemMetaData.GameObject, projectItemMetaData.DiosType[0]);
            } else
            {
                _prefabName.text = Data.GameObject.name;
            }
        }
        else
            _prefabName.text = Data.FolderName;
    }

    public void UpdateScreenshot(Texture2D screenshot)
    {
        _icon.texture = screenshot;
        Data.Screenshot = screenshot;
    }
}
