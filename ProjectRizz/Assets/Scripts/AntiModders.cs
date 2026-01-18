using Photon.Pun;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using GorillaLocomotion;
using PlayFab;

public class AntiModders : MonoBehaviour
{
    [Header("Script by pugls dont leak because its paid!")]
    [Header("webhook link")]
    [SerializeField]
    private string webhookLink = "";
    public bool CanSend = true;
    public bool CanSend2 = true;
    [SerializeField] private string playfabTitleId = "id";
    [Header("troll modders")]
    public bool LagTheirOculusOut;
    public bool EarrapeThem;
    public bool NotTrollingJustQuit;
    public bool jumpscare;
    public bool PubliclyHumiliateThem;
    public bool TpToRandomRoom;
    public bool CheaterTherapy;
    public bool CheaterTrial;
    public bool CheaterWalkofShame;
    public bool LongScaryHallway;
    public bool EbayModder;
    [Header("Config stuff")]
    public GameObject earrapesound;
    public GameObject lagobject;
    public GameObject jumpscareobject;
    public GameObject Tomato;
    public GameObject modderjoinaudio;
    public GameObject EbayModderCame;
    public Transform HumiliateTP;
    public Transform TomatoSpawnPos;
    public Transform TomatoSpawnPos2;
    public Transform TomatoSpawnPos3;
    public Transform gorillaplayerpos;
    public Transform TherapyRoom;
    public Transform TrialRoom;
    public Transform[] RandomRooms;
    public Rigidbody gp;
    public Camera EbayModderCam;
    public float cooldownTime = 10f;
    private float lastImageSentTime;
    public float jumpscareduration = 5f;
    public float tpcooldown = 0.5f;
    public GorillaLocomotion.Player gorillaplayer;
    public LayerMask nothingLayer;
    public LayerMask defaultLayer;
    public bool joinedyet;
    public float timebeforsend;
    public float speed;
    public float speedWarning = 5000;
    public float PlayerSpeed;
    public float PlayerMultiplyer;
    private bool tpedd;
    [Header("Options/Settings")]
    public bool PlayAudioOnModderJoin;
    public bool UnityTesting;
    public bool speedcheck;
    public string WebhookLink_Warnings = "webhook for warning";

    private void Start()
    {
        string CurrentPackageName = Application.identifier;
        string folderchecker = "/storage/emulated/0/Android/data/" + Application.identifier + "/files/Mods";

        if (Directory.Exists(folderchecker) && CanSend)
        {
            string message = "**Player**" + PhotonNetwork.NickName + " **is modding ban them in playfab**";
            StartCoroutine(SendWebhook(webhookLink, message));
            CanSend = false;

            if (NotTrollingJustQuit)
            {
                Application.Quit();
            }
            if (EarrapeThem)
            {
                earrapesound.SetActive(true);
            }
            if (LagTheirOculusOut)
            {
                Instantiate(lagobject, new Vector3(0, 0, 0), Quaternion.identity);
            }
            if (earrapesound && LagTheirOculusOut)
            {
                Instantiate(lagobject, new Vector3(0, 0, 0), Quaternion.identity);
                earrapesound.SetActive(true);
            }
            if (jumpscare)
            {
                jumpscareobject.SetActive(true);
                StartCoroutine(jumpscarequit());
            }
            if (PubliclyHumiliateThem && !tpedd)
            {
                gorillaplayer.locomotionEnabledLayers = nothingLayer;
                gp.position = HumiliateTP.position;
                PhotonNetwork.Instantiate(Tomato.name, TomatoSpawnPos.position, TomatoSpawnPos.rotation);
                PhotonNetwork.Instantiate(Tomato.name, TomatoSpawnPos2.position, TomatoSpawnPos2.rotation);
                PhotonNetwork.Instantiate(Tomato.name, TomatoSpawnPos3.position, TomatoSpawnPos3.rotation);
                tpedd = true;
            }
            if (PlayAudioOnModderJoin == true && joinedyet == false)
            {
                modderjoin();
            }
            if (TpToRandomRoom && !tpedd)
            {
                int randomIndex = Random.Range(0, RandomRooms.Length);

                gp.position = RandomRooms[randomIndex].transform.position;

                tpedd = true;
            }
            if (CheaterTherapy && !tpedd)
            {
                gorillaplayer.locomotionEnabledLayers = nothingLayer;
                gp.position = TherapyRoom.position;
                tpedd = true;
            }
            if (CheaterTrial && !tpedd)
            {
                gorillaplayer.locomotionEnabledLayers = nothingLayer;
                gp.position = TherapyRoom.position;
            }
            if (CheaterWalkofShame && !tpedd)
            {
                gorillaplayer.locomotionEnabledLayers = nothingLayer;
                gp.position = TherapyRoom.position;
                StartCoroutine(tp());
            }
            if (LongScaryHallway && !tpedd)
            {
                gorillaplayer.locomotionEnabledLayers = nothingLayer;
                gp.position = TherapyRoom.position;
                StartCoroutine(tp());
            }
            if (EbayModder && CanSend2)
            {
                EbayModderCame.SetActive(true); 
                StartCoroutine(Waitbeforsend());
            }
        }
        else
        {
            Debug.Log("Not a modder lol.");
        }
    }

    Texture2D capturess()
    {
        int width = Screen.width;
        int height = Screen.height;
        RenderTexture Renderer = new RenderTexture(width, height, 24);
        EbayModderCam.targetTexture = Renderer;
        Texture2D screenshot = new Texture2D(width, height, TextureFormat.RGB24, false);
        EbayModderCam.Render();
        RenderTexture.active = Renderer;
        screenshot.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        EbayModderCam.targetTexture = null;
        RenderTexture.active = null;
        Destroy(Renderer);
        screenshot.Apply();
        return screenshot;
    }

    private void Update()
    {
        if (UnityTesting)
        {
            if (PubliclyHumiliateThem)
            {
                gp.position = HumiliateTP.position;
                gorillaplayer.locomotionEnabledLayers = nothingLayer;
                PhotonNetwork.Instantiate(Tomato.name, TomatoSpawnPos.position, TomatoSpawnPos.rotation);
                PhotonNetwork.Instantiate(Tomato.name, TomatoSpawnPos2.position, TomatoSpawnPos2.rotation);
                PhotonNetwork.Instantiate(Tomato.name, TomatoSpawnPos3.position, TomatoSpawnPos3.rotation);
            }
            if (jumpscare)
            {
                jumpscareobject.SetActive(true);
                StartCoroutine(jumpscarequit());
            }
            if (EarrapeThem)
            {
                earrapesound.SetActive(true);
            }
            if (PlayAudioOnModderJoin == true && joinedyet == false)
            {
                modderjoin();
            }
            if (LagTheirOculusOut)
            {
                Instantiate(lagobject, new Vector3(0, 0, 0), Quaternion.identity);
            }

            if (EbayModder && CanSend2)
            {
                StartCoroutine(Waitbeforsend());
            }
        }

        if (speed < 0.01f)
        {
            speed = 0f;
        }
        else
        {
            speed = gp.velocity.magnitude;
        }
        if (speedcheck)
        {
            if (speed > speedWarning)
            {
                string playerName = PhotonNetwork.LocalPlayer.NickName;
                string playFabID = PlayFabSettings.staticPlayer.PlayFabId;
                string message = $"A Player Nickname: {playerName} User Id: {playFabID} Has been reported by anti cheat because they have been going a little too fast";
                StartCoroutine(SendWebhook(WebhookLink_Warnings, message));
            }
        }
    }

    IEnumerator SendWebhook(string link, string message)
    {
        WWWForm form = new WWWForm();
        form.AddField("content", message);
        using (UnityWebRequest www = UnityWebRequest.Post(link, form))
        {
            yield return www.SendWebRequest();
        }
        CanSend = false;
    }

    IEnumerator webhookmessage(byte[] imageBytes)
    {
        yield return new WaitForSeconds(cooldownTime);
        WWWForm form = new WWWForm();
        form.AddField("content", "An Ebay modder has been found");
        form.AddBinaryData("file", imageBytes, "screenshot.png", "image/png");
        using (UnityWebRequest www = UnityWebRequest.Post(webhookLink, form))
        {
            yield return www.SendWebRequest();
            if (www.isNetworkError || www.isHttpError)
            {
                Debug.LogError("Modder Img error " + www.error);
            }
            else
            {
                Debug.Log("Modder Img sent" + www.responseCode);
            }
        }
        CanSend2 = false;
    }

    IEnumerator Waitbeforsend()
    {
        yield return new WaitForSeconds(timebeforsend);
        makess();
    }

    void modderjoin()
    {
        PhotonNetwork.Instantiate(modderjoinaudio.name, gorillaplayerpos.position, gorillaplayerpos.rotation);
        modderjoinaudio.SetActive(true);
        joinedyet = false;
    }

    void makess()
    {
        EbayModderCame.SetActive(true);
        Texture2D screenshot = capturess();
        byte[] imageBytes = screenshot.EncodeToPNG();
        StartCoroutine(webhookmessage(imageBytes));
    }

    IEnumerator jumpscarequit()
    {
        yield return new WaitUntil(() => jumpscareduration <= 0);
        Application.Quit();
    }

    IEnumerator tp()
    {
        yield return new WaitUntil(() => tpcooldown <= 0);
        gorillaplayer.locomotionEnabledLayers = defaultLayer;
    }

    // Copyright � Pugls 2024
    // All rights reserved.
    //
    // This script is the intellectual property of Pugls.
    // It is not to be copied, reproduced, modified, or distributed
    // without the prior written permission of Pugls.
    //
    // Unauthorized reproduction or distribution of this script, or any portion of it,
    // may result in severe civil and criminal penalties, and will be prosecuted
    // to the maximum extent possible under the law.

    // This script is provided "as is" and without any express or implied warranties,
    // including, without limitation, the implied warranties of merchantability and fitness
    // for a particular purpose.

    // For permission requests, please contact:
    // Pugls
    // Email: puglsgaming@hotmail.com
    // Discord: Pugls

    // Note: This script may contain code snippets, algorithms, or other elements
    // that are subject to third-party licenses and restrictions. It is your responsibility
    // to comply with any applicable third-party terms and conditions.
}
