using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AssetWarning : MonoBehaviour
{
    public Button agree;
    public Button decline;
    // Start is called before the first frame update
    void Start()
    {
        agree.onClick.AddListener(Agree);
        decline.onClick.AddListener(Decline);
    }

    private void Agree()
    {
        PlayerPrefs.SetInt("assetAgreement", 1);
        PlayerPrefs.Save();
        SceneManager.LoadScene("MonkeTag");
    }

    private void Decline()
    {
        PlayerPrefs.SetInt("assetAgreement", 0);
        PlayerPrefs.Save();
        SceneManager.LoadScene("MonkeTag");
    }
}
