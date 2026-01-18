using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(PhotonView))]
public class SoundBoard : MonoBehaviour, IPunObservable
{
    [Header("Made by KEO_CS")]
    [Header("Slightly edited by B.Awesome")]
    [Header("Just drag on the script")]
    [Header("and fill everything out.")]



    public AudioSource Sound;
    public PhotonView PTView;
    public string HandTag = "HandTag";

    public void PlaySound()
    {
        PTView.RPC(nameof(RPC_PlaySound), RpcTarget.AllBuffered);
    }

    [PunRPC]
    private void RPC_PlaySound()
    {
        Sound.Play();
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo photonMessageInfo)
    {
        
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(HandTag))
        {
            PlaySound();
        }
    }
}
