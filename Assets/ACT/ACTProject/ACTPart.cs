using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ACTPart : MonoBehaviour, IStateReactor
{
    [SerializeField]
    private Canvas Canvas;
    [SerializeField]
    public ActivityState ActivityState;
    [SerializeField]
    private GameObject Menu;
    [SerializeField]
    private MouseInput MouseInput;

    private bool activated = false;

    // Start is called before the first frame update
    void Start()
    {
        if (!activated)
        {
            Canvas.gameObject.SetActive(false);
        }
    }

    public void Activate()
    {
        activated = true;
        ActivityState.SetActivityGroup(ACTProjects.Instance.ActivityGroup);
        Menu.SetActive(false);
        ACTLevelScene.Instance.AddPart(this);
        Canvas.worldCamera = ACTProjects.Instance.Camera;
        Canvas.gameObject.SetActive(true);

    }

    private void OnDestroy()
    {
        if (activated)
        {
            ACTLevelScene.Instance.RemovePart(this);
            Menu.SetActive(false);
        }
    }

    public void Select(bool enabled)
    {
        if (activated)
        {
            Menu.SetActive(enabled);
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
    ///  Disable/Enable
    ///
    ////////////////////////////////////////////////////
    public void Interactive(bool on)
    {
        if (!activated)
        {
            return;
        }
        if (!on)
        {
            ActivityState.ChangeMode(StateMode.None);
        }
        MouseInput.enabled = on;
    }
}
