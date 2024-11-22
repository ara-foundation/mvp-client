using Lean.Gui;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TL;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WTelegram;

public class TelegramChat_ChatItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI Title;
    [SerializeField] private TextMeshProUGUI Username;
    [SerializeField] private RawImage ProfileThumb;
    [SerializeField] private RawImage DefaultThumb;
    [SerializeField] private List<LeanToggle> ChatTypes;

    private ChatBase _chat;

    public void Show(ChatBase chat)
    {
        _chat = chat;

        var chatType = ChatType(chat);
        ChatTypes[(int)chatType].TurnOn();

        Title.text = chat.Title;
        if (!string.IsNullOrEmpty(chat.MainUsername))
        {
            Username.gameObject.SetActive(true);
            Username.text = $"@{chat.MainUsername}";
        } else
        {
            Username.gameObject.SetActive(false);
        }
        ShowProfileThumb();
    }

    void ShowProfileThumb()
    {
        if (_chat.Photo == null || _chat.Photo.stripped_thumb == null)
        {
            DefaultThumb.gameObject.SetActive(true);
            ProfileThumb.gameObject.SetActive(false);
            Debug.Log($"The {_chat.Title} doesn't have a photo {_chat.Photo == null}");
            return;
        }
        DefaultThumb.gameObject.SetActive(false);
        ProfileThumb.gameObject.SetActive(true);

        var width = (int)ProfileThumb.rectTransform.rect.width;
        var height = (int)ProfileThumb.rectTransform.rect.height;
        Texture2D tex = new(5, 5);
        tex.LoadImage(_chat.Photo.stripped_thumb);

        ProfileThumb.texture = tex;
    }

    public static TelegramChat.ChatType ChatType(ChatBase chat)
    {
        if (chat.IsChannel)
        {
            return TelegramChat.ChatType.Broadcast;
        }
        else if (chat.IsGroup)
        {
            return TelegramChat.ChatType.Group;
        }
        return TelegramChat.ChatType.Direct;
    }
}
