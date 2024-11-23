using Lean.Gui;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Zlib;
using QRCoder;
using QRCoder.Unity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using TL;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WalletConnectUnity.Modal.Views;
using WTelegram;

public class TelegramChat : MonoBehaviour
{
    public enum ChatType
    {
        Direct = 0,
        Broadcast = 1,
        Group = 2,
        Sangha = 3,
    }

    private const string TG_SESSION_KEY = "tg_phone";
    private const string TG_CONTACT_KEY = "tg_contact";
    private const string TG_PHOTO_PREFIX = "tg_photo_";
    private const string TG_CONTACT_HASH_KEY = "tg_contact_hash";
    private const string API_HASH = "2100d1ef8459a3a22820652a2e6d1a86";
    private const int API_ID = 26296279;

    [SerializeField] private LeanWindow LoginPanel;
    [SerializeField] private LeanWindow Disable2FaPanel;
    [SerializeField] private LeanWindow ProfilePanel;
    [SerializeField] private TextMeshProUGUI LoginComment;
    [SerializeField] private RawImage QRCodeImage;
    [SerializeField] private RawImage ProfilePicture;
    [SerializeField] private TextMeshProUGUI Greetings;
    [SerializeField] private TextMeshProUGUI Username;
    [Space(10)]
    [Header("Chats")]
    [SerializeField] private TextMeshProUGUI ChatsTitle;
    [SerializeField] private Content ChatsContent;
    [SerializeField] private GameObject ChatItemPrefab;

    private List<TelegramChat_ChatItem> _chatItems = new();

    readonly string DefaultComment = "Open Telegram on your phone. Go Settings -> Devices and scan qr code to login.";
    readonly string SingleSignOn = "Telegram doesn't allow login. Steps to do:" +
        "1. Disable the 2FA authentication in Telegram.";

    private Client client;
    private User user;

    private static TelegramChat _instance;

    public static TelegramChat Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<TelegramChat>();
            }
            return _instance;
        }
    }

    void OnEnable()
    {
        ToggleLoginPanel(enabled: false);
        ChatsContent.Clear();

        Login();
    }

    public void OnLoginAgain()
    {
        ToggleDisable2FaPanel(enabled: false);
        Login();
    }

    async void Login()
    {
        client = new WTelegram.Client(API_ID, API_HASH);

        if (!PlayerPrefs.HasKey(TG_SESSION_KEY))
        {
            LoginComment.text = DefaultComment;
            try
            {
                user = await client.LoginWithQRCode(OnQrDisplay);
                PlayerPrefs.SetString(TG_SESSION_KEY, user.phone);
                Debug.Log($"Login session saved for '{user.phone}'");
            }
            catch (WTException e)
            {
                if (e.Message.Equals("You must provide a config value for password"))
                {
                    ToggleDisable2FaPanel(enabled: true);
                }
                else
                {
                    LoginComment.text = $"Login Error: {e.Message}";
                }
            }
        } else
        {
            try
            {
                if (client.User == null)
                {
                    var loggedIn = await client.Login(PlayerPrefs.GetString(TG_SESSION_KEY));
                    if (loggedIn != null)
                    {
                        Notification.Instance?.Show($"To resume a session need {loggedIn}");
                        Debug.LogError($"To resume a session need {loggedIn}");
                        return;
                    }
                }
                user = client.User;
            }
            catch (WTException e)
            {
                LoginComment.text = $"Login Error: {e.Message}";
            }
        }
        ShowProfile();
        LoadChats();
    }

    async void LoadContacts()
    {
        var contactHash = ContactHash();
        Debug.Log($"(hash={contactHash}) Fetching contact list...");

        var contacts = await client.Contacts_GetContacts(contactHash);

        if (contacts == null)
        {
            Debug.Log("Contact list was not modified");
            return;
        }
        ContactHash(contacts.contacts);

        Debug.Log($"(hash={contactHash}) There are {contacts.saved_count} contacts...");
        for( int i = 0; i < contacts.contacts.Length; i++)
        {
            var contact = contacts.contacts[i];
            Debug.Log($"{i+1}/{contacts.saved_count}: userId={contact.user_id}");
        }
    }

    /// <summary>
    /// https://wiz0u.github.io/WTelegramClient/#terminology
    /// </summary>
    async void LoadChats()
    {
        var dialogs = await client.Messages_GetAllDialogs();
        if (dialogs == null)
        {
            Debug.LogWarning("No dialogs found");
            return;
        }

        ChatsTitle.text = $"Chats ({dialogs.dialogs.Length})";
        foreach (var dialog in dialogs.dialogs)
        {
            switch (dialogs.UserOrChat(dialog))
            {
                case User user when user.IsActive: Console.WriteLine("User " + user); break;
                case ChatBase chat when chat.IsActive:
                    var chatItem = ChatsContent.Add<TelegramChat_ChatItem>(ChatItemPrefab);
                    chatItem.Show(chat);
                    _chatItems.Add(chatItem);
                    break;
            }
            
            
        }
    }

    long ContactHash()
    {
        return long.Parse(PlayerPrefs.GetString(TG_CONTACT_HASH_KEY, "0"));
    }

    void ContactHash(Contact[] contacts)
    {
        long hash = 0;
        foreach (var contact in contacts)
        {
            hash ^= contact.user_id >> 21;
            hash ^= contact.user_id << 35;
            hash ^= contact.user_id >> 4;
            hash += contact.user_id;
        }
        ContactHash(hash);
    }

    void ContactHash(long hash)
    {
        PlayerPrefs.SetString(TG_CONTACT_HASH_KEY, hash.ToString());
    }

    async void ShowProfile()
    {
        ToggleProfilePanel(enabled: true);

        ProfilePicture.texture = await FetchPhoto(user, _width: 100, _height: 100);
        Greetings.text = $"Hello, {user.first_name} {user.last_name}!";
        Username.text = $"@{user.username}";
    }

    public async Task<Texture2D> FetchPhoto(User _user, int _width = 100, int _height = 100)
    {
        Texture2D texture = new(_width, _height);
        
        var key = TG_PHOTO_PREFIX + _user.id;
        
        if (PlayerPrefs.HasKey(key))
        {
            var photoHex = PlayerPrefs.GetString(key);
            var photo = StringToByteArray(photoHex);
            texture.LoadImage(photo);
            return texture;
        }
        using MemoryStream photoStream = new();

        var storageType = await client.DownloadProfilePhotoAsync(_user, photoStream);
        Debug.Log($"Photo filetype: {storageType}");

        var photoBytes = photoStream.ToArray();
        PlayerPrefs.SetString(key, ByteArrayToString(photoBytes));
        
        texture.LoadImage(photoBytes);

        return texture;
    }

    public async Task<Texture2D> FetchPhoto(ChatBase _chat, int _width = 100, int _height = 100)
    {
        Texture2D texture = new(_width, _height);

        var key = TG_PHOTO_PREFIX + _chat.ID;

        if (PlayerPrefs.HasKey(key))
        {
            var photoHex = PlayerPrefs.GetString(key);
            var photo = StringToByteArray(photoHex);
            texture.LoadImage(photo);
            return texture;
        }
        using MemoryStream photoStream = new();

        var storageType = await client.DownloadProfilePhotoAsync(_chat, photoStream);
        Debug.Log($"Photo filetype: {storageType}");

        var photoBytes = photoStream.ToArray();
        PlayerPrefs.SetString(key, ByteArrayToString(photoBytes));

        texture.LoadImage(photoBytes);

        return texture;
    }

    public static string ByteArrayToString(byte[] ba)
    {
        return BitConverter.ToString(ba).Replace("-", "");
    }

    public static byte[] StringToByteArray(String hex)
    {
        int NumberChars = hex.Length;
        byte[] bytes = new byte[NumberChars / 2];
        for (int i = 0; i < NumberChars; i += 2)
            bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
        return bytes;
    }

    void ToggleProfilePanel(bool enabled)
    {
        ProfilePanel.Set(enabled);
    }

    void ToggleLoginPanel(bool enabled)
    {
        LoginPanel.Set(enabled);
    }

    void ToggleDisable2FaPanel(bool enabled)
    {
        Disable2FaPanel.Set(enabled);
    }

    void OnQrDisplay(string token)
    {
        ToggleLoginPanel(enabled: true);

        QRCodeGenerator qrGenerator = new QRCodeGenerator();
        QRCodeData qrCodeData = qrGenerator.CreateQrCode(token, QRCodeGenerator.ECCLevel.Q);
        UnityQRCode qrCode = new UnityQRCode(qrCodeData);
        Texture2D qrCodeAsTexture2D = qrCode.GetGraphic(20);

        QRCodeImage.texture = qrCodeAsTexture2D;
    }
}
