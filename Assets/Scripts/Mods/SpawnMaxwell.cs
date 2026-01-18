using System.Collections;
using System.Collections.Generic;
using GorillaLocomotion;
using Photon.Pun;
using Unity.VisualScripting;
using UnityEngine;

public class SpawnMaxwell : MonoBehaviour
{
    public ModMenu menuScript;
    public GameObject maxwellPrefab;
    private GameObject previousMaxwell;
    public bool spawn;

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (menuScript.spawnMaxwell)
        {
            if (PhotonNetwork.IsConnected)
            {
                // // Destroy the previous Maxwell if the player owns it
                // if (previousMaxwell != null && previousMaxwell.GetComponent<PhotonView>().IsMine)
                // {
                //     PhotonNetwork.Destroy(previousMaxwell);
                // }

                // Instantiate the new Maxwell
                PhotonNetwork.Instantiate("Maxwell", menuScript.lSphere.position, Quaternion.identity, 0);
            }
            menuScript.spawnMaxwell = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform != menuScript.lSphere)
        {
            if (other.tag == "HandTag")
            {
                if (PhotonNetwork.IsConnected)
                {
                    // Destroy the previous Maxwell if the player owns it
                    if (previousMaxwell != null && previousMaxwell.GetComponent<PhotonView>().IsMine)
                    {
                        PhotonNetwork.Destroy(previousMaxwell);
                    }

                    // Instantiate the new Maxwell
                    previousMaxwell = PhotonNetwork.Instantiate("Maxwell", menuScript.lSphere.position, Quaternion.identity, 0);
                }
            }
        }
    }
}
