using Oculus.Platform;
using Oculus.Platform.Models;
using Photon.Pun;
using Photon.Realtime;
using Photon.VR;
using Photon.VR.Player;
using PlayFab;
using PlayFab.ClientModels;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

    public class PlayfabLogin : MonoBehaviourPunCallbacks
    {
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
        public string StartingUsername;
        public string Name;
        [SerializeField]
        public bool UpdateName;
        private bool hashed;
        public TextMeshPro IDText;
        public UnityEvent LoginEvent = new UnityEvent();
        public string OculusID;
        public string OculusUserName;
        public string OculusDisplayName;
        public bool MetaAuthSuceed;

        private string playFabTicket; // Store PlayFab authentication ticket

        public void Awake()
        {
            DontDestroyOnLoad(gameObject);
            MetaAuthSuceed = false;
            instance = this;
        }

        void Start()
        {
            if (Core.IsInitialized())
            {
                Users.GetLoggedInUser().OnComplete(GetLoggedInUserCallback);
            }
        }

        public void login()
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
            PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnError);
        }

        public void OnLoginSuccess(LoginResult result)
        {
            LoginEvent.Invoke();
            Debug.Log("Logging into PlayFab...");
            playFabTicket = result.SessionTicket; // Store session ticket for Photon authentication
            GetAccountInfoRequest InfoRequest = new GetAccountInfoRequest();
            PlayFabClientAPI.GetAccountInfo(InfoRequest, AccountInfoSuccess, OnError);
            GetVirtualCurrencies();
            GetMOTD();
            var request = new UpdateUserDataRequest
            {
                Data = new Dictionary<string, string> {
                        { "MetaUsername", OculusUserName },
                        { "MetaDisplayName", OculusDisplayName }
                    }
            };
            PlayFabClientAPI.UpdateUserData(request, OnDataUpdateSuccess, OnDataUpdateFailure);
            Users.GetUserProof().OnComplete(OnUserProofCallback);
    }

        void ConnectToPhoton(GetPhotonAuthenticationTokenResult result)
        {
            // var customAuth = new AuthenticationValues { AuthType = CustomAuthenticationType.Custom };
            // customAuth.AddAuthParameter("username", MyPlayFabID);
            // customAuth.AddAuthParameter("token", result.PhotonCustomAuthenticationToken);
            // PhotonNetwork.AuthValues = customAuth;
            if (MetaAuthSuceed && MyPlayFabID != "") {
                PhotonNetwork.ConnectUsingSettings();
            }
        }

        public void OnUpdateUsernameFailure(PlayFabError error)
        {
            UnityEngine.Application.Quit();
        }

        public void AccountInfoSuccess(GetAccountInfoResult result)
        {
            string username = result.AccountInfo.Username;
            Debug.Log("name: " + username);
            MyPlayFabID = result.AccountInfo.PlayFabId;
            PlayFabClientAPI.GetPhotonAuthenticationToken(new GetPhotonAuthenticationTokenRequest()
            {
                PhotonApplicationId = PhotonNetwork.PhotonServerSettings.AppSettings.AppIdRealtime
            }, ConnectToPhoton, OnPlayFabError);
            if (string.IsNullOrEmpty(username))
            {
                Debug.Log("EMPTY USERNAME, I REPEAT, EMPTY USERNAME");
                // If username is null or empty, set it to "RIZZ" + 4 random numbers
                string numbers = null;
                for (int i = 0; i < 4; i++)
                {
                    numbers = numbers + Random.Range(0, 100);
                }
                string newUsername = "RIZZ" + numbers;
                UpdateUserTitleDisplayNameRequest request = new UpdateUserTitleDisplayNameRequest { DisplayName = newUsername };
                PlayFabClientAPI.UpdateUserTitleDisplayName(request, OnUpdateUsernameSuccess, OnUpdateUsernameFailure);
                PhotonVRManager.SetUsername(newUsername);
            }
            else
            {
                Debug.Log("no pawblem found lil ked");
            }

            PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(),
            (result) =>
            {
                foreach (var item in result.Inventory)
                {
                    if (item.CatalogVersion == CatalogName)
                    {
                        for (int i = 0; i < specialitems.Count; i++)
                        {
                            if (specialitems[i].name == item.ItemId)
                            {
                                specialitems[i].SetActive(true);
                            }
                        }
                        for (int i = 0; i < disableitems.Count; i++)
                        {
                            if (disableitems[i].name == item.ItemId)
                            {
                                disableitems[i].SetActive(false);
                            }
                        }
                        for (int i = 0; i < 1; i++)
                        {
                            if (item.ItemId == "Vents")
                            {
                                KickButtons.SetActive(true);
                            }
                        }
                    }
                }
            },
            (error) =>
            {
                Debug.LogError(error.GenerateErrorReport());
            });
        }

        private void GetLoggedInUserCallback(Message<User> msg)
        {
            if (!msg.IsError)
            {
                OculusID = msg.Data.ID.ToString();
                User user = msg.GetUser();
                OculusUserName = user.OculusID.ToString();
                OculusDisplayName = user.DisplayName.ToString();
                Entitlements.IsUserEntitledToApplication().OnComplete(OnEntitlementCheckComplete);
                login();
            }
            else
            {
                MOTDText.text = "FAILED TO AUTHENTICATE WITH META HORIZON! PLEASE REJOIN!";
            }
        }

        public override void OnConnectedToMaster()
        {
            base.OnConnectedToMaster();
            PhotonVRManager.SetUsername(Name); 
        }

        private void OnEntitlementCheckComplete(Message msg)
        {
            if (msg.IsError)
            {
                MOTDText.text = "FAILED TO AUTHENTICATE WITH META HORIZON! PLEASE REJOIN!";
            }
            else
            {
                MetaAuthSuceed = true;
            }
        }

        private void OnUserProofCallback(Message<UserProof> msg)
        {
            if (!msg.IsError)
            {
                // meta auth
                // string oculusNonce = msg.Data.Value.ToString();
                // Debug.Log("Oculus nonce: " + oculusNonce);
                // PhotonNetwork.AuthValues = new AuthenticationValues();
                // PhotonNetwork.AuthValues.UserId = OculusID;
                // PhotonNetwork.AuthValues.AuthType = CustomAuthenticationType.Oculus;
                // PhotonNetwork.AuthValues.AddAuthParameter("userid", OculusID);
                // PhotonNetwork.AuthValues.AddAuthParameter("nonce", oculusNonce);
                // PhotonNetwork.ConnectUsingSettings();
            }
        }

        void OnDataUpdateSuccess(UpdateUserDataResult result)
        {
            Debug.Log("Successfully updated player data with Meta username.");
        }

        private void OnUpdateUsernameSuccess(UpdateUserTitleDisplayNameResult result) { }

        void OnDataUpdateFailure(PlayFabError error)
        {
            Debug.LogError("Error updating player data: " + error.GenerateErrorReport());
            UnityEngine.Application.Quit();
        }

        public void GetVirtualCurrencies()
        {
            PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(), OnGetUserInventorySuccess, OnError);
        }

        void OnGetUserInventorySuccess(GetUserInventoryResult result)
        {
            coins = result.VirtualCurrency["RT"];
            currencyText.text = "YOU HAVE " + coins.ToString() + " " + CurrencyName;
        }

        private void OnError(PlayFabError error)
        {
            if (error.Error == PlayFabErrorCode.AccountBanned)
            {
                SceneManager.LoadScene(bannedscenename);
            }
            else
            {
                login();
            }
        }

        public void GetMOTD()
        {
            PlayFabClientAPI.GetTitleData(new GetTitleDataRequest(), MOTDGot, OnError);
        }

        public void MOTDGot(GetTitleDataResult result)
        {
            if (result.Data == null || result.Data.ContainsKey("MOTD") == false)
            {
                Debug.Log("No MOTD");
                return;
            }
            MOTDText.text = result.Data["MOTD"];

        }

        private void Update()
        {
            if (PhotonNetwork.IsConnected && !hashed)
            {
                if (MyPlayFabID != "" && MyPlayFabID != " ")
                {
                    IDText.text = MyPlayFabID;
                    ExitGames.Client.Photon.Hashtable hash = PhotonNetwork.LocalPlayer.CustomProperties;
                    hash["PlayfabID"] = MyPlayFabID;
                    PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
                    hashed = true;
                }
            }
        }
        private void OnPlayFabError(PlayFabError error)
        {
            Debug.LogError($"PlayFab Error ({error.Error}): {error.ErrorMessage}");
        }

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
    }