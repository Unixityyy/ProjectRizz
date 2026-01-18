using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class TestSound : MonoBehaviourPun
{
    [SerializeField] public AudioSource audioSource;
    public Button playButton;

    private void Start()
    {
        playButton.onClick.AddListener(SendPlay);
    }

    private void SendPlay()
    {
        photonView.RPC("PlayAudio", RpcTarget.All);
    }

    // Call this method from your client code to play the sound on all clients
    [PunRPC]
    private void PlayAudio()
    {
        audioSource.Play();
    }
}
