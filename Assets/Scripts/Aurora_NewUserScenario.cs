using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aurora_NewUserScenario : MonoBehaviour
{
    [SerializeField] private Drawer_UserScenario Drawer;
    [SerializeField] private SenseverDialogue Dialogue;
    [SerializeField] private CardLogos PinnedLogos;
    [SerializeField] private Auroa_Tutorial Tutorial;

    // Start is called before the first frame update
    void Start()
    {
        Dialogue.gameObject.SetActive(false);
        Drawer.gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        Dialogue.gameObject.SetActive(false);
        Drawer.gameObject.SetActive(false);
    }

    private void OnDialogueEnd(Auroa_Tutorial.TutorialStep showed)
    {
        if (showed == Auroa_Tutorial.TutorialStep.Exit) 
        {
            Tutorial.Hide();
        }
    }

    private void OnDialogueStart(Auroa_Tutorial.TutorialStep started)
    {
        if (started == Auroa_Tutorial.TutorialStep.Idea)
        {
            Tutorial.ShowFog();
            Tutorial.ShowIdeaArea();
        } else if (started == Auroa_Tutorial.TutorialStep.Exit)
        {
            Tutorial.ShowExitArea();
        } else if (started == Auroa_Tutorial.TutorialStep.Enjoy)
        {
            Tutorial.Hide();
        } else if (started == Auroa_Tutorial.TutorialStep.None)
        {
            Dialogue.gameObject.SetActive(false);
            Drawer.gameObject.SetActive(true);
        }
    }

    public void Show(AraDiscussion logos)
    {
        Drawer.gameObject.SetActive(false);
        Dialogue.gameObject.SetActive(true);
        Dialogue.StartTutorial(OnDialogueEnd, OnDialogueStart);
        PinnedLogos.Show(logos);

        // show the tutorial
        // highlight
        // load user scenario
        // edit scenario
        // post scenario
    }
}
