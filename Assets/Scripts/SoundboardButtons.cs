using UnityEngine;
using Photon.Pun;

public class SoundboardButtons : MonoBehaviour
{
    public int soundIndex;
    private PlayerAudioHub localPlayerHub;

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("HandTag")) {
            if (localPlayerHub == null)
            {
                GetAudioHub();
            }

            if (localPlayerHub != null)
            {
                localPlayerHub.RequestSound(soundIndex);
                Debug.Log("requesting sound");
            }
            else
            {
                Debug.LogError("couldnt find audio hub. is the player connected?");
            }
        }
    }

    void GetAudioHub()
    {
        Debug.Log("searching");
        PlayerAudioHub[] hubs = GameObject.FindObjectsOfType<PlayerAudioHub>();
        foreach (PlayerAudioHub hub in hubs)
        {
            if (hub.GetComponent<PhotonView>().IsMine)
            {
                localPlayerHub = hub;
                Debug.Log("found");
                break;
            }
        }
    }

    // void Update()
    // {
    //     Debug.Log("blehhh");
    // }
}