using System;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MessagePresenter : MonoBehaviour
{
  public TMP_Text Nickname;
  public TMP_Text Content;
  public TMP_Text SendTime;

  public Button DeleteButton;
  public GameObject Tail;
  public GameObject PortraitObject;
  public RawImage PortraitImage;

  private const string TimeFormat = "DD/MM/YYYY HH:mm:ss";
  private readonly CultureInfo _cultureInfoProvider = new CultureInfo("ru-RU");
  public event Action<Message> OnMessageDelete;

  private Message _message;

  public Message Message
  {
    get => _message;
    set
    {
      _message = value;
      UpdatePresenter();
    }
  }

  private void Awake()
  {
    if(DeleteButton)
      DeleteButton.onClick.AddListener(OnDeleteButtonClick);
  }

  private void Reset() => 
    DeleteButton = GetComponentInChildren<Button>();

    private async void UpdatePresenter()
    {
        if (Message.Sender != null)
        {
            Nickname.SetText(Message.Sender.MainUsername);
            PortraitImage.texture = await TelegramChat.Instance.FetchPhoto(Message.Sender);
        } else
        {
            Nickname.SetText(Message.Owner.MainUsername);
            PortraitImage.texture = await TelegramChat.Instance.FetchPhoto(Message.Owner);
        }
        Content.SetText(Message.Content);
        SendTime.SetText(Message.SendTime.ToString(TimeFormat, _cultureInfoProvider));
    }

  private void OnDeleteButtonClick()
  {
    OnMessageDelete?.Invoke(_message);
  }

  public void Redraw(bool asLast)
  {
    Tail.SetActive(asLast);
    PortraitObject.SetActive(asLast);
  }
}