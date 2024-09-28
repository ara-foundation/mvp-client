using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

public class SenseverChat : MonoBehaviour
{
    public UnityEngine.UI.Text info;
    public bool appeared;
    private bool appear;
    private string appearText;

    private void Awake()
    {
        appeared = false;
        appearText = "";
        appear = false;
    }

    void startDisappear()
    {
    }

    // Start is called before the first frame update
    public void Show(string text)
    {
        info.text = "";
        appearText = text;
        if (appeared)
        {
            CancelInvoke(nameof(startDisappear));
            appear = true;
            startDisappear();
        }
    }

    public void Hide()
    {
        info.text = "";
        if (!appeared)
        {
            return;
        }
        CancelInvoke(nameof(startDisappear));
        startDisappear();
    }
}
