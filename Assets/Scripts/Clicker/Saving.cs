using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Saving : MonoBehaviour
{
    public float clicks;
    public Click clickScript;
    // Start is called before the first frame update
    void Start()
    {
        clicks = PlayerPrefs.GetFloat("Clicks");
        clickScript.clickText.text = "CLICKS: " + clicks;
    }

    // Update is called once per frame
    private void OnApplicationQuit()
    {
        // TODO: MAKE THIS LOAD FROM CLOUD!!!!!!!!!!!!!!
        PlayerPrefs.SetFloat("Clicks", clicks);
        PlayerPrefs.Save();
        Debug.Log("saved clicks");
    }
}
