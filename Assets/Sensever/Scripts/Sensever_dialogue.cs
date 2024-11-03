using Lean.Gui;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Sensever_dialogue : MonoBehaviour
{
    public static readonly int None = -1;
    [SerializeField] private EventController EventController;
    [SerializeField] private Button ContinueButton;
    public bool AllowSkip = false;
    [Space(20)]
    [Header("Tutorial Text (Set by the caller, here to debug)")]
    public List<Event> TutorialTexts;
    private int showed;
    private Action<int> OnTextEndCallback;
    private Action<int> OnTextStartCallback;
    /// <summary>
    /// Sometimes the tutorial text is required until the user fulfills the requirement.
    /// </summary>
    private Func<int, bool> HideInsteadContinue;

    #region StartSensever

    /// <summary>
    /// Start a tutorial with HideInsteadContinue checker
    /// </summary>
    /// <param name="textEndCallback"></param>
    /// <param name="textStartCallback"></param>
    /// <param name="hideInsteadContinue"></param>
    public void StartTutorial(Action<int> textEndCallback, Action<int> textStartCallback, Func<int, bool> hideInsteadContinue)
    {
        StartTutorial(textEndCallback, textStartCallback);
        HideInsteadContinue = hideInsteadContinue;
    }

    /// <summary>
    /// Start a tutorial without HideInsteadContinue
    /// </summary>
    /// <param name="textEndCallback"></param>
    /// <param name="textStartCallback"></param>
    public void StartTutorial(Action<int> textEndCallback, Action<int> textStartCallback)
    {
        HideInsteadContinue = null;
        OnTextEndCallback = textEndCallback;
        OnTextStartCallback = textStartCallback;

        StartTexting(NextStep(None));
    }

    /// <summary>
    /// Start animating the dialogoue at the step.
    /// </summary>
    /// <param name="step"></param>
    public void ShowAt(int step)
    {
        if (TutorialTexts == null || TutorialTexts.Count == 0)
        {
            Notification.Instance.Show("No tutorial was set to start");
            return;
        }
        // Force Update is called here, to ensure that end call receives the callback about previous event.
        // Otherwise, show will place arbitrary showed parameter.
        ForceStopTexting();
        StartTexting(step);
    }

    #endregion

    void OnTextAnimationEnd()
    {
        showed = NextStep(showed);
        EnableButton();
        // Hello = 0, Idea = 1, Exit = 2 for ACT
        OnTextEndCallback?.Invoke(showed);
    }

    private int NextStep(int currentStep)
    {
        if (currentStep >= TutorialTexts.Count - 1)
        {
            return None;
        }
        return currentStep + 1;
    }

    private int PrevStep(int nextStep)
    {
        if (nextStep <= None)
        {
            return 0;
        }
        return nextStep - 1;
    }

    public void CancelTexting()
    {
        ForceStopTexting();
    }

    /// <summary>
    /// Returns a flag to hide instead continue if showed text is marked as such.
    /// </summary>
    /// <returns></returns>
    private void ForceStopTexting()
    {
        if (EventController.IsTexting())
        {
            EventController.StopTexting();
        }
    }

    /// <summary>
    /// Starts the texting animation
    /// </summary>
    private void StartTexting(int nextStep)
    {
        showed = PrevStep(nextStep);
        OnTextStartCallback?.Invoke(nextStep);
        var text = TutorialTexts.ElementAtOrDefault(nextStep);
        if (text != null)
        {
            DisableButton();
            EventController.StartTexting(TutorialTexts[nextStep], OnTextAnimationEnd);
        }
    }

    public void OnClickContinue()
    {
        ForceStopTexting();
        if (HideInsteadContinue != null && HideInsteadContinue(showed))
        {
            Sensever_window.Instance.HideSensever();
            return;
        }

        StartTexting(NextStep(showed));
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
