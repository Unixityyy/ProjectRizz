using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.VR;
using PlayFab;
using PlayFab.ClientModels;
using Photon.Pun;
using System.IO;
using System;
using UnityEngine.SceneManagement;

public class ExecuteButtonName : MonoBehaviour
{
    public NameScript nameScript;
    public string Handtag;
    public Sleep slep;
    private string oldName;
//     public void AddBan(string playerId, string name)
//     {
//     // Get the player's ban history
//     PlayFabServerAPI.GetUserBans(new GetUserBansRequest()
//     {
//         PlayFabId = playerId
//     }, result =>
//     {
//         // Calculate the duration of the ban based on the number of previous bans
//         uint hours = (uint)(result.BanData.Count + 1) * 24;

//         // Add the ban
//         PlayFabServerAPI.BanUsers(new BanUsersRequest()
//         {
//             Bans = new List<BanRequest>()
//             {
//                 new BanRequest()
//                 {
//                     DurationInHours = hours,
//                     PlayFabId = playerId,
//                     Reason = "Automatic ban for bad name:" + name + ". Use ur brain next time",
//                 }
//             }
//         }, banResult =>
//         {
//             // Handle success
//         }, error =>
//         {
//             Debug.Log(error.GenerateErrorReport());
//         });
//     }, error =>
//     {
//         Debug.Log(error.GenerateErrorReport());
//     });
// }
    private HashSet<string> badWords;
    private void Start()
    {
        badWords = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {"ASS", "FUCK", "NIGGA", "NIGGER", "SHIT", "KKK", "ANUS", "BREASTS", "TITS", "KNOCKERS", "COCK", "PENIS", "CUM"};

        // Assuming "bad_words.txt" is in the "Assets" directory
        
    }
    public bool IsBadWord(string word)
    {
        return badWords.Contains(word);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (nameScript.NameVar.Length < 3)
        {
            oldName = nameScript.NameVar;
            nameScript.NameVar = "Name must be more than 3 characters!";
            StartCoroutine(slep.SleepWait(2));
            nameScript.NameVar = oldName;
        }
        if (other.transform.tag == Handtag)
        {
            if (IsBadWord(nameScript.NameVar))
            {
                PlayerPrefs.SetString("username", nameScript.NameVar);
                if (PlayFabClientAPI.IsClientLoggedIn())
                {
                    PlayFabClientAPI.UpdateUserTitleDisplayName(new UpdateUserTitleDisplayNameRequest
                    {
                        DisplayName = PlayerPrefs.GetString("username")
                    }, delegate (UpdateUserTitleDisplayNameResult result)
                    {
                        Debug.Log("Display Name Changed!");
                    }, delegate (PlayFabError error)
                    {
                        Debug.Log("Error");
                        if (error.Error == PlayFabErrorCode.AccountBanned)
                        {
                            SceneManager.LoadScene("Bans");
                        }
                    });
                }
                nameScript.NameVar = "CENSORED";
                PlayerPrefs.SetString("username", nameScript.NameVar);
            }
            PhotonVRManager.SetUsername(nameScript.NameVar);
            PlayerPrefs.SetString("username", nameScript.NameVar);
            PhotonNetwork.LocalPlayer.NickName = nameScript.NameVar;

            if (PlayFabClientAPI.IsClientLoggedIn())
            {
                PlayFabClientAPI.UpdateUserTitleDisplayName(new UpdateUserTitleDisplayNameRequest
                {
                    DisplayName = PlayerPrefs.GetString("username")
                }, delegate (UpdateUserTitleDisplayNameResult result)
                {
                    Debug.Log("Display Name Changed!");
                }, delegate (PlayFabError error)
                {
                    Debug.Log("Error");
                    if (error.Error == PlayFabErrorCode.AccountBanned)
                    {
                        SceneManager.LoadScene("Bans");
                    }
                });
            }

        }
    }
}