using Lean.Gui;
using NBitcoin.Protocol;
using Rundo.RuntimeEditor.Behaviours.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Decentralized I/O System Data
/// </summary>
public class DIOSData : MonoBehaviour
{
    /// <summary>
    /// The DataType
    /// </summary>
    [Serializable]
    public enum Type {
        NoData = -1, // If it's a LeanWindow with a Canvas Group then selected the background
        Text = 0, // A text either from Text component or TMPText component
        Image = 1, // An Image or Raw Image component.
        Button = 2, // Either Button or LeanButton component
        InputField = 3, // Either InputField or LeanInput Field component
        Card = 4,   // A card that could be any of Lungta data (Logos, Aurora etc.)
        Idea = 5,   // Logos idea
        Scenario = 6, // Aurora user scenario
        Plan = 7, // Maydone plan
        Development = 8, // Act development, it's a 3d interface
        Scene = 9, // Act development scene,
        Part = 10, // Act development plan,
        Task = 11, // Act scene
        ContentList = 13, // Content.cs or InputList.cs were given, put it in the viewport, and main target is content list
        MaintainerCard = 14, // If it's CardMaintainer
        User = 15, // If it's AraAuth
        Logos = 16,
        Aurora = 17,
        Maydone = 18,
        ACT = 19,
        Sangha = 20,
        Auth = 21,
        Notification = 22,
        Zero = 12, // Anything unknown
    }

    [SerializeField] public List<Type> DataTypes;
    [SerializeField] public GameObject MainTarget;
    [SerializeField] public string Payload;    // Additional info, for example for Auth data type it will show Profile window type

    /// <summary>
    /// Return the name of the object including the type
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="types"></param>
    /// <returns></returns>
    public static string TypeAndName(GameObject gameObject, Type dataType, bool cache = true)
    {
        var name = gameObject.name;
        var subType = "";
        
        var found = gameObject.TryGetComponent<DIOSData>(out var data);
        if (found)
        {
            if (data.MainTarget != null)
            {
                name = data.MainTarget.name;
            }
            if (!string.IsNullOrEmpty(data.Payload))
            {
                subType = $"({data.Payload})";
            }
        }
        return $"{dataType}{subType}={name}";
    }

    /// <summary>
    /// Extract the ItemMetaData into list of item metadata for each data type including its valid game object.
    /// </summary>
    /// <returns></returns>
    public static List<ProjectItemMetaData> Extract(ProjectItemMetaData item)
    {
        var items = new List<ProjectItemMetaData>();
        if (item.DiosType == null || item.DiosType.Count == 0)
        {
            items.Add(item);
            return items;
        }

        foreach (var diosType in item.DiosType)
        {
            var go = MainGameObject(item.GameObject, diosType);
            if (go == null)
            {
                Debug.LogWarning($"Skipping Extraction of dios object from {item.GameObject.name} for {diosType} of {item.DiosType.Count} types");
                continue;
            }

            var metaData = new ProjectItemMetaData
            {
                GameObject = go,
                DiosType = new List<Type> { diosType },
            };
            items.Add(metaData);
        }

        return items;
    }

    #region forEachDataType
    ////////////////////////////////////////////////////////////////////////////////////
    ///
    /// For Each Data type
    /// 
    ////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Return 
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="dataType"></param>
    /// <returns></returns>

    public static GameObject MainGameObject(GameObject gameObject, Type dataType)
    {
        if (dataType == Type.NoData || dataType == Type.Zero)
        {
            return null;
        }
        // Given explicitly
        var found = gameObject.TryGetComponent<DIOSData>(out var diosData);
        if (found)
        {
            if (diosData.MainTarget != null)
            {
                return diosData.MainTarget.gameObject;
            }
            return gameObject;
        }

        var types = new List<Type>();

        if (dataType == Type.User)
        {
            Debug.LogWarning($"Dios User data type must have a payload. Attach DIOS to the auth windows");
            return AraAuth.Instance.gameObject;
        }

        // UI Elements means return this object 
        if (dataType == Type.Text || 
            dataType == Type.InputField || 
            dataType == Type.Button ||
            dataType == Type.Image)
        {
            return gameObject;
        }

        // For objects detected by CardBody.cs it's the parent
        if (dataType == Type.Card || 
            dataType == Type.Idea || 
            dataType == Type.MaintainerCard ||
            dataType == Type.Plan)
        {
            return gameObject.transform.parent.gameObject;
        }

        Debug.Log("Return the default DIOS game object");
        return gameObject;
    }

    /// <summary>
    /// Returns a data type of the game object by detecting it's data
    /// </summary>
    /// <param name="gameObject"></param>
    /// <returns></returns>
    public static List<Type> GameObjectDataType(GameObject gameObject)
    {
        // Given explicitly
        var found = gameObject.TryGetComponent<DIOSData>(out var diosData);
        if (found)
        {
            return diosData.DataTypes;
        }

        if (gameObject.TryGetComponent<LeanWindow>(out _))
        {
            Debug.Log($"{gameObject.name} is a lean window so it's no data by design");
            return new List<Type> { Type.NoData };
        }

        var types = new List<Type>();

        if (gameObject.TryGetComponent<AraAuth>(out _))
        {
            types.Add(Type.User);
        }
       
        if (gameObject.TryGetComponent<Text>(out _) || gameObject.TryGetComponent<TextMeshProUGUI>(out _))
        {
            types.Add(Type.Text);
        }
        else if (gameObject.TryGetComponent<InputField>(out _) || gameObject.TryGetComponent<TMP_InputField>(out _))
        {
            types.Add(Type.InputField);
        }
        else if (gameObject.TryGetComponent<Button>(out _) || gameObject.TryGetComponent<LeanButton>(out _))
        {
            types.Add(Type.Button);
        }
        else if (gameObject.TryGetComponent<Image>(out _) || gameObject.TryGetComponent<RawImage>(out _))
        {
            types.Add(Type.Image);
        }
        if (gameObject.TryGetComponent<CardBody>(out _)) {
            if (gameObject.transform.parent == null)
            {
                Debug.LogError($"The {gameObject.name} has a card body but it doesn't have a parent");
            }
            var cardFound = gameObject.transform.parent.TryGetComponent<Card>(out _);


            var logosCard = gameObject.transform.parent.TryGetComponent<CardLogos>(out _);
            var newPostCard = gameObject.transform.parent.TryGetComponent<CardNewPost>(out _);
            if (logosCard || newPostCard)
            {
                types.Add(Type.Idea);
                return types;
            }

            if (gameObject.transform.parent.TryGetComponent<CardMaintainer>(out _))
            {
                types.Add(Type.MaintainerCard);
                return types;
            }

            if (gameObject.transform.parent.TryGetComponent<CardMaydone>(out _))
            {
                types.Add(Type.Plan);
            }

            if (cardFound)
            {
                types.Add(Type.Card);
            }
        }

        if (types.Count == 0)
        {
            types.Add(Type.Zero);
        }

        return types;
    }

    #endregion forEachDataType

}
