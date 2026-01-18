using Photon.Pun;
using UnityEngine;

public class MenuButtons : MonoBehaviourPunCallbacks
{
    public ModMenu menuScript;
    public PhotonView ptview;
    public string sound;
    private MainSB localMainSB;

    void Start()
    {
        foreach (var player in GameObject.FindGameObjectsWithTag("Player"))
        {
            PhotonView view = player.GetComponent<PhotonView>();
            if (view != null && view.IsMine)
            {
                Transform soundboardTransform = player.transform.Find("Head/Speaker/Soundboard");
                if (soundboardTransform != null)
                {
                    localMainSB = soundboardTransform.GetComponent<MainSB>();
                }
                break;
            }
        }
    }


    // Update is called once per frame
    void Update()
    {

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform != menuScript.lSphere)
        {
            if (other.tag == "HandTag")
            {
                PlaySound();
            }
        }
    }

    private void PlaySound()
    {
        int playerLayer = LayerMask.NameToLayer("Player");

        foreach (PhotonView view in FindObjectsOfType<PhotonView>())
        {
            if (view.IsMine && view.gameObject.layer == playerLayer)
            {
                Transform soundboardTransform = view.transform.Find("Head/Speaker/Soundboard");
                if (soundboardTransform != null)
                {
                    PhotonView sbView = soundboardTransform.GetComponent<PhotonView>();
                    if (sbView != null)
                    {
                        sbView.RPC("PS", RpcTarget.AllBuffered, sound);
                        Debug.Log($"RPC sent to Soundboard with sound: {sound}");
                    }
                    else
                    {
                        Debug.LogWarning("PhotonView not found on Soundboard.");
                    }
                }
                else
                {
                    Debug.LogWarning("Soundboard transform not found.");
                }
                break;
            }
        }
    }
}