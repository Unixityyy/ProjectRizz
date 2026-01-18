using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Made By conspeed55#0540 On Discord
public class SaveColor : MonoBehaviour
{
    private float RedKey = 0;
    private float GreenKey = 0;
    private float BlueKey = 0;
    private float LastRed = 0;
    private float LastGreen = 0;
    private float LastBlue = 0;
    public ColorScript ColorScript;
    // Start is called before the first frame update
    void Start()
    {
        RedKey = PlayerPrefs.GetFloat("RedKey");
        GreenKey = PlayerPrefs.GetFloat("GreenKey");
        BlueKey = PlayerPrefs.GetFloat("BlueKey");
        ColorScript.Red = RedKey;
        ColorScript.Green = GreenKey;
        ColorScript.Blue = BlueKey;
    }

    // Update is called once per frame
    private void Update()
    {
            RedKey = ColorScript.Red;
            GreenKey = ColorScript.Green;
            BlueKey = ColorScript.Blue;
            
        if (RedKey != LastRed &&  GreenKey != LastGreen && BlueKey != LastBlue)
        {
            PlayerPrefs.SetFloat("BlueKey", BlueKey);
            PlayerPrefs.SetFloat("GreenKey", GreenKey);
            PlayerPrefs.SetFloat("RedKey", RedKey);
            LastRed = RedKey;
            LastGreen = GreenKey;
            LastBlue = BlueKey;
        }
    }
}