using UnityEngine;
using Photon.Pun;
using System;
using System.Collections.Generic;

[RequireComponent(typeof(PhotonView))]
public class MainSB : MonoBehaviourPunCallbacks, IPunObservable
{
    public List<sound> sounds = new List<sound>();
    public PhotonView ptview;
    public AudioSource audioplay;

    [Serializable]
    public class sound
    {
        public string name;
        public AudioClip clip;
    }

    [PunRPC]
    public void PS(string nwsound)
    {
        foreach (sound s in sounds)
        {
            if (s.name == nwsound)
            {
                audioplay.clip = s.clip;
                audioplay.Play();
                break;
            }
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
    }
}