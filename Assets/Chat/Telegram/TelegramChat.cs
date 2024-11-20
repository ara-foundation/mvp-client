using Lean.Gui;
using Org.BouncyCastle.Utilities.Zlib;
using QRCoder;
using QRCoder.Unity;
using System;
using System.IO;
using TL;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WalletConnectUnity.Modal.Views;
using WTelegram;

public class TelegramChat : MonoBehaviour
{
    private const string TG_SESSION_KEY = "tg_phone";
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

    readonly string DefaultComment = "Open Telegram on your phone. Go Settings -> Devices and scan qr code to login.";
    readonly string SingleSignOn = "Telegram doesn't allow login. Steps to do:" +
        "1. Disable the 2FA authentication in Telegram.";

    private Client client;
    private User user;

    void OnEnable()
    {
        ToggleLoginPanel(enabled: false);

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
                ShowProfile();
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

                ShowProfile();
            }
            catch (WTException e)
            {
                LoginComment.text = $"Login Error: {e.Message}";
            }
            
        }
    }

    async void ShowProfile()
    {
        ToggleProfilePanel(enabled: true);

        using MemoryStream photoStream = new();

        var storageType = await client.DownloadProfilePhotoAsync(user, photoStream);
        Debug.Log($"Photo filetype: {storageType}");

        Texture2D tex = new(100, 100);
        tex.LoadImage(photoStream.ToArray());
        ProfilePicture.texture = tex;

        Greetings.text = $"Hello, {user.first_name} {user.last_name}!";
        Username.text = $"@{user.username}";
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
