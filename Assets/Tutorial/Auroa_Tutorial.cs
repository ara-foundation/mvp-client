using Highlighter;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Auroa_Tutorial : MonoBehaviour
{
    public enum TutorialStep
    {
        None = -1,
        Hello = 0,
        Idea = 1,
        Exit = 2,
        Enjoy = 3
    }

    [SerializeField] private StandardPostProcessingCamera FogScene;
    [SerializeField] private RectangleEffect IdeaArea;
    [SerializeField] private RadialWaveEffect IdeaWave;
    [SerializeField] private RectangleEffect ExitArea;
    [SerializeField] private RadialWaveEffect ExitWave;

    private static Auroa_Tutorial _instance;

    public static Auroa_Tutorial Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<Auroa_Tutorial>();
            }
            return _instance;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Hide();
    }

    public void ShowFog()
    {
        FogScene.enabled = true;
    }

    public void Hide()
    {
        FogScene.enabled = false;
        IdeaArea.enabled = false;
        IdeaWave.enabled = false;
        ExitArea.enabled = false;
        ExitWave.enabled = false;
    }

    public void ShowIdeaArea()
    {
        IdeaArea.enabled = true;
        IdeaWave.enabled = true;
        ExitArea.enabled = false;
        ExitWave.enabled = false;
    }

    public void ShowExitArea()
    {
        IdeaArea.enabled = false;
        IdeaWave.enabled = false;
        ExitArea.enabled = true;
        ExitWave.enabled = true;
    }


}
