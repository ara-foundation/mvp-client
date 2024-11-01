using Lean.Gui;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(ACTPart_controller))]
public class ACTPart_edit : MonoBehaviour
{
    public delegate void ProjectNameEditedDelegate(string name, bool submitted);
    public delegate void TechStackEditedDelegate(string name, bool submitted);
    public delegate void BudgetEditedDelegate(double amount, bool submitted);
    public delegate void MaintainerNameEditedDelegate(string name, bool submitted);
    public delegate void ModelEditedDelegate(ACTPartModel model);

    [HideInInspector]
    private ACTPart_controller Controller;

    private string maintainerName = "none";

    /// <summary>
    /// OnProjectNameEdited is invoked when project name switches back from edit field to a label.
    /// First argument is the new string, the second argument is is it submitted
    /// </summary>
    public ProjectNameEditedDelegate OnProjectNameEdited;
    public TechStackEditedDelegate OnTechStackEdited;
    public BudgetEditedDelegate OnBudgetEdited;
    public MaintainerNameEditedDelegate OnMaintainerNameEdited;
    public ModelEditedDelegate OnModelEdited;

    void Awake()
    {
        Controller = GetComponent<ACTPart_controller>();
    }

    // Start is called before the first frame update
    void Start()
    {
        // Show a project name label, hide the project name editing field.
        Controller.ToggleProjectNameEditing(edit: false);
        Controller.ToggleMaintainerEditing(edit: false);
        /*if (Controller != null)
        {
            Controller.SetBudget(0);
        }*/
    }

    #region ProjectName


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

        Controller.ProjectNameState.ChangeMode(StateMode.None);
        Controller.ToggleProjectNameEditing(edit: true);
        CameraFocus.Instance.SelectTargetThrough(Controller.ProjectNameContainer, selecting: true);
    }

    /// <summary>
    /// Escaped, nothing is changed or perhaps project name was changed
    /// </summary>
    /// <param name="name"></param>
    public void OnProjectNameEditEnd(string name)
    {
        CameraFocus.Instance.SelectTargetThrough(Controller.ProjectNameContainer, selecting: false);

        if (!EventSystem.current.alreadySelecting) EventSystem.current.SetSelectedGameObject(null);

        Controller.ToggleProjectNameEditing(edit: false);
        var submitted = false;

        if (!Input.GetKeyDown(KeyCode.Escape) && !string.IsNullOrEmpty(name) && !Controller.ProjectName().Equals(name))
        {
            submitted = true;
            var model = Controller.SetProjectName(name);
            OnModelEdited?.Invoke(model);
        }

        OnProjectNameEdited?.Invoke(name, submitted);
        OnProjectNameEdited = null;
    }

    #endregion

    #region TechStack

    /// <summary>
    /// Double click on the tech stack
    /// </summary>
    /// <param name="focused"></param>
    public void OnEditTechStack(bool focused)
    {
        Controller.TechStackWindow.Set(focused);
        CameraFocus.Instance.SelectTargetThrough(Controller.TechStackCameraTarget, selecting: focused);
    }

    public void OnTechStackEditEnd(string content)
    {
        if (!EventSystem.current.alreadySelecting) EventSystem.current.SetSelectedGameObject(null);

        if (Input.GetKeyDown(KeyCode.Escape) || string.IsNullOrEmpty(content))
        {
            Controller.TechStackMenuButton.Focus();
            OnTechStackEdited?.Invoke(content, false);
            OnTechStackEdited = null;

        }
    }

    public void OnTechStackSubmitted()
    {
        var techStack = Controller.TechStack();
        if (string.IsNullOrEmpty(techStack))
        {
            Notification.Instance.Show("Tech Stack is empty");
            return;
        }
        Controller.TechStackMenuButton.Focus();
        OnTechStackEdited?.Invoke(techStack, true);
        OnTechStackEdited = null;

        OnModelEdited?.Invoke(Controller.SetTechStack(techStack));
    }

    #endregion

    #region Budget

    /// <summary>
    /// Double click on the tech stack
    /// </summary>
    /// <param name="focused"></param>
    public void OnEditBudget(bool focused)
    {
        Controller.BudgetWindow.Set(focused);
        CameraFocus.Instance.SelectTargetThrough(Controller.BudgetCameraTarget, selecting: focused);
        
        if (focused)
        {
            Controller.BudgetPieChart.GenerateChart();
        }
    }

    public void OnCancelBudgetEdit()
    {
        if (!EventSystem.current.alreadySelecting) EventSystem.current.SetSelectedGameObject(null);

        OnBudgetEdited?.Invoke(0, true);
        OnBudgetEdited = null;
    }

    public void OnBudgetSubmitted()
    {
        if (!EventSystem.current.alreadySelecting) EventSystem.current.SetSelectedGameObject(null);

        OnBudgetEdited?.Invoke(0, true);
        OnBudgetEdited = null;
    }

    #endregion

    #region Maintainer

    public void OnEditMaintainer(bool focused)
    {
        if (!focused)
        {
            return;
        }

        Controller.MaintainerState.ChangeMode(StateMode.None);
        Controller.ToggleMaintainerEditing(edit: true);
        CameraFocus.Instance.SelectTargetThrough(Controller.MaintainerCameraTarget, selecting: true);
    }

    public void OnMaintainerEditEnd(string name)
    {
        CameraFocus.Instance.SelectTargetThrough(Controller.MaintainerCameraTarget, selecting: false);

        if (!EventSystem.current.alreadySelecting) EventSystem.current.SetSelectedGameObject(null);

        Controller.ToggleMaintainerEditing(edit: false);
        var submitted = false;
        var editedName = name;

        if (!Input.GetKeyDown(KeyCode.Escape) && !maintainerName.Equals(name) && !string.IsNullOrEmpty(name))
        {
            submitted = true;
            maintainerName = name;
            var model = Controller.SetMaintainerName(maintainerName);
            OnModelEdited?.Invoke(model);
        }

        OnMaintainerNameEdited?.Invoke(name, submitted);
        OnMaintainerNameEdited = null;
    }

    #endregion

    /// Tasks are managed in the nested level so it's a part of ACTPart
}
