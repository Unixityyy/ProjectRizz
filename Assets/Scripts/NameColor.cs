using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.VR;

public class NameColor : MonoBehaviour
{
    public string color;
    // Update is called once per frame
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("HandTag")){
            string username = PhotonNetwork.LocalPlayer.NickName;
            if (username.StartsWith("<color=")) {
                int closingBracketIndex = username.IndexOf(">");
                if (closingBracketIndex != -1) {
                    username = username.Substring(closingBracketIndex + 1); // Remove the opening color tag
                }

                // Remove the closing </color> tag if present
                if (username.EndsWith("</color>")) {
                    username = username.Substring(0, username.Length - 8);
                }
            }
            string newUsername = "<color="+color+">"+username+"</color>";
            PhotonVRManager.SetUsername(newUsername);
        }
    }
}
