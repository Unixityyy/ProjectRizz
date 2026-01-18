using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.VR;
using TMPro;

public class ColorScript : MonoBehaviour
{
    public static ColorScript instance;

    public float Red;
    public TextMeshPro RValueText;
    public float Green;
    public TextMeshPro GValueText;
    public float Blue;
    public TextMeshPro BValueText;
    float TrueRed;
    float TrueBlue;
    float TrueGreen;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        BValueText.text = "Blue: " + Blue.ToString();
        GValueText.text = "Green: " + Green.ToString();
        RValueText.text = "Red: " + Red.ToString();
    }
    // Update is called once per frame
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("HandTag"))
        {
            TrueRed = Red / 10;
            TrueBlue = Blue / 10;
            TrueGreen = Green / 10;

            Color myColour = new Color(TrueRed, TrueGreen, TrueBlue);
            PhotonVRManager.SetColour(myColour);
        }
    }

}
