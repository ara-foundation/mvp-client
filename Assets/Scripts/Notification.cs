using Lean.Gui;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Notification : MonoBehaviour
{
    private static Notification _instance;

    public static Notification Instance
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

    [SerializeField] private LeanPulse LeanPulse;
    [SerializeField] private Text Message;

    public void Show(string message)
    {
        Message.text = message;
        LeanPulse.Pulse();
    }
}
