using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawSelect : MonoBehaviour
{
    Texture2D selectTexture;

    Vector3 boxOrigin;
    Vector3 boxEnd;

    bool drawing;
    bool ready = false;

    private void Start()
    {
        ready = false;
    }

    public void OnReady()
    {
        Reset();
        ready = true;
    }

    private void Reset()
    {
        selectTexture = new Texture2D(1, 1);
        selectTexture.SetPixel(0, 0, UnityEngine.Color.white);
        selectTexture.Apply();
    }

    public void OnUnCancel()
    {
        ready = false;
        Reset();
    }

    void Update()
    {
        if (!ready)
        {
            return;
        }
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            boxOrigin = Input.mousePosition;
            drawing = true;
        }

        if (drawing)
        {
            boxEnd = Input.mousePosition;
        }

        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            drawing = false;
        }
    }

    void OnGUI()
    {
        if (drawing)
        {
            Rect area = new(boxOrigin.x, Screen.height - boxOrigin.y, boxEnd.x - boxOrigin.x, boxOrigin.y - boxEnd.y);

            Rect lineArea = area;
            lineArea.height = 1; //Top line
            GUI.DrawTexture(lineArea, selectTexture);
            lineArea.y = area.yMax - 1; //Bottom
            GUI.DrawTexture(lineArea, selectTexture);
            lineArea = area;
            lineArea.width = 1; //Left
            GUI.DrawTexture(lineArea, selectTexture);
            lineArea.x = area.xMax - 1;//Right
            GUI.DrawTexture(lineArea, selectTexture);
        }
    }

}
