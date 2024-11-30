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
    Rect prevArea = new();
    List<DIOSObject> diosObjects = new();

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

        prevArea = new Rect();
        diosObjects = new();
        boxEnd = Vector3.zero;
        boxOrigin = Vector3.zero;
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
            // everything is here

            drawing = false;

            boxEnd = Vector3.zero;
            boxOrigin = Vector3.zero;
            prevArea = new Rect();
            diosObjects = new();
            boxEnd = Vector3.zero;
            boxOrigin = Vector3.zero;
        }
    }

    void HighlightDIOS(Rect area)
    {
        if (area.x == prevArea.x && 
            area.y == prevArea.y && 
            area.width == prevArea.width && 
            area.height == prevArea.height) 
            return; 
        else 
            prevArea = area;

        if (area.width != 0 && area.height != 0)
        {
            diosObjects.Clear();

            var mainCameraArea = area;
            diosObjects = DIOSObjectRegistry.GetObjectsWithinArea(mainCameraArea);
        }
    }

    /// <summary>
    /// Given the Ara Frontend's draw selection is in the Ara Frontend area,
    /// let's convert into the main camera area;
    /// </summary>
    /// <param name="area"></param>
    /// <returns></returns>
    Rect ConvertToMainScreenArea(Rect area)
    {
        var frontendCamera = AraFrontend.Instance.AraFrontendCamera;
        var mainCamera = AraFrontend.Instance.MainCamera;

        Debug.Log($"Convert area to viewport area to(x={area.x}, y={area.y}), (width={area.width}, height={area.height})");

        var start = new Vector3(area.x, area.y, 0);
        var startViewport = frontendCamera.ScreenToViewportPoint(start);

        Debug.Log($"From Ara Frontend screen={start} to viewport={startViewport}");

        var startFrontend = new Vector3(startViewport.x, startViewport.y, 0);
        var mainStart = mainCamera.ViewportToScreenPoint(startFrontend);

        var mainScreenArea = new Rect(mainStart.x, mainStart.y, area.width, area.height);

        Debug.Log($"Area in frontend camera={area}, Area in main camera screen: {mainScreenArea}");
        return mainScreenArea;
    }

    void OnGUI()
    {
        if (drawing)
        {
            Rect area = new(boxOrigin.x, Screen.height - boxOrigin.y, boxEnd.x - boxOrigin.x, boxOrigin.y - boxEnd.y);

            Rect lineArea = area;
            lineArea.height = 1; // Top line
            GUI.DrawTexture(lineArea, selectTexture);
            lineArea.y = area.yMax - 1; // Bottom
            GUI.DrawTexture(lineArea, selectTexture);
            lineArea = area;
            lineArea.width = 1; // Left
            GUI.DrawTexture(lineArea, selectTexture);
            lineArea.x = area.xMax - 1;// Right
            GUI.DrawTexture(lineArea, selectTexture);

            HighlightDIOS(area);
        }
    }

}
