using UnityEngine;
using Photon.Pun;

public class PlayerAudioHub : MonoBehaviourPunCallbacks
{
    public AudioSource soundboardSource;
    public AudioClip[] clips;

    public void RequestSound(int index)
    {
        photonView.RPC("RPC_PlaySound", RpcTarget.All, index);
    }

    [PunRPC]
    void RPC_PlaySound(int index)
    {
        if (index >= 0 && index < clips.Length)
        {
            soundboardSource.PlayOneShot(clips[index]);
        }
    }
}