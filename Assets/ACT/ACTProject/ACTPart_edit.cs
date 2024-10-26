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

    // Start is called before the first frame update
    void Start()
    {
        // Show a project name label, hide the project name editing field.
        ToggleProjectNameEditing(edit: false);
    }

    #region ProjectName

    /// <summary>
    /// Switch between editing the project name and showing it as a label
    /// </summary>
    /// <param name="edit"></param>
    void ToggleProjectNameEditing(bool edit)
    {
        ProjectNameState.gameObject.SetActive(!edit);
        ProjectNameFieldContainer.SetActive(edit);
    }

    /// <summary>
    /// Invoked from Scene. By original design when a user clicks twice on project name label
    /// </summary>
    /// <param name="focused"></param>
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

    /// <summary>
    /// Escaped, nothing is changed
    /// </summary>
    /// <param name="name"></param>
    public void OnProjectNameEditEnd(string name)
    {
        CameraFocus.Instance.SelectTargetThrough(ProjectNameContainer, enabled: false);

        if (!EventSystem.current.alreadySelecting) EventSystem.current.SetSelectedGameObject(null);

        ToggleProjectNameEditing(edit: false);

        if (Input.GetKeyDown(KeyCode.Escape) || ProjectNameLabel.text.Equals(name) || string.IsNullOrEmpty(name))
        {
            // Nothing just cancel
        } else
        {
            ProjectNameLabel.text = name;
            // TODO call back the ara tutorial to start showing next part
            Debug.Log("Submit the data (1) check is text changed, (2) call submit to save data in the server");
        }
    }

    #endregion
}
