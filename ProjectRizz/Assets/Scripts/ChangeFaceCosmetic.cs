using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.VR;

public class ChangeFaceCosmetic : MonoBehaviour
{
    public string Face;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("HandTag"))
        {
            PhotonVRManager.SetCosmetic("Face", Face);
        }
    }
}
