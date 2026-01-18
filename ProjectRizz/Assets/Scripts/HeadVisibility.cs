using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class HeadVisibility : MonoBehaviour
{
    public Transform head;
    private PhotonView photonView;

    // Start is called before the first frame update
    void Start()
    {
        // Debug.Log("enabled");
        photonView = PhotonView.Get(this);
        UpdateVisibility();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateVisibility();
    }

    // Method to update the visibility of the head
    void UpdateVisibility()
    {
        // Debug.Log("PhotonView IsMine: " + photonView.IsMine);
        if (photonView.IsMine)
        {
            // Debug.Log("local");
            head.localScale = new Vector3(0, 0, 0);
        }
    }
}
