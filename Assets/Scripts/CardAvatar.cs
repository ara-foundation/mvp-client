using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardAvatar : MonoBehaviour
{
    private Image Avatar;

    // Start is called before the first frame update
    void Start()
    {
        Avatar = gameObject.GetComponent<Image>();
    }

    public void HideAvatar()
    {
        gameObject.SetActive(false);
    }

    public bool LoadAvatar(string avatarUrl)
    {
        gameObject.SetActive(true);
        Debug.LogWarning($"Todo Load the avatar on {avatarUrl}");
        return true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
