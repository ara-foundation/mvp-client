using Lean.Gui;
using System;
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
    private User _user;

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
        ShowProfileThumb(_chat);
    }

    public void Show(User user)
    {
        _user = user;

        var chatType = TelegramChat.ChatType.Direct;
        ChatTypes[(int)chatType].TurnOn();

        Title.text = user.first_name  + " " + user.last_name;
        Username.gameObject.SetActive(true);
        Username.text = $"@{user.username}";
        ShowProfileThumb(user);
    }

    async void ShowProfileThumb(ChatBase _chat)
    {
        if (_chat.Photo == null)
        {
            ProfileThumb.gameObject.SetActive(false);
            return;
        }
        DefaultThumb.gameObject.SetActive(false);
        ProfileThumb.gameObject.SetActive(true);

        var width = (int)ProfileThumb.rectTransform.rect.width;
        var height = (int)ProfileThumb.rectTransform.rect.height;
        Texture2D tex = await TelegramChat.Instance.FetchPhoto(_chat, width, height);
        ProfileThumb.texture = tex;
    }

    async void ShowProfileThumb(User user)
    {
        if (user.photo == null)
        {
            DefaultThumb.gameObject.SetActive(true);
            ProfileThumb.gameObject.SetActive(false);
            return;
        }
        DefaultThumb.gameObject.SetActive(false);
        ProfileThumb.gameObject.SetActive(true);

        var width = (int)ProfileThumb.rectTransform.rect.width;
        var height = (int)ProfileThumb.rectTransform.rect.height;
        Texture2D tex = await TelegramChat.Instance.FetchPhoto(user, width, height);
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
