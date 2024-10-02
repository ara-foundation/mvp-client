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
    public EventController senseverChat;

    private Vector3 originalPosition;

    // Start is called before the first frame update
    void Start()
    {
        originalPosition = transform.position;
        cameraTransform = Camera.main.transform;
        if (cameraTransform == null)
        {
            Debug.LogError("Camera is not set to the main camera");
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

}
