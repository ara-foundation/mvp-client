using Rundo.RuntimeEditor.Behaviours.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectedItem : MonoBehaviour
{
    [SerializeField] private Texture2D DefaultTexture;
    [SerializeField] private TMP_Text _prefabName;
    [SerializeField] private RawImage _icon;
    [SerializeField] private Button _button;

    public ProjectItemMetaData Data;

    private void Awake()
    {
        Data = new ProjectItemMetaData();
    }

    public void Hide()
    {
        Data.GameObject = null;
        gameObject.SetActive(false);
    }

    public bool IsShowing()
    {
        return gameObject.activeSelf && Data.GameObject != null;
    }

    public void ShowDefault()
    {
        Data.GameObject = null;
        _prefabName.text = "No data selected";
        _icon.texture = DefaultTexture;
        _button.gameObject.SetActive(false);
    }

    public void Show(ProjectItemMetaData projectItemMetaData, bool btnEnabled)
    {
        Show(projectItemMetaData);
        _button.gameObject.SetActive(btnEnabled);
    } 

    public string DIOSTypeString()
    {
        if (IsShowing())
        {
            return $"{Data.DiosType[0]}";
        }
        return null;
    }

    public int DIOSTypeInt()
    {
        if (IsShowing())
        {
            return (int)Data.DiosType[0];
        }
        return (int)DIOSData.Type.NoData;
    }

    public void Show(ProjectItemMetaData projectItemMetaData)
    {
        _button.gameObject.SetActive(true);
        Data = projectItemMetaData;
        _icon.texture = Data.Screenshot;

        if (Data.GameObject != null)
        {
            if (projectItemMetaData.DiosType != null && projectItemMetaData.DiosType.Count > 0)
            {
                _prefabName.text = DIOSData.TypeAndName(projectItemMetaData.GameObject, projectItemMetaData.DiosType[0]);
            }
            else
            {
                _prefabName.text = Data.GameObject.name;
            }
        }
        else
            _prefabName.text = Data.FolderName;

        gameObject.SetActive(true);
    }
}
