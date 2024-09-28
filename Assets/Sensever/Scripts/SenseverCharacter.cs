using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;


public class SenseverCharacter : MonoBehaviour
{
    [SerializeField]
    public InputField terminalTextField;
    public string SenseverBackendUrl;

    private Transform cameraTransform;

    public GameObject meshBody;
    public SenseverChat senseverChat;

    private Vector3 originalPosition;

    public class ImplementResult
    {
        public string code;
        public bool correct;
    }


    // Start is called before the first frame update
    void Start()
    {
        originalPosition = transform.position;
        cameraTransform = Camera.main.transform;
        if (cameraTransform == null)
        {
            Debug.LogError("Camera brain is not set to the main camera");
            return;
        }
    }

    private void Update()
    {
        meshBody.transform.LookAt(cameraTransform);
    }

    public void MoveTo(Transform point)
    {
        if (point == null)
        {
            transform.position = originalPosition;
        } else
        {
            transform.position = point.position;
        }
    }

    // Tries to implement the given stuff
    public async Task<ImplementResult> Implement(string issue)
    {
        ImplementResult incorrectImplement = new()
        {
            correct = false
        };

        var url = SenseverBackendUrl;
        var res = await WebClient.Get(url, 1);
        if (res.Length == 0)
        {
            Debug.LogError("Sensever is not available");
            return incorrectImplement;
        }

        if (!SenseverBackendUrl.EndsWith("/"))
        {
            url += "/";
        }
        url += "implement?code_gen_task=" + issue;

        try
        {
            res = await WebClient.Get(url);
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
            return incorrectImplement;
        }
        ImplementResult implement;
        try
        {
            implement = JsonUtility.FromJson<ImplementResult>(res);
        }
        catch (Exception e)
        {
            Debug.LogError(e + " for " + res);
            return incorrectImplement;
        }

        return implement;
    }

    public void Highlight(bool _enabled)
    {
    }

    public void Select(bool enabled)
    {
        if (enabled)
        {
        }
    }

    public void Clear()
    {
        
    }

    private void ShowTerminal()
    {
       
    }

    public void Focus(bool enabled)
    {
        if (enabled)
        {
            // if it's next to the web variant, then
            if (senseverChat.appeared)
            {
            } else
            {
                MoveTo(null);
                ShowTerminal();
            }
        }
        else
        {
           
        }
    }

    public string TerminalText()
    {
        return terminalTextField.text;
    }
}
