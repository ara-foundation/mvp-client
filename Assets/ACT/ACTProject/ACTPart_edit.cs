using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ACTPart_edit : MonoBehaviour
{
    [Header("Project Name")]
    [SerializeField] private Transform ProjectNameContainer;
    [SerializeField] private TextMeshProUGUI ProjectNameLabel;
    [SerializeField] private ActivityState ProjectNameState;
    [SerializeField] private GameObject ProjectNameFieldContainer;

    private string _projectName = "";

    // Start is called before the first frame update
    void Start()
    {
        ToggleProjectNameEditing(edit: false);
    }

    void ToggleProjectNameEditing(bool edit)
    {
        ProjectNameState.gameObject.SetActive(!edit);
        ProjectNameFieldContainer.SetActive(edit);
    }

    public void OnEditProjectName(bool focused)
    {
        if (!focused)
        {
            return;
        }
        ProjectNameState.ChangeMode(StateMode.None);
        ToggleProjectNameEditing(edit: true);
        CameraFocus.Instance.SelectTargetThrough(ProjectNameContainer, enabled: true);
    }

    private void ShowProjectName(string name)
    {
        ProjectNameLabel.text = name;
        ToggleProjectNameEditing(edit: false);
    }

    public void OnProjectNameEditSelect(string name)
    {
        _projectName = name;
    }

    /// <summary>
    /// Escaped, nothing is changed
    /// </summary>
    /// <param name="name"></param>
    public void OnProjectNameEditEnd(string name)
    {
        CameraFocus.Instance.SelectTargetThrough(ProjectNameContainer, enabled: false);

        if (!EventSystem.current.alreadySelecting) EventSystem.current.SetSelectedGameObject(null);

        if (Input.GetKeyDown(KeyCode.Escape) || _projectName.Equals(name) || string.IsNullOrEmpty(name))
        {
            ShowProjectName(ProjectNameLabel.text);
        } else
        {
            ShowProjectName(name);
            // TODO call back the ara tutorial to start showing next part
            Debug.Log("Submit the data (1) check is text changed, (2) call submit to save data in the server");
        }
    }
}
