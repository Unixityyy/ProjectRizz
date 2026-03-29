using UnityEngine;
using UnityEngine.Networking;
using Photon.Pun;
using TMPro;
using Photon.VR;
using Photon.Voice.PUN;
using Photon.VR.Player;
using System.Text;
using System.Collections;
using System;
using System.Collections.Generic;
using PlayFab.ClientModels;
using PlayFab;

[RequireComponent(typeof(PhotonView))]
public class LeaderBoard : MonoBehaviourPunCallbacks
{
    [SerializeField] public TMP_Text[] displaySpot;
    [SerializeField] public Renderer[] ColorSpot;
    public string WebHookURL = "https://discord.com/api/webhooks/1413894359581196388/u0rcLDI2d3A6hGBs3BUc2S4bCTGJpvPMhvtLn_yHgfprZ-caUEcuuiCjKDiG4Sqp8s7u";
    private string WebHookURL1 = "https://discord.com/api/webhooks/1413736708775612416/OWgCe3UTPiphoKyLwb0miqhIbz0brl_jJ8_0Ks-Qr6MgjZaocMTJlL3LqkUk4rS_2bYm";
    [SerializeField] public PlayfabLogin playfablogin;
    private bool hashed;
    public bool DisplayUsernames;
    public ModMenu menuScript;
    private bool Kicked = false;
    public bool menuHashed;
    public Rigidbody GorillaRigidBody;
    private bool fling = false;

    private void Start()
    {
        if (GetComponent<PhotonView>().OwnershipTransfer != OwnershipOption.Takeover)
        {
            GetComponent<PhotonView>().OwnershipTransfer = OwnershipOption.Takeover;
        }
    }
    private void Update()
    {
        if (PhotonNetwork.IsConnected && !menuHashed)
        {
            ExitGames.Client.Photon.Hashtable hash = PhotonNetwork.LocalPlayer.CustomProperties;
            hash["Hidden"] = menuScript.hideFromLeaderboard;
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
            menuHashed = true;
        }
        if (PhotonNetwork.IsConnected && !hashed)
        {
            ExitGames.Client.Photon.Hashtable hash = PhotonNetwork.LocalPlayer.CustomProperties;
            hash["PlayfabID"] = playfablogin.MyPlayFabID;
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
            hashed = true;
        }

        int j = 0;

        foreach (var player in PhotonNetwork.PlayerList)
        {
            if (j >= displaySpot.Length) break;

            if (player == PhotonNetwork.LocalPlayer && Kicked)
            {
                if (PhotonNetwork.IsConnected)
                {
                    Application.Quit();
                }
                displaySpot[j].color = Color.red;
                displaySpot[j].text = "You have been Kicked";
                j++;
                continue;
            }

            string nameText = player.NickName;
            if (DisplayUsernames)
            {
                player.CustomProperties.TryGetValue("MetaDisplayName", out object disp);
                player.CustomProperties.TryGetValue("MetaUsername", out object user);
                string d = disp != null ? (string)disp : "Unknown";
                string u = user != null ? (string)user : "Unknown";
                nameText = $"{player.NickName}\n<color=grey><size=50%>{d} (@{u})</size></color>";
            }

            displaySpot[j].text = nameText;

            foreach (PhotonVRPlayer PVRP in FindObjectsOfType<PhotonVRPlayer>())
            {
                if (PVRP.gameObject.GetComponent<PhotonView>().Owner == player)
                {
                    if (player.CustomProperties.TryGetValue("Colour", out object colorJson))
                    {
                        ColorSpot[j].material.color = JsonUtility.FromJson<Color>((string)colorJson);
                    }
                }
            }

            j++;
        }

        for (int i = j; i < displaySpot.Length; i++)
        {
            displaySpot[i].text = null;
            ColorSpot[i].material.color = Color.white;
        }
    }

    public void MutePress(int ButtonNumber)
    {
        if (PhotonNetwork.PlayerList.Length >= ButtonNumber - 1)
        {
            foreach (PhotonVRPlayer PVRP in FindObjectsOfType<PhotonVRPlayer>())
            {
                if (PVRP.gameObject.GetComponent<PhotonView>().Owner == PhotonNetwork.PlayerList[ButtonNumber - 1])
                {
                    AudioSource audioSource = PVRP.gameObject.GetComponent<PhotonVoiceView>().SpeakerInUse.gameObject.GetComponent<AudioSource>();
                    audioSource.mute = !audioSource.mute;
                    break;
                }
            }
        }
    }

    public void KickPress(int ButtonNumber)
    {
        if (PhotonNetwork.PlayerList.Length >= ButtonNumber - 1)
        {
            foreach (PhotonVRPlayer PVRP in FindObjectsOfType<PhotonVRPlayer>())
            {
                if (PVRP.gameObject.GetComponent<PhotonView>().Owner == PhotonNetwork.PlayerList[ButtonNumber - 1])
                {
                    GetComponent<PhotonView>().RequestOwnership();
                    GetComponent<PhotonView>().RPC("KickPlayer", PVRP.gameObject.GetComponent<PhotonView>().Owner);
                }
            }
        }
    }


    [PunRPC]
    void KickPlayer()
    {
        if (playfablogin.MyPlayFabID != "597830033DFE2334")
        {
            Kicked = true;
        }
    }

    public void FlingPress(int ButtonNumber)
    {
        if (PhotonNetwork.PlayerList.Length >= ButtonNumber - 1)
        {
            foreach (PhotonVRPlayer PVRP in FindObjectsOfType<PhotonVRPlayer>())
            {
                if (PVRP.gameObject.GetComponent<PhotonView>().Owner == PhotonNetwork.PlayerList[ButtonNumber - 1])
                {
                    GetComponent<PhotonView>().RequestOwnership();
                    GetComponent<PhotonView>().RPC("FlingPlayer", PVRP.gameObject.GetComponent<PhotonView>().Owner);
                }
            }
        }
    }


    [PunRPC]
    void FlingPlayer()
    {
        // no check since i wanna fling myself
        // if (playfablogin.MyPlayFabID != "597830033DFE2334")
        // {
            fling = true;
        // }
    }

    private void FixedUpdate()
    {
        if (fling)
        {
            GorillaRigidBody.AddForce(Vector3.up * 50f, ForceMode.Impulse);
            fling = false;
        }
    }

    public void Report(int ButtonNumber)
    {
        string playfabid = playfablogin.MyPlayFabID;
        if (PhotonNetwork.PlayerList.Length >= ButtonNumber - 1)
        {
            foreach (PhotonVRPlayer PVRP in FindObjectsOfType<PhotonVRPlayer>())
            {
                if (PVRP.gameObject.GetComponent<PhotonView>().Owner == PhotonNetwork.PlayerList[ButtonNumber - 1])
                {
            SendtoWebhook(PhotonNetwork.PlayerList[ButtonNumber - 1].NickName + " " + ((string)PVRP.gameObject.GetComponent<PhotonView>().Owner.CustomProperties["PlayfabID"]) + " was reported by " + PlayerPrefs.GetString("Username", null) + " " + playfablogin.MyPlayFabID);
                }
                }
        }
    }

    public void SendtoWebhook(string message)
    {
        StartCoroutine(PostToDiscord(message));
    }

    IEnumerator PostToDiscord(string message)
    {
        string jsonPayload = "{\"content\": \"" + message + "\"}";
        UnityWebRequest www = new UnityWebRequest(WebHookURL1, "POST");
        byte[] jsonToSend = new UTF8Encoding().GetBytes(jsonPayload);
        www.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
        www.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/json");
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Reporting Webhook Error: " + www.error);
        }
    }
}