using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using PlayFab;
using PlayFab.ClientModels;
using Oculus.Platform;

public class WardrobePurchase : MonoBehaviour
{
    // Start is called before the first frame update

    [Header("COSMETICS")]
    public GameObject Purchasable;
    public GameObject WardrobePart;

    [Header("BUY")]
    public string CosmeticName;
    public int coinsPrice;
    public PlayfabLogin playfablogin;
    public bool collectible;


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "HandTag")
        {
            Debug.Log("hit");
            if (playfablogin.coins >= coinsPrice)
            {  
                BuyItem();
            }
        }

    }

    private void OnConvertResult(ExecuteCloudScriptResult result) {
        Debug.Log("converted: " + CosmeticName);
        PlayerPrefs.DeleteKey(CosmeticName);
        PlayerPrefs.Save();
    }

    private void OnErrorShared(PlayFabError error)
    {
        Debug.Log(error.GenerateErrorReport());
    }

    public void BuyItem()
    {
        Debug.Log("buy");
        var request = new PurchaseItemRequest
        {
            ItemId = CosmeticName,
            Price = coinsPrice,
            VirtualCurrency = "RT",
        };
        PlayFabClientAPI.PurchaseItem(request, PurchaseSuccess, OnError);
    }

    void PurchaseSuccess(PurchaseItemResult result)
    {
        Purchasable.SetActive(true);
        WardrobePart.SetActive(true);
        gameObject.SetActive(false);
        PlayfabLogin.instance.GetVirtualCurrencies();
        Debug.Log("buy success");
    }

    void OnError(PlayFabError error)
    {
        Debug.Log("Error: " + error.GenerateErrorReport());
    }

    public void OnLoginEvent() 
    {
        // not needed
    }

    private void Start()
    {
        if (PlayerPrefs.GetInt(CosmeticName) == 1)
        {
            Purchasable.SetActive(true);
            WardrobePart.SetActive(true);
            gameObject.SetActive(false);
        }
    }
}