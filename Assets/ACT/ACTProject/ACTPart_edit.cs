using Lean.Gui;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ACTPart_edit : MonoBehaviour
{
    public delegate void ProjectNameEditedDelegate(string name, bool submitted);
    public delegate void TechStackEditedDelegate(string name, bool submitted);
    public delegate void BudgetEditedDelegate(double amount, bool submitted);
    public delegate void MaintainerNameEditedDelegate(string name, bool submitted);

    [Header("Project Name")]
    [SerializeField] private Transform ProjectNameContainer;
    [SerializeField] private TextMeshProUGUI ProjectNameLabel;
    [SerializeField] private ActivityState ProjectNameState;
    [SerializeField] private GameObject ProjectNameFieldContainer;
    [Space(20)]
    [Header("Tech Stack")]
    [SerializeField] private ActivityState TechStackMenuButton;
    [SerializeField] private LeanWindow TechStackWindow;
    [SerializeField] private Transform TechStackCameraTarget;
    [SerializeField] private TMP_InputField TechStackContent;
    [Space(20)]
    [Header("Budget")]
    [SerializeField] private TextMeshProUGUI BudgetMenuLabel;
    [SerializeField] private LeanWindow BudgetWindow;
    [SerializeField] private Transform BudgetCameraTarget;
    [SerializeField] private PieChart.ViitorCloud.PieChart BudgetPieChart;
    [Space(20)]
    [Header("Maintainer")]
    [SerializeField] private TextMeshProUGUI MaintainerMenuLabel;
    [SerializeField] private ActivityState MaintainerState;
    [SerializeField] private GameObject MaintainerInputFieldContainer;
    [SerializeField] private Transform MaintainerCameraTarget;
    private string maintainerName = "none";

    /// <summary>
    /// OnProjectNameEdited is invoked when project name switches back from edit field to a label.
    /// First argument is the new string, the second argument is is it submitted
    /// </summary>
    public ProjectNameEditedDelegate OnProjectNameEdited;
    public TechStackEditedDelegate OnTechStackEdited;
    public BudgetEditedDelegate OnBudgetEdited;
    public MaintainerNameEditedDelegate OnMaintainerNameEdited;

    // Start is called before the first frame update
    void Start()
    {
        // Show a project name label, hide the project name editing field.
        ToggleProjectNameEditing(edit: false);
        ToggleMaintainerEditing(edit: false);
        BudgetMenuLabel.text = "Budget: $0";
        
    }

    private void OnEnable()
    {
        if (AraAuth.Instance.IsLoggedIn(AraAuth.Instance.UserParams))
        {
            maintainerName = AraAuth.Instance.UserParams.loginParams.username;
        }
        else
        {
            maintainerName = "none";
        }
        SetMaintainerName();
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
        CameraFocus.Instance.SelectTargetThrough(ProjectNameContainer, selecting: true);
    }

    /// <summary>
    /// Escaped, nothing is changed or perhaps project name was changed
    /// </summary>
    /// <param name="name"></param>
    public void OnProjectNameEditEnd(string name)
    {
        CameraFocus.Instance.SelectTargetThrough(ProjectNameContainer, selecting: false);

        if (!EventSystem.current.alreadySelecting) EventSystem.current.SetSelectedGameObject(null);

        ToggleProjectNameEditing(edit: false);
        var submitted = false;
        var editedName = name;

        if (Input.GetKeyDown(KeyCode.Escape) || ProjectNameLabel.text.Equals(name) || string.IsNullOrEmpty(name))
        {
            editedName = ProjectNameLabel.text;
        } else
        {
            submitted = true;
            ProjectNameLabel.text = name;
            // TODO call back the ara tutorial to start showing next part
            Debug.Log("Submit the data (1) check is text changed, (2) call submit to save data in the server");
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
        TechStackWindow.Set(focused);
        CameraFocus.Instance.SelectTargetThrough(TechStackCameraTarget, selecting: focused);
    }

    public void OnTechStackEditEnd(string content)
    {
        if (!EventSystem.current.alreadySelecting) EventSystem.current.SetSelectedGameObject(null);

        if (Input.GetKeyDown(KeyCode.Escape) || string.IsNullOrEmpty(content))
        {
            TechStackMenuButton.Focus();
            OnTechStackEdited?.Invoke(content, false);
            OnTechStackEdited = null;
        }
    }

    public void OnTechStackSubmitted()
    {
        TechStackMenuButton.Focus();
        OnTechStackEdited?.Invoke(TechStackContent.text, true);
        OnTechStackEdited = null;
    }

    #endregion

    #region Budget

    /// <summary>
    /// Double click on the tech stack
    /// </summary>
    /// <param name="focused"></param>
    public void OnEditBudget(bool focused)
    {
        BudgetWindow.Set(focused);
        CameraFocus.Instance.SelectTargetThrough(BudgetCameraTarget, selecting: focused);
        
        if (focused)
        {
            BudgetPieChart.GenerateChart();
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

    void ToggleMaintainerEditing(bool edit)
    {
        MaintainerMenuLabel.gameObject.SetActive(!edit);
        MaintainerInputFieldContainer.SetActive(edit);
    }

    void SetMaintainerName()
    {
        MaintainerMenuLabel.text = "Maintainer: " + maintainerName;
    }

    public void OnEditMaintainer(bool focused)
    {
        if (!focused)
        {
            return;
        }

        MaintainerState.ChangeMode(StateMode.None);
        ToggleMaintainerEditing(edit: true);
        CameraFocus.Instance.SelectTargetThrough(MaintainerCameraTarget, selecting: true);
    }

    public void OnMaintainerEditEnd(string name)
    {
        CameraFocus.Instance.SelectTargetThrough(MaintainerCameraTarget, selecting: false);

        if (!EventSystem.current.alreadySelecting) EventSystem.current.SetSelectedGameObject(null);

        ToggleMaintainerEditing(edit: false);
        var submitted = false;
        var editedName = name;

        if (!Input.GetKeyDown(KeyCode.Escape) && !maintainerName.Equals(name) && !string.IsNullOrEmpty(name))
        {
            submitted = true;
            maintainerName = name;
            SetMaintainerName();
            // TODO call back the ara tutorial to start showing next part
            Debug.Log("Submit the maintainer name (1=done) check is text changed, (2=todo) call submit to save data in the server");
        }

        OnMaintainerNameEdited?.Invoke(name, submitted);
        OnMaintainerNameEdited = null;
    }

    #endregion

    /// Tasks are managed in the nested level so it's a part of ACTPart
}
