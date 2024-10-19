using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MouseInput : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public ActivityState ActivityState;
    public bool EnableSingleClick = true; // If it's catched by button or toggle

    void Awake()
    {
        if (ActivityState == null)
        {
            Debug.LogError("ActivityState for MouseInput wasn't set for " + transform.name + " of " + transform.parent.name);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        int clickCount = eventData.clickCount;

        if (clickCount == 2)
            OnDoubleClick(); 
        else if (clickCount == 1 && EnableSingleClick)
            OnSingleClick();
    }

    void OnSingleClick()
    {
        ActivityState.Select();
    }

    void OnDoubleClick()
    {
        ActivityState.ChangeMode(StateMode.Focused);
    }

    //Detect if the Cursor starts to pass over the GameObject
    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        ActivityState.ChangeMode(StateMode.Highlighted);
    }

    //Detect when Cursor leaves the GameObject
    public void OnPointerExit(PointerEventData pointerEventData)
    {
        ActivityState.ChangeMode(StateMode.Unhighlight);
    }

}