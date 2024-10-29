using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ACTProject : MonoBehaviour, IStateReactor
{
    [SerializeField]
    public ActivityState ActivityState;
    [SerializeField]
    private GameObject Menu;

    // Start is called before the first frame update
    void Start()
    {
        ActivityState.SetActivityGroup(ACTLevelScene.Instance.ActivityGroup);
        Menu.SetActive(false);
    }

    private void OnDestroy()
    {
        Menu.SetActive(false);
    }

    public void Select(bool enabled)
    {
        Menu.SetActive(enabled);
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
}
