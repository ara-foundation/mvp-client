using Lean.Gui;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;


public class ACTPart_controller : MonoBehaviour
{
    private ACTPartModel _model;
    [SerializeField] public Canvas Canvas;
    [SerializeField] public GameObject Menu;
    [SerializeField] public Transform SplinePositionerContent;

    [Header("Project Name")]
    [SerializeField] public Transform ProjectNameContainer;
    [SerializeField] private TextMeshProUGUI ProjectNameLabel;
    [SerializeField] public ActivityState ProjectNameState;
    [SerializeField] public GameObject ProjectNameFieldContainer;
    [Space(20)]
    [Header("Tech Stack")]
    [SerializeField] public ActivityState TechStackMenuButton;
    [SerializeField] public LeanWindow TechStackWindow;
    [SerializeField] public Transform TechStackCameraTarget;
    [SerializeField] private TMP_InputField TechStackContent;
    [Space(20)]
    [Header("Budget")]
    [SerializeField] private TextMeshProUGUI BudgetMenuLabel;
    [SerializeField] public LeanWindow BudgetWindow;
    [SerializeField] public Transform BudgetCameraTarget;
    [SerializeField] public PieChart.ViitorCloud.PieChart BudgetPieChart;
    [Space(20)]
    [Header("Maintainer")]
    [SerializeField] private TextMeshProUGUI MaintainerMenuLabel;
    [SerializeField] public ActivityState MaintainerState;
    [SerializeField] public GameObject MaintainerInputFieldContainer;
    [SerializeField] public Transform MaintainerCameraTarget;

    public ACTPartModel Model { get { return _model; } }

    //
    // Tasks are managed in the nested level so it's a part of ACTPart
    //

    /// <summary>
    /// Switch between editing the project name and showing it as a label
    /// </summary>
    /// <param name="edit"></param>
    public void ToggleProjectNameEditing(bool edit)
    {
        ProjectNameState.gameObject.SetActive(!edit);
        ProjectNameFieldContainer.SetActive(edit);
    }

    public string ProjectName()
    {
        return ProjectNameLabel.text;
    }

    public string TechStack()
    {
        return TechStackContent.text;
    }

    public void ToggleMaintainerEditing(bool edit)
    {
        if (MaintainerMenuLabel != null)
        {
            MaintainerMenuLabel.gameObject.SetActive(!edit);
            MaintainerInputFieldContainer.SetActive(edit);
        }
    }

    public ACTPartModel SetProjectName(string projectName)
    {
        this._model.projectName = projectName;
        ProjectNameLabel.text = projectName;
        return this._model;
    }

    public ACTPartModel SetTechStack(string techStack)
    {
        this._model.techStack = techStack;
        TechStackContent.text = techStack;
        return this._model;
    }

    public ACTPartModel SetBudget(double budget)
    {
        this._model.budget = budget;
        SetBudgetInternal(budget);
        return this._model;
    }

    public ACTPartModel SetMaintainerName(string maintainerName)
    {
        this._model.maintainer = maintainerName;
        SetMaintainerNameInternal(maintainerName);
        return this._model;
    }

    private void SetBudgetInternal(double budget)
    {
        if (BudgetMenuLabel != null)
        {
            BudgetMenuLabel.text = $"Budget: ${budget}";
        }
    }

    private void SetMaintainerNameInternal(string maintainerName)
    {
        if (MaintainerMenuLabel != null)
        {
            MaintainerMenuLabel.text = "Maintainer: " + maintainerName;
        }
    }

    public void SetData(ACTPartModel model)
    {
        _model = model;
        if (string.IsNullOrEmpty(model.projectName))
        {
            ProjectNameLabel.text = "Part name";
        } else
        {
            ProjectNameLabel.text = model.projectName;
        }
        if (string.IsNullOrEmpty(model.techStack))
        {
            TechStackContent.text = "";
        }
        else
        {
            TechStackContent.text = model.techStack;
        }
        SetBudgetInternal(model.budget);
        if (string.IsNullOrEmpty(model.maintainer))
        {
            if (AraAuth.Instance != null && AraAuth.Instance.IsLoggedIn(AraAuth.Instance.UserParams))
            {
                SetMaintainerNameInternal(AraAuth.Instance.UserParams.loginParams.username);
            }
            else
            {
                SetMaintainerNameInternal("none");
            }
        }
        else
        {
            SetMaintainerNameInternal(model.maintainer);
        }
    }
}
