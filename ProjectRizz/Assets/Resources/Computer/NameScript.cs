using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.VR;
using PlayFab;
using TMPro;
using System.IO;
public class NameScript : MonoBehaviour
{
    public string NameVar;
    public TextMeshPro Nametext;
    public void Update()
    {
        if (NameVar.Length > 12)
        {
            NameVar = NameVar.Substring(0, 12);
        }
        Nametext.text = NameVar;
    }
}
