using Lean.Gui;
using NBitcoin.Protocol;
using Rundo.RuntimeEditor.Behaviours.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

/// <summary>
/// Decentralized I/O System Data
/// </summary>
[ExecuteInEditMode]
public class DIOSObjectRegistry : MonoBehaviour
{
    List<DIOSObject> objects = new ();

    private static DIOSObjectRegistry _instance;

    private static DIOSObjectRegistry Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<DIOSObjectRegistry>();
            }
            return _instance;
        }
    }

    public static void Add(DIOSObject obj)
    {
        if (Instance != null)
        {
            Instance.objects.Add(obj);
        }
    }

    public static void Delete(DIOSObject obj)
    {
        if (Instance != null)
        {
            Instance.objects.Remove(obj);
        }
    }

    public static void Selectable(bool enabled)
    {
        if (Instance != null)
        {
            var objects = Instance.objects;

            foreach (var obj in objects)
            {
                obj.Selectable(enabled);
            }
        }
    }

    public static List<DIOSObject> GetObjectsWithinArea(Rect area)
    {
        var withinAreaObjects = new List<DIOSObject>();

        if (Instance == null)
        {
            return withinAreaObjects;
        }

        var objects = Instance.objects;

        foreach (var obj in objects)
        {
            var objArea = obj.ScreenPosition();

            var leftTopCorner = new Vector2(objArea.x, objArea.y);
            var rightTopCorner = new Vector2(objArea.xMax, objArea.yMin);
            var leftBottomCorner = new Vector2(objArea.xMin, objArea.yMax);
            var rightBottomCorner = new Vector2(objArea.xMax, objArea.yMax);
            // Top is selected
            if (area.Contains(leftTopCorner) && area.Contains(rightTopCorner))
            {
                Debug.Log($"Checking {objects.Count} objects in ({area.x}, {area.y}, {area.x + area.width}, {area.y + area.height})");
                Debug.Log($"The top of {obj.transform.parent.name} in the area");

                withinAreaObjects.Add(obj);
            // Left side selected
            } else if (area.Contains(leftTopCorner) && area.Contains(leftBottomCorner)) {
                Debug.Log($"The left of {obj.transform.parent.name} in the area");
                withinAreaObjects.Add(obj);
            } else if (area.Contains(rightTopCorner) && area.Contains(rightBottomCorner))
            {
                withinAreaObjects.Add(obj);
                Debug.Log($"The right of {obj.transform.parent.name} in the area");
            }
        }

        return withinAreaObjects;
    }
}
