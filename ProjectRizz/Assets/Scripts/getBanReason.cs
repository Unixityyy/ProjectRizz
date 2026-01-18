using UnityEngine;
using TMPro;
using PlayFab;
using System;
using PlayFab.ClientModels;
using Photon.Pun;
using System.Globalization;

public class getBanReason : MonoBehaviour
{
    public TextMeshPro banText;

    // Start is called before the first frame update
    void Start()
    {
        // banText = this.GetComponent<TextMeshPro>();
        login();
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
        PlayFabClientAPI.LoginWithCustomID(request, OnSuccess, OnError);
    }

    void OnSuccess(LoginResult result)
    {
        // this.GetComponent<TextMeshPro>().text = "You aren't banned how the hell did you get here?";
        banText.text = "You aren't banned how the hell did you get here?";
    }

    void OnError(PlayFabError error)
    {

        Debug.Log("Error while logging in/creating account!");
        if (error.Error == PlayFabErrorCode.AccountBanned)
        {
            Debug.Log("band");
            foreach (var item in error.ErrorDetails)
            {
                    PhotonNetwork.Disconnect();
                    banText.text = "YOU HAVE BEEN BANNED FOR " + item.Key.ToString();

                    string unbanDateString = item.Value[0];
                    if (DateTime.TryParseExact(unbanDateString, "yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out DateTime unbanDate))
                    {
                        DateTime currentDate = DateTime.UtcNow;
                        TimeSpan timeRemaining = unbanDate - currentDate;
                        double hoursRemaining = Math.Abs(timeRemaining.TotalHours);
                        int hoursRemainingInt = (int)Math.Floor(hoursRemaining);
                        banText.text = banText.text + ", " + hoursRemainingInt.ToString() + " HOURS REMAIN.";
                    }
                    else
                    {
                    banText.text = "YOUR ACCOUNT HAS BEEN PERMANENTLY BANNED FOR " + item.Key.ToString();
                    banText.text = banText.text + ".";
                    }
                }
            return;
            }
        login();  
        }
    }
