using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.VR;
public class ChangeRightCosmetic : MonoBehaviour
{
    public string Right;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("HandTag"))
        {
            PhotonVRManager.SetCosmetic("RightHand", Right);
        }
    }
}
