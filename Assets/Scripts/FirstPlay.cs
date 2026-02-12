using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
using System.IO;

public class FirstPlay : MonoBehaviour
{
    public Canvas uiCanvas;
    public Button understand;
    private int count;
    public bool skip;
    private string assetPath;
    private bool agreed;

    void Start()
    {
        agreed = (PlayerPrefs.GetInt("assetAgreement", 0)) == 1;
        assetPath = Path.Combine(Application.persistentDataPath, "JS/Assets");
        
        /*
        understand.onClick.RemoveAllListeners(); 
        understand.onClick.AddListener(IUnderstand);

        if (PlayerPrefs.GetString("firstPlay") == "yes")
        {
            HandleTransition();
        }
        else
        {
            uiCanvas.gameObject.SetActive(true);
        }
        */

        // Go straight to assets/scene transition
        HandleTransition();
    }

    private void Update()
    {
        /*
        if (skip)
        {
            skip = false;
            FinishFirstPlay();
        }
        */
    }

    public void IUnderstand()
    {
        /*
        count++;
        Debug.Log("Button clicked. Current count: " + count);

        if (count >= 10) 
        {
            FinishFirstPlay();
        }
        */
    }

    private void FinishFirstPlay()
    {
        /*
        PlayerPrefs.SetString("firstPlay", "yes");
        PlayerPrefs.Save();
        HandleTransition();
        */
    }

    private void HandleTransition()
    {
        if (Directory.Exists(assetPath))
        {
            try
            {
                if (!agreed)
                {
                    StartCoroutine(ScriptRunner.instance.LoadWarning());
                }
                else
                {
                    SceneManager.LoadScene("MonkeTag");
                }
            }
            catch (Exception)
            {
                Debug.Log("Failed to load custom asset warning!");
                SceneManager.LoadScene("MonkeTag");
            }
        }
        else
        {
            Debug.Log("No custom assets.");
            SceneManager.LoadScene("MonkeTag");
        }
    }
}