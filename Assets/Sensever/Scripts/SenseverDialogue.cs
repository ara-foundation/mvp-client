using Lean.Gui;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SenseverDialogue : MonoBehaviour
{
    

    [SerializeField] private EventController EventController;
    [SerializeField] private Button ContinueButton;
    public bool AllowSkip = false;
    [Space(20)]
    [Header("Tutorial Text")]
    public List<Event> TutorialTexts;
    private Auroa_Tutorial.TutorialStep showed;
    private Action<Auroa_Tutorial.TutorialStep> OnTextEndCallback;
    private Action<Auroa_Tutorial.TutorialStep> OnTextStartCallback;
    private Action OnExitStart;

    public void StartTutorial(Action<Auroa_Tutorial.TutorialStep> textEndCallback, Action<Auroa_Tutorial.TutorialStep> textStartCallback)
    {
        OnTextEndCallback = textEndCallback;
        OnTextStartCallback = textStartCallback;
        showed = Auroa_Tutorial.TutorialStep.None;
        OnContinue();
    }

    public void ShowFirst()
    {
        OnTextEndCallback = null;
        OnTextStartCallback = null;
        showed = Auroa_Tutorial.TutorialStep.None;
        OnContinue();
    }

    void OnTextEnd()
    {
        showed = NextStep(showed);
        EnableButton();
        // Hello = 0, Idea = 1, Exit = 2 for ACT
        OnTextEndCallback?.Invoke(showed);
    }

    private Auroa_Tutorial.TutorialStep NextStep(Auroa_Tutorial.TutorialStep currentStep)
    {
        if (currentStep == Auroa_Tutorial.TutorialStep.Enjoy)
        {
            return Auroa_Tutorial.TutorialStep.None;
        }
        var step = (int)currentStep;
        return (Auroa_Tutorial.TutorialStep)step + 1;
    }

    public void OnContinue()
    {
        var nextStep = NextStep(showed);
        OnTextStartCallback?.Invoke(nextStep);
        var text = TutorialTexts.ElementAtOrDefault((int)nextStep);
        if (text != null)
        {
            DisableButton();
            EventController.StartTexting(TutorialTexts[(int)nextStep], OnTextEnd);
        }
    }

    void DisableButton()
    {
        if (!AllowSkip)
            ContinueButton.interactable = false;
    }

    void EnableButton()
    {
        if (!AllowSkip)
            ContinueButton.interactable = true;
    }
}
