using Oculus.Platform;
using Oculus.Platform.Models;
using Photon.Pun;
using Photon.Realtime;
using Photon.VR;
using PlayFab;
using PlayFab.ClientModels;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using Photon.Voice;
using Photon.Voice.Unity;
using Photon.Voice.PUN;

public class BypassCertificate : CertificateHandler
{
    protected override bool ValidateCertificate(byte[] certificateData)
    {
        return true;
    }
}

public class PlayfabLogin : MonoBehaviourPunCallbacks
{
    [Header("SETTINGS")]
    public bool useAttestation = false;

    [Header("COSMETICS")]
    public static PlayfabLogin instance;
    public string MyPlayFabID;
    public string CatalogName;
    public GameObject KickButtons;
    public List<GameObject> specialitems;
    public List<GameObject> disableitems;
    [Header("CURRENCY")]
    public string CurrencyName;
    public TextMeshPro currencyText;
    [SerializeField]
    public int coins;
    [Header("BANNED")]
    public string bannedscenename;
    [Header("TITLE DATA")]
    public TextMeshPro MOTDText;
    [Header("PLAYER DATA")]
    public TextMeshPro UserName;
    [SerializeField] public bool UpdateName;
    public string StartingUsername;
    public string Name;
    public TextMeshPro IDText;
    public UnityEvent LoginEvent = new UnityEvent();
    public string OculusID;
    public string OculusUserName;
    public string OculusDisplayName;
    public bool MetaAuthSuceed;

    private string playFabTicket;
    private bool hashed;
    private bool UsernameGot;
    private bool photonMetaSynced = false;
    private string backendUrl = "https://dzhkj7xsbckrhwodmsxllguc2i0cxxqs.lambda-url.us-east-1.on.aws/";

    public void ModCall(string Reason)
    {
        string jsonPayload = "{\"content\": \"<@&1355942327583248494> Player has called for mod! Reason: " + Reason + ", Player ID is " + MyPlayFabID + ".\", \"allowed_mentions\": {\"roles\": [\"1355942327583248494\"]}}";

        UnityWebRequest www = new UnityWebRequest("https://discord.com/api/webhooks/1413736708775612416/OWgCe3UTPiphoKyLwb0miqhIbz0brl_jJ8_0Ks-Qr6MgjZaocMTJlL3LqkUk4rS_2bYm", "POST");
        byte[] jsonToSend = new UTF8Encoding().GetBytes(jsonPayload);
        www.uploadHandler = new UploadHandlerRaw(jsonToSend);
        www.downloadHandler = new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/json");

        www.SendWebRequest().completed += _ =>
        {
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Reporting Webhook Error: " + www.error);
            }
        };
    }

    public void Awake()
    {
        DontDestroyOnLoad(gameObject);
        MetaAuthSuceed = false;
        UsernameGot = false;
        instance = this;
    }

    void Start()
    {
        if (Core.IsInitialized())
        {
            Users.GetLoggedInUser().OnComplete(GetLoggedInUserCallback);
        }
    }

    private void GetLoggedInUserCallback(Message<User> msg)
    {
        if (!msg.IsError)
        {
            OculusID = msg.Data.ID.ToString();
            OculusUserName = msg.Data.OculusID;
            OculusDisplayName = msg.Data.DisplayName;
            Entitlements.IsUserEntitledToApplication().OnComplete(OnEntitlementCheckComplete);
        }
        else
        {
            MOTDText.text = "FAILED TO AUTHENTICATE WITH META!";
        }
    }

    void LoginWithCustomID()
    {
        var request = new LoginWithCustomIDRequest
        {
            CustomId = SystemInfo.deviceUniqueIdentifier,
            CreateAccount = true
        };
        PlayFabClientAPI.LoginWithCustomID(request, OnCustomIDLoginSuccess, OnError);
    }

    private void OnEntitlementCheckComplete(Message msg)
    {
        if (msg.IsError)
        {
            MOTDText.text = "NOT ENTITLED TO APP!";
        }
        else
        {
            MetaAuthSuceed = true;
            LoginWithCustomID(); 
        }
    }

    private void OnCustomIDLoginSuccess(LoginResult result)
    {
        playFabTicket = result.SessionTicket;
        MyPlayFabID = result.PlayFabId;

        if (useAttestation)
        {
#if UNITY_STANDALONE_ANDROID && !UNITY_EDITOR
            string nonce = System.Guid.NewGuid().ToString();

            Oculus.Platform.DeviceApplicationIntegrity.GetIntegrityToken(nonce).OnComplete(message => {
                if (!message.IsError) {
                    string integrityToken = message.Data;
                    Debug.Log("got integ token");
                    StartCoroutine(SecureLoginRoutine(integrityToken));
                } else {
                    var error = message.GetError();
                    Debug.LogError($"integ err: {error.Code} - {error.Message}");
                    MOTDText.text = "SECURITY CHECK FAILED";
                }
            });
#elif UNITY_EDITOR
            HandlePostLogin();
#endif
        } else
        {
            HandlePostLogin();
        }
    }

    IEnumerator SecureLoginRoutine(string attestationToken)
    {
        string jsonPayload = "{\"attestation_token\":\"" + attestationToken + "\", \"playfab_id\":\"" + MyPlayFabID + "\"}";
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonPayload);

        using (UnityWebRequest request = new UnityWebRequest(backendUrl, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("attestation success");

                HandlePostLogin();
            }
            else
            {
                MOTDText.text = "META HORIZON ATTESTATION HAS FAILED. PLEASE RESTART THE GAME. IF THIS KEEPS HAPPENING REINSTALL THE GAME.";
                Debug.LogError("attestation fail: " + request.error);
            }
        }
    }

    public override void OnCreateRoomFailed(short returnCode, string message) => Debug.LogError($"Create Room Failed: {message}");
    public override void OnJoinRoomFailed(short returnCode, string message) => Debug.LogError($"Join Room Failed: {message}");

    private void HandlePostLogin()
    {
        LoginEvent.Invoke();
        GetVirtualCurrencies();
        GetMOTD();
        UpdatePlayFabMetaInfo();

        PlayFabClientAPI.GetAccountInfo(new GetAccountInfoRequest(), result => {
            string username = result.AccountInfo.TitleInfo.DisplayName;
            if (string.IsNullOrEmpty(username))
            {
                string numbers = Random.Range(0, 9999).ToString("D4");
                string newUsername = "RIZZ" + numbers;
                PlayFabClientAPI.UpdateUserTitleDisplayName(new UpdateUserTitleDisplayNameRequest { DisplayName = newUsername }, null, null);
                PhotonVRManager.SetUsername(newUsername);
            }
            else
            {
                PhotonVRManager.SetUsername(username);
            }
        }, OnPlayFabError);

        PlayFabClientAPI.GetPhotonAuthenticationToken(new GetPhotonAuthenticationTokenRequest()
        {
            PhotonApplicationId = PhotonNetwork.PhotonServerSettings.AppSettings.AppIdRealtime
        }, ConnectToPhoton, OnPlayFabError);
    }

    public override void OnJoinedRoom()
    {
        // 99.99% is connected and ready but a little check never hurt nobody
        if (PhotonNetwork.IsConnectedAndReady && UsernameGot)
        {
            ExitGames.Client.Photon.Hashtable customProps = new ExitGames.Client.Photon.Hashtable();
            
            customProps["MetaUsername"] = OculusUserName;
            customProps["MetaDisplayName"] = OculusDisplayName;
            
            PhotonNetwork.LocalPlayer.SetCustomProperties(customProps);
        }
    }

    void UpdatePlayFabMetaInfo()
    {
        var request = new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string> {
                { "MetaUsername", OculusUserName },
                { "MetaDisplayName", OculusDisplayName }
            }
        };
        PlayFabClientAPI.UpdateUserData(request, null, null);

        UsernameGot = true;
    }

    void ConnectToPhoton(GetPhotonAuthenticationTokenResult result)
    {
        if (MetaAuthSuceed && !string.IsNullOrEmpty(MyPlayFabID))
        {
            AuthenticationValues authValues = new AuthenticationValues();
            authValues.AuthType = CustomAuthenticationType.Custom;

            authValues.AddAuthParameter("username", MyPlayFabID);
            authValues.AddAuthParameter("token", result.PhotonCustomAuthenticationToken);

            PhotonNetwork.AuthValues = authValues;

            if (PunVoiceClient.Instance != null)
            {
                PunVoiceClient.Instance.Client.AuthValues = authValues;
            }

            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public void GetVirtualCurrencies()
    {
        PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(), result =>
        {
            coins = result.VirtualCurrency.ContainsKey("RT") ? result.VirtualCurrency["RT"] : 0;
            currencyText.text = "YOU HAVE " + coins.ToString() + " " + CurrencyName;

            foreach (var item in result.Inventory)
            {
                if (item.CatalogVersion == CatalogName)
                {
                    foreach (var sItem in specialitems) if (sItem.name == item.ItemId) sItem.SetActive(true);
                    foreach (var dItem in disableitems) if (dItem.name == item.ItemId) dItem.SetActive(false);
                    if (item.ItemId == "Moderator") KickButtons.SetActive(true);
                }
            }
        }, OnError);
    }

    public void GetMOTD()
    {
        PlayFabClientAPI.GetTitleData(new GetTitleDataRequest(), result =>
        {
            if (result.Data != null && result.Data.ContainsKey("MOTD"))
            {
                MOTDText.text = result.Data["MOTD"];
            }
        }, OnError);
    }

    private void Update()
    {
        if (PhotonNetwork.IsConnected && !hashed && !string.IsNullOrEmpty(MyPlayFabID))
        {
            IDText.text = MyPlayFabID;
            ExitGames.Client.Photon.Hashtable hash = PhotonNetwork.LocalPlayer.CustomProperties;
            hash["PlayfabID"] = MyPlayFabID;
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
            hashed = true;
        }

        if (PhotonNetwork.InRoom && UsernameGot && !photonMetaSynced)
        {
            ExitGames.Client.Photon.Hashtable metaHash = new ExitGames.Client.Photon.Hashtable();
            metaHash["MetaUsername"] = OculusUserName;
            metaHash["MetaDisplayName"] = OculusDisplayName;
            
            PhotonNetwork.LocalPlayer.SetCustomProperties(metaHash);
            photonMetaSynced = true;
        }
    }

    private void OnError(PlayFabError error)
    {
        if (error.Error == PlayFabErrorCode.AccountBanned)
            SceneManager.LoadScene(bannedscenename);
    }

    private void OnPlayFabError(PlayFabError error) => Debug.LogError(error.GenerateErrorReport());

    [System.Serializable]
    public class SecureLoginResponse
    {
        public string SessionTicket;
        public string PlayFabId;
    }
}