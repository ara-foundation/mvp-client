using Lean.Gui;
using Newtonsoft.Json;
using Org.BouncyCastle.Security;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Thirdweb;
using Thirdweb.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WalletConnectSharp.Core.Controllers;

/*Defined in act-server/src/handlers/users.ts*/
[Serializable]
public class UserCreate {
    public string username;
    public string password;
    public string email;
}

/* Defined in npm://@ara-foundation/flarum-js-client/types */
[Serializable]
public class CreateSessionToken {
    public string token;
}

/* Not defined anywhere but I use this format */
[Serializable]
public class ErrorResponse
{
    public string message;
}

[Serializable]
public class UserParams
{
    // Latest logged in user name;
    public static readonly string LatestUsernameKey = "ara_last_logged_username";
    private static readonly string UserKeyPrefix = "ara_user_";

    public UserCreate loginParams;
    public string token;
    public int tokenExpiry;
    public string privateKey;
    public string address;

    public static UserParams Load()
    {
        if (!PlayerPrefs.HasKey(LatestUsernameKey))
        {
            return null;
        }

        return Load(PlayerPrefs.GetString(LatestUsernameKey));
    }

    /** Save the object as latest 
     */
    public static void Save(UserParams userParams)
    {
        PlayerPrefs.SetString(LatestUsernameKey, userParams.loginParams.username);
        var value = JsonConvert.SerializeObject(userParams);
        var key = UserKeyPrefix + userParams.loginParams.username;
        PlayerPrefs.SetString(key, value);
    }

    public static UserParams Load(string username)
    {
        var userKey = UserKeyPrefix + username;
        if (!PlayerPrefs.HasKey(userKey))
        {
            return null;
        }

        var userRaw = PlayerPrefs.GetString(userKey);
        UserParams userParams;
        try
        {
            userParams = JsonConvert.DeserializeObject<UserParams>(userRaw);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            return null;
        }

        return userParams;
    }

    public bool IsTokenActive()
    {
        return tokenExpiry > UniversalTime.Now();
    }
}

public class AraAuth : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI LoginText;
    [SerializeField]
    private LeanWindow LoginModal;
    [SerializeField] private LeanWindow SignupModal;
    [SerializeField] private LeanWindow ProfileModal;
    private static IThirdwebWallet Wallet;
    private static readonly string DefaultLoginText = "Login";

    [HideInInspector]
    public UserParams UserParams = null;

    [SerializeField] private TMP_InputField LoginUsername;
    [SerializeField] private TMP_InputField LoginPassword;
    [SerializeField] private Button LoginButton;
    [SerializeField] private TMP_InputField SignupUsername;
    [SerializeField] private TMP_InputField SignupEmail;
    [SerializeField] private TMP_InputField SignupPassword;
    [SerializeField] private Button SignupButton;
    [SerializeField] private TMP_InputField ProfileUsername;
    [SerializeField] private TMP_InputField ProfileEmail;
    [SerializeField] private TMP_InputField ProfilePassword;
    [SerializeField] private TMP_InputField ProfileAddress;
    [SerializeField] private TMP_InputField ProfilePrivateKey;

    // Start is called before the first frame update
    async void Start()
    {
        UserParams = await AutoLogin();
        Debug.Log("Auto Logged in? " + UserParams != null);
        if (IsLoggedIn(UserParams))
        {
            LoginText.text = UserParams.loginParams.username;
        } else
        {
            LoginText.text = DefaultLoginText;
        }
    }

    private bool IsLoggedIn(UserParams userParams)
    {
        return userParams != null &&
            userParams.loginParams != null &&
            userParams.loginParams.username != null &&
            userParams.loginParams.username.Length > 0;
    }

    public void OnClick()
    {
        if (IsLoggedIn(UserParams))
        {
            ShowProfileModal();
        } else
        {
            ShowLoginModal();
        }
    }
    public void ShowProfileModal()
    {
        if (ProfileModal != null)
        {
            ProfileUsername.text = UserParams.loginParams.username;
            ProfileEmail.text = UserParams.loginParams.email;
            ProfilePassword.text = UserParams.loginParams.password;
            ProfileAddress.text = UserParams.address;
            ProfilePrivateKey.text = UserParams.privateKey;

            ProfileModal.TurnOffOthersNow();
            ProfileModal.TurnOn();
        }
    }

    public void ShowSignupModal()
    {
        if ( SignupModal != null )
        {
            SignupModal.TurnOffOthersNow();
            SignupModal.TurnOn();
        }
    }

    public void ShowLoginModal()
    {
        if (LoginModal != null)
        {
            LoginModal.TurnOffOthersNow();
            LoginModal.TurnOn();
        }
    }

    public async void OnSignup()
    {
        var foundUserParams = UserParams.Load(SignupUsername.text);
        if (foundUserParams != null)
        {
            Notification.Instance.Show($"User already signed up. login as '{SignupUsername.text}'");
            return;
        }

        SignupButton.interactable = false;

        var userCreate = new UserCreate
        {
            email = SignupEmail.text,
            username = SignupUsername.text,
            password = SignupPassword.text
        };
        var userParams = new UserParams
        {
            loginParams = userCreate
        };

        

        userParams = await Signup(userParams);
        SignupButton.interactable = true;

        if (userParams != null)
        {
            UserParams = userParams;
            LoadWallet();
            UserParams.Save(userParams);
            LoginText.text = UserParams.loginParams.username;

            Notification.Instance.Show("Successfully signed up!");

            SignupModal.TurnOffSiblingsNow();
            SignupModal.TurnOff();
        }
    }

    public async void OnLogin()
    {
        LoginButton.interactable = false;

        var userCreate = new UserCreate
        {
            email = "",
            username = LoginUsername.text,
            password = LoginPassword.text
        };
        var userParams = new UserParams
        {
            loginParams = userCreate
        };

        var foundUserParams = UserParams.Load(LoginUsername.text);
        if (foundUserParams != null)
        {
            userParams = foundUserParams;
        }

        userParams = await Login(userParams);
        LoginButton.interactable = true;

        if (userParams != null)
        {
            UserParams = userParams;
            LoadWallet();
            UserParams.Save(userParams);
            LoginText.text = UserParams.loginParams.username;

            Notification.Instance.Show("Successfully logged in!");

            SignupModal.TurnOffSiblingsNow();
            SignupModal.TurnOff();
        }
    }


    public void OnLogout()
    {
        ThirdwebManager.Instance.RemoveWallet(UserParams.address);
        UserParams = null;
        LoginText.text = DefaultLoginText;
        PlayerPrefs.DeleteKey(UserParams.LatestUsernameKey);

        SignupModal.TurnOffSiblingsNow();
        SignupModal.TurnOff();
    }

    private async Task<UserParams> Login(UserParams userParams)
    {
        var body = JsonUtility.ToJson(userParams.loginParams);
        string url = NetworkParams.AraActUrl + "/users/login";

        string res;
        try
        {
            res = await WebClient.Post(url, body);
        }
        catch (Exception ex)
        {
            Notification.Instance.Show($"Error: web client exception {ex.Message}");
            Debug.LogError(ex);
            return null;
        }

        CreateSessionToken result;
        try
        {
            result = JsonConvert.DeserializeObject<CreateSessionToken>(res);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            Notification.Instance.Show($"Error: deserialization exception {e.Message}");
            return null;
        }

        var expiry = UniversalTime.Now() + 3600;
        LoginButton.interactable = true;

        userParams.token = result.token;
        userParams.tokenExpiry = (int)expiry;

        return userParams;
    }

    private async Task<UserParams> Signup(UserParams userParams)
    {
        var body = JsonUtility.ToJson(userParams.loginParams);
        string url = NetworkParams.AraActUrl + "/users";

        string res;
        try
        {
            res = await WebClient.Post(url, body);
        }
        catch (Exception ex)
        {
            Notification.Instance.Show($"Error: web client exception {ex.Message}");
            Debug.LogError(ex);
            return null;
        }

        CreateSessionToken result;
        try
        {
            result = JsonConvert.DeserializeObject<CreateSessionToken>(res);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            Notification.Instance.Show($"Error: deserialization exception {e.Message}");
            return null;
        }

        var expiry = UniversalTime.Now() + 3600;
        SignupButton.interactable = true;

        userParams.token = result.token;
        userParams.tokenExpiry = (int)expiry;

        return userParams;
    }

    void OnDisable()
    {
        if (IsLoggedIn(UserParams)) 
            ThirdwebManager.Instance.RemoveWallet(UserParams.address);
    }

    private async Task<UserParams> AutoLogin()
    {
        var userParams = UserParams.Load();
        if (!IsLoggedIn(userParams))
        {
            return null;
        }

        if (userParams.IsTokenActive()) {
            return userParams;
        }

        Notification.Instance.Show("Session expired, auto log in...");
        userParams = await Login(userParams);
        UserParams.Save(userParams);

        LoadWallet();

        return userParams;
    }

    private async void LoadWallet()
    {
        PrivateKeyWallet wallet;

        if (UserParams.privateKey == null || UserParams.privateKey.Length == 0)
        {
            wallet = await PrivateKeyWallet.Generate(ThirdwebManager.Instance.Client);
            UserParams.privateKey = await wallet.Export();

            Wallet = await ThirdwebManager.Instance.AddWallet(wallet);
            UserParams.address = await Wallet.GetAddress();

            UserParams.Save(UserParams);
        } else
        {
            wallet = await PrivateKeyWallet.Create(ThirdwebManager.Instance.Client, UserParams.privateKey);
            Wallet = await ThirdwebManager.Instance.AddWallet(wallet);
        }
    }
}
