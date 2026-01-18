using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.VR;
public class ChangeHeadCosmetic : MonoBehaviour
{
    public string Head;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("HandTag"))
        {
            PhotonVRManager.SetCosmetic("Head", Head);
        }
    }
}