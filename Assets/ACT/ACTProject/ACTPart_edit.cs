using Lean.Gui;
using Nethereum.Web3;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(ACTPart_controller))]
public class ACTPart_edit : MonoBehaviour
{
    public delegate void ProjectNameEditedDelegate(string name, bool submitted);
    public delegate void TechStackEditedDelegate(string name, bool submitted);
    public delegate void BudgetEditedDelegate(decimal amount, bool submitted);
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
            Controller.TechStackMenuButton.ToggleFocus();
            OnTechStackEdited?.Invoke(content, false);
            OnTechStackEdited = null;

        }
    }

    public void OnTechStackSubmitted()
    {
        var techStack = Controller.TechStack();
        if (string.IsNullOrEmpty(techStack))
        {
            Notification.Show("Tech Stack is empty");
            return;
        }
        Controller.TechStackMenuButton.ToggleFocus();
        OnTechStackEdited?.Invoke(techStack, true);
        OnTechStackEdited = null;

        OnModelEdited?.Invoke(Controller.SetTechStack(techStack));
    }

    #endregion

    #region Budget

    /// <summary>
    /// Returns the budget, and used budgets. 
    /// For level one, the budget is taken from the ACTSession.
    /// </summary>
    /// <returns></returns>
    private Tuple<decimal, decimal> GetBudgets()
    {
        if (Controller.Model.level == 0)
        {
            Debug.LogError("Budget works with Controller.Model.level only");
            return Tuple.Create(new decimal(0), new decimal(0));
        }

        if (Controller.Model.level == 1)
        {
            // First budget is taken from ACTSession.project.plan
            var plan = ACTSession.Instance.Development.plan[0];
            decimal costUsd = 0;
            if (!string.IsNullOrEmpty(plan.cost_usd))
                costUsd = Web3.Convert.FromWei(BigInteger.Parse(plan.cost_usd));
            decimal usedBudget = 0;
            if (!string.IsNullOrEmpty(plan.used_budget))
                usedBudget = Web3.Convert.FromWei(BigInteger.Parse(plan.used_budget));

            return Tuple.Create(costUsd, usedBudget);
        }

        var partModel = ACTSession.Instance.CurrentPart();
        if (partModel == null)
        {
            Debug.LogError("Current part from ACTSession is empty");
            return Tuple.Create(new decimal(0), new decimal(0));
        }

        return Tuple.Create(partModel.budget, partModel.usedBudget ?? new decimal(0));
    }

    private void SetUsedBudget(decimal addition)
    {
        if (Controller.Model.level == 0)
        {
            Debug.LogError("Budget works with Controller.Model.level only");
            return;
        }

        if (addition == 0)
        {
            return;
        }

        if (Controller.Model.level == 1)
        {
            // First budget is taken from ACTSession.project.plan
            var plan = ACTSession.Instance.Development.plan[0];
            decimal usedBudget = 0;
            if (!string.IsNullOrEmpty(plan.used_budget))
                usedBudget = Web3.Convert.FromWei(BigInteger.Parse(plan.used_budget));
            usedBudget += addition;
            plan.used_budget = Web3.Convert.ToWei(usedBudget).ToString();
            return;
        }

        var partModel = ACTSession.Instance.CurrentPart();
        if (partModel == null)
        {
            Debug.LogError("Current part from ACTSession is empty");
            return;
        }

        if (partModel.usedBudget > 0)
        {
            partModel.usedBudget += addition;
        } else
        {
            partModel.usedBudget = addition;
        }
        ACTSession.Instance.CurrentPart(partModel);
    }

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
            var budgets = GetBudgets();
            Controller.SetBudgetBox(budgets.Item1, budgets.Item2);
        }
    }

    // For slider
    public void OnBudgetEditEnd()
    {
        var budgetAllocation = Controller.BudgetSliderValue();
        Controller.ChangeBudgetAllocation(budgetAllocation, updateInput: true, updateSlider: false);
    }

    // For input field
    public void OnBudgetEditEnd(string budgetValue)
    {
        var budgetAllocation = decimal.Parse(budgetValue);
        Controller.ChangeBudgetAllocation(budgetAllocation, updateInput: false, updateSlider: true);
    }

    public void OnCancelBudgetEdit()
    {
        if (Controller.BudgetWindow.On)
        {
            Controller.BudgetWindow.Set(false);
        }
        CameraFocus.Instance.SelectTargetThrough(Controller.BudgetCameraTarget, selecting: false);

        OnBudgetEdited?.Invoke((decimal)0, false);
        OnBudgetEdited = null;
    }

    public void OnBudgetSubmitted()
    {
        if (Controller.BudgetWindow.On)
        {
            Controller.BudgetWindow.Set(false);
        }
        CameraFocus.Instance.SelectTargetThrough(Controller.BudgetCameraTarget, selecting: false);

        var budgetValue = Controller.BudgetSliderValue();
        var updatedUsedBudget = budgetValue - Controller.Model.budget;

        var model = Controller.SetBudget(budgetValue);
        if (model == null)
        {
            Notification.Show("Budget was not set validly");
            OnBudgetEdited?.Invoke((decimal)0, false);
            OnBudgetEdited = null;

            return;
        }

        SetUsedBudget(updatedUsedBudget);

        OnModelEdited?.Invoke(model);

        OnBudgetEdited?.Invoke(budgetValue, true);
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
