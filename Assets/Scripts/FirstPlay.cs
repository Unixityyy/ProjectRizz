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
    // Start is called before the first frame update
    void Start()
    {
        agreed = (PlayerPrefs.GetInt("assetAgreement", 0)) == 1;
        assetPath = Path.Combine(Application.persistentDataPath, "JS/Assets");
        understand.onClick.AddListener(IUnderstand);
        if (PlayerPrefs.GetString("firstPlay") == "yes")
        {
            
            if (Directory.Exists(assetPath))
            {
                try
                {
                    if (!agreed)
                    {
                        StartCoroutine(ScriptRunner.instance.LoadWarning());
                    }
                }
                catch (Exception ex)
                {
                    Debug.Log($"Failed to load custom asset warning!");
                }
            }
            else
            {
                Debug.Log($"No custom assets.");
                SceneManager.LoadScene("MonkeTag");
            }
        } else {
            uiCanvas.gameObject.SetActive(true);
        }
    }

    private void Update()
    {
        if (skip)
        {
            skip = false;
            PlayerPrefs.SetString("firstPlay", "yes");
            PlayerPrefs.Save();
            if (Directory.Exists(assetPath))
            {
                try
                {
                    if (!agreed)
                    {
                        StartCoroutine(ScriptRunner.instance.LoadWarning());
                    }
                }
                catch (Exception ex)
                {
                    Debug.Log($"Failed to load custom asset warning!");
                }
            }
            else
            {
                Debug.Log($"No custom assets.");
                SceneManager.LoadScene("MonkeTag");
            }
        }
    }

    private void IUnderstand()
    {
        if (count != 9)
        {
            count += 1;
        } else {
            PlayerPrefs.SetString("firstPlay", "yes");
            PlayerPrefs.Save();
            if (Directory.Exists(assetPath))
            {
                try
                {
                    if (!agreed)
                    {
                        StartCoroutine(ScriptRunner.instance.LoadWarning());
                    }
                }
                catch (Exception ex)
                {
                    Debug.Log($"Failed to load custom asset warning!");
                }
            }
            else
            {
                Debug.Log($"No custom assets.");
                SceneManager.LoadScene("MonkeTag");
            }
        }
    }
}
