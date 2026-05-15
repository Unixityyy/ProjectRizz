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
    public string backendUrl = "https://api.unixityyy.dev/api/v1/attestation/";
    public bool attestation = false;

    [Header("COSMETICS")]
    public static PlayfabLogin instance;
    public string MyPlayFabID;
    public string CatalogName;
    public GameObject KickButtons;
    public GameObject FlingButtons;
    public List<GameObject> specialitems;
    public List<GameObject> disableitems;

    [Header("CURRENCY")]
    public string CurrencyName;
    public TextMeshPro currencyText;
    public int coins;

    [Header("BANNED")]
    public string bannedscenename;

    [Header("TITLE DATA")]
    public GameObject ModApps;
    public TextMeshPro MOTDText;

    [Header("PLAYER DATA")]
    public TextMeshPro UserName;
    public string Name;
    public TextMeshPro IDText;
    public string OculusID;
    public string OculusUserName;
    public string OculusDisplayName;
    private List<string> preLogItems = new List<string>();

    [Header("MISC")]
    public UnityEvent LoginEvent = new UnityEvent();
    public TextMeshPro CurrentServer;
    private bool hashed;
    private bool UsernameGot;
    private bool photonMetaSynced = false;
    private bool MetaAuthSuceed;

    public void ModCall(string Reason)
    {
        string jsonPayload = "{\"content\": \"<@&1355942327583248494> Player has called for mod! Reason: " + Reason + ", Player ID is " + MyPlayFabID + ".\", \"allowed_mentions\": {\"roles\": [\"1355942327583248494\"]}}";
        // this wont work btw cuz it dont exist
        UnityWebRequest www = new UnityWebRequest("https://discord.com/api/webhooks/1413736708775612416/OWgCe3UTPiphoKyLwb0miqhIbz0brl_jJ8_0Ks-Qr6MgjZaocMTJlL3LqkUk4rS_2bYm", "POST");
        byte[] jsonToSend = new UTF8Encoding().GetBytes(jsonPayload);
        www.uploadHandler = new UploadHandlerRaw(jsonToSend);
        www.downloadHandler = new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/json");

        www.SendWebRequest().completed += _ =>
        {
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Webhook Error: " + www.error);
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
        preLogItems.Clear();
        foreach (GameObject item in specialitems)
        {
            if (item != null && item.activeSelf)
            {
                preLogItems.Add(item.name);
            }
        }
        if (Core.IsInitialized())
        {
            Users.GetLoggedInUser().OnComplete(GetLoggedInUserCallback);
        }
        else
        {
            Core.AsyncInitialize().OnComplete(m => {
                if (!m.IsError) Users.GetLoggedInUser().OnComplete(GetLoggedInUserCallback);
            });
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
            MOTDText.text = "AUTH FAILED";
        }
    }

    private void OnEntitlementCheckComplete(Message msg)
    {
        if (msg.IsError)
        {
            MOTDText.text = "NOT ENTITLED";
        }
        else
        {
            MetaAuthSuceed = true;
            StartSecureAttestation();
        }
    }

    private void StartSecureAttestation()
    {
        string nonce = System.Guid.NewGuid().ToString();
        Oculus.Platform.DeviceApplicationIntegrity.GetIntegrityToken(nonce).OnComplete(message => {
            if (!message.IsError) {
                StartCoroutine(SecureLoginRoutine(message.Data));
            } else {
                MOTDText.text = "ATTESTATION FAILED";
                Debug.LogError($"Integ err: {message.GetError().Message}");
            }
        });
    }

    IEnumerator SecureLoginRoutine(string attestationToken)
    {
        if (attestation) {
            string hwId = SystemInfo.deviceUniqueIdentifier;
            string jsonPayload = "{\"attestationToken\":\"" + attestationToken + "\", \"hardwareId\":\"" + hwId + "\"}";
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonPayload);

            using (UnityWebRequest request = new UnityWebRequest(backendUrl, "POST"))
            {
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");
                request.certificateHandler = new BypassCertificate();

                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    var response = JsonUtility.FromJson<SecureLoginResponse>(request.downloadHandler.text);
                    
                    PlayFabSettings.staticPlayer.ClientSessionTicket = response.sessionTicket;
                    MyPlayFabID = response.playFabId;
                    
                    HandlePostLogin();
                }
                else
                {
                    MOTDText.text = "LOGIN FAILED";
                    Debug.LogError("Attestation Error: " + request.error);
                }
            }
        } else
        {
            var request = new LoginWithCustomIDRequest
            {
                CustomId = SystemInfo.deviceUniqueIdentifier,
                CreateAccount = true,
                InfoRequestParameters = new GetPlayerCombinedInfoRequestParams
                {
                    GetPlayerProfile = true
                }
            };

            PlayFabClientAPI.LoginWithCustomID(request, OnLoginWithCustomIDSuccess, OnPlayFabError);
            yield break;
        }
    }

    private void OnLoginWithCustomIDSuccess(LoginResult result)
    {
        MyPlayFabID = result.PlayFabId;
        PlayFabSettings.staticPlayer.ClientSessionTicket = result.SessionTicket;
        HandlePostLogin();
    }

    private void HandlePostLogin()
    {
        if (preLogItems.Count > 0)
        {
            dumbbitch();
        }

        LoginEvent.Invoke();
        GetVirtualCurrencies();
        GetMOTD();
        UpdatePlayFabMetaInfo();

        PlayFabClientAPI.GetAccountInfo(new GetAccountInfoRequest(), result => {
            string username = result.AccountInfo.TitleInfo.DisplayName;
            if (string.IsNullOrEmpty(username))
            {
                string newUsername = "RIZZ" + Random.Range(0, 9999).ToString("D4");
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

    void ConnectToPhoton(GetPhotonAuthenticationTokenResult result)
    {
        if (MetaAuthSuceed && !string.IsNullOrEmpty(MyPlayFabID))
        {
            AuthenticationValues authValues = new AuthenticationValues();
            authValues.AuthType = CustomAuthenticationType.Custom;
            authValues.AddAuthParameter("username", MyPlayFabID);
            authValues.AddAuthParameter("token", result.PhotonCustomAuthenticationToken);

            PhotonNetwork.AuthValues = authValues;
            if (PunVoiceClient.Instance != null) PunVoiceClient.Instance.Client.AuthValues = authValues;

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
                    if (item.ItemId == "Moderator") { KickButtons.SetActive(true); FlingButtons.SetActive(true); }
                }
            }
        }, OnError);
    }

    public void GetMOTD()
    {
        PlayFabClientAPI.GetTitleData(new GetTitleDataRequest(), result =>
        {
            if (result.Data != null && result.Data.ContainsKey("MOTD")) MOTDText.text = result.Data["MOTD"];
            if (result.Data.ContainsKey("ModApps")) ModApps.SetActive(bool.TryParse(result.Data["ModApps"], out bool isEnabled) && isEnabled);
        }, OnError);
    }

    void UpdatePlayFabMetaInfo()
    {
        PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest {
            Data = new Dictionary<string, string> {
                { "MetaUsername", OculusUserName },
                { "MetaDisplayName", OculusDisplayName }
            }
        }, null, null);
        UsernameGot = true;
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

    public override void OnJoinedRoom()
    {
        if (PhotonNetwork.IsConnectedAndReady && UsernameGot)
        {
            ExitGames.Client.Photon.Hashtable customProps = new ExitGames.Client.Photon.Hashtable();
            customProps["MetaUsername"] = OculusUserName;
            customProps["MetaDisplayName"] = OculusDisplayName;
            PhotonNetwork.LocalPlayer.SetCustomProperties(customProps);
        }
        CurrentServer.text = $"CURRENT SERVER: {PhotonNetwork.CurrentRoom.Name}";
    }

    public override void OnLeftRoom() => CurrentServer.text = $"CURRENT SERVER: NONE";

    private void OnError(PlayFabError error)
    {
        if (error.Error == PlayFabErrorCode.AccountBanned) SceneManager.LoadScene(bannedscenename);
    }
    private void OnPlayFabError(PlayFabError error) => Debug.LogError(error.GenerateErrorReport());

    [System.Serializable]
    public class SecureLoginResponse
    {
        public string sessionTicket;
        public string playFabId;
    }
}