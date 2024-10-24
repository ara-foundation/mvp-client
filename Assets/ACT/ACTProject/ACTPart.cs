using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ACTPart : MonoBehaviour, IStateReactor
{
    public enum ModeInScene
    {
        Interactive, // Default mode
        View,   // During the object placeholder, make it view
        DrawLine // For drawing the line change the select
    }
    [SerializeField]
    private Canvas Canvas;
    [SerializeField]
    public ActivityState ActivityState;
    [SerializeField]
    private GameObject Menu;
    [SerializeField]
    private MouseInput MouseInput;

    public ModeInScene Mode = ModeInScene.View;


    // Start is called before the first frame update
    void Start()
    {
        if (Mode != ModeInScene.Interactive)
        {
            Canvas.gameObject.SetActive(false);
        }
        
    }

    public void Activate()
    {
        Mode = ModeInScene.Interactive;
        ActivityState.SetActivityGroup(ACTProjects.Instance.ActivityGroup);
        Menu.SetActive(false);
        ACTLevelScene.Instance.AddPart(this);
        Canvas.worldCamera = ACTProjects.Instance.Camera;
        Canvas.gameObject.SetActive(true);

    }

    private void OnDestroy()
    {
        if (Mode == ModeInScene.Interactive)
        {
            ACTLevelScene.Instance.RemovePart(this);
            Menu.SetActive(false);
        }
    }

    public void Select(bool enabled)
    {
        if (Mode == ModeInScene.Interactive)
        {
            Menu.SetActive(enabled);
        } else if (Mode == ModeInScene.DrawLine)
        {
            if (enabled)
            {
                Debug.Log($"Make {gameObject.name} as a point of line: {enabled}");
            }
        }
    }

    public void Focus(bool enabled)
    {
    }

    public void Highlight(bool enabled)
    {
    }

    public void Clear()
    {
    }

    ////////////////////////////////////////////////////
    ///
    ///  Change the mode of part in the scene
    ///
    ////////////////////////////////////////////////////
    public void Interactive(bool on)
    {
        if ((on && Mode == ModeInScene.Interactive) || (!on && Mode == ModeInScene.View))
        {
            return;
        }
        if (!on)
        {
            ActivityState.ChangeMode(StateMode.None);
            Mode = ModeInScene.View;
        } else
        {
            Mode = ModeInScene.Interactive;
        }
        MouseInput.enabled = on;
    }

    public void SetLineMode(bool on)
    {
        if ((on && Mode == ModeInScene.DrawLine) || (!on && Mode == ModeInScene.View))
        {
            return;
        }
        if (!on)
        {
            ActivityState.ChangeMode(StateMode.None);
            Mode = ModeInScene.View;
        }
        else
        {
            Mode = ModeInScene.DrawLine;
        }
        MouseInput.enabled = on;
    }
}
