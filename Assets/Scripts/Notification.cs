using Lean.Gui;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Notification : MonoBehaviour
{
    private static Notification _instance;

    [SerializeField] private LeanPulse LeanPulse;
    [SerializeField] private Text Message;

    public static Notification GetInstance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<Notification>();
            }
            return _instance;
        }
    }

    public static void Show(string message, bool debug = true)
    {
        var i = GetInstance;
        if (i != null)
        {
            i.Show(message);
        } else
        {
            if (debug)
            {
                Debug.LogError(message);
            }
        }
    }

    public void Show(string message)
    {
        Message.text = message;
        LeanPulse.Pulse();
    }
}
