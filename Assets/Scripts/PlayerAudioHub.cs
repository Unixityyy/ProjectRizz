using UnityEngine;
using Photon.Pun;

public class PlayerAudioHub : MonoBehaviourPunCallbacks
{
    public AudioSource soundboardSource;
    public AudioClip[] clips;
    public float cooldownTime = 0.5f;
    private float lastPlayedTime;

    public void RequestSound(int index)
    {
        if (photonView.IsMine && Time.time >= lastPlayedTime + cooldownTime)
        {
            lastPlayedTime = Time.time;
            photonView.RPC("RPC_PlaySound", RpcTarget.All, index);
        }
    }

    [PunRPC]
    void RPC_PlaySound(int index)
    {
        if (clips != null && index >= 0 && index < clips.Length)
        {
            if (clips[index] != null)
            {
                soundboardSource.PlayOneShot(clips[index]);
            }
        }
    }
}