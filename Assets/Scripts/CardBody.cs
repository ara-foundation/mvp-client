using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class CardBody : MonoBehaviour
{
    [SerializeField] private UnityEngine.UI.Outline _outline;
    [SerializeField]
    private GameObject[] Fixed = { };
    [SerializeField]
    private GameObject[] Dynamic = { };

    private Image _image;

    private void OnEnable()
    {
        _outline = GetComponent<UnityEngine.UI.Outline>();
        _image = GetComponent<Image>();
    }

    // Start is called before the first frame update
    // Update is called once per frame
    void Update()
    {
        if (Fixed.Length == 0)
        {
            return;
        }
        int dynamicLength = gameObject.transform.childCount - Fixed.Length;
        if (dynamicLength != Dynamic.Length)
        {
            Dynamic = new GameObject[dynamicLength];

            for (int i = 0, dI = 0; i < gameObject.transform.childCount; i++)
            {
                GameObject child = gameObject.transform.GetChild(i).gameObject;
                bool isFixed = false;
                for (int j = 0; j < Fixed.Length; j++)
                {
                    if (Fixed[j] == child)
                    {
                        isFixed = true;
                        break;
                    }
                }

                if (!isFixed)
                {
                    Dynamic[dI] = child;

                    dI++;
                }
            }
        }
    }

    public void SetImageColor(Color color)
    {
        if (_image != null )
        {
            _image.color = color;
        }
    }

    public void ShowContent(bool onlyHeader)
    {
        if (Fixed.Length < 2)
        {
            Debug.LogError("Expected 2 fixed elements (Header, and Divider)");
            return;
        }

        
        _outline.enabled = onlyHeader;

        for (int i = 2; i<Fixed.Length; i++)
        {
            Fixed[i].SetActive(!onlyHeader);
        }
        for (int i = 0; i < Dynamic.Length; i++)
        {
            Dynamic[i].SetActive(!onlyHeader);
        }
    }
}
