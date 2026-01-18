using Photon.Pun;
using PlayFab;
using PlayFab.ClientModels;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ParkourEnable : MonoBehaviour
{
    PhotonView photonView;
    public bool isEnable;
    public GameObject parkour;

    [PunRPC]
    public void parkToggle(bool _)
    {
        parkour.gameObject.SetActive(_);
    }

    public void OnTriggerEnter(Collider collision)
    {
        if (gameObject != parkour) { 
            if (collision.CompareTag("HandTag"))
            {
                var request = new ExecuteCloudScriptRequest
                {
                    FunctionName = "parkToggle",
                    FunctionParameter = new Dictionary<string, object>
                    {
                        { "enable", isEnable }
                    },
                    GeneratePlayStreamEvent = true
                };

                PlayFabClientAPI.ExecuteCloudScript(request,
                    result =>
                    {
                        photonView.RPC("parkToggle", RpcTarget.AllBuffered, isEnable);
                    },
                    error =>
                    {
                       Application.Quit(); 
                });
            }
        }
    }
}