using PlayFab;
using PlayFab.ClientModels;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EarnRust : MonoBehaviour
{
    public GameObject parkour;
    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("HandTag")) {
            var request = new ExecuteCloudScriptRequest
            {
                FunctionName = "completeTestPark",
                GeneratePlayStreamEvent = true
            };

            PlayFabClientAPI.ExecuteCloudScript(request,
                result =>
                {
                    PlayfabLogin.instance.GetVirtualCurrencies();
                    parkour.SetActive(false);
                },
                error =>
                {
                    parkour.SetActive(false);
            });
        }
    }
}