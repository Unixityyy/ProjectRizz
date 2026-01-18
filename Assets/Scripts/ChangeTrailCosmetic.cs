using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.VR;
public class ChangeTrailCosmetic : MonoBehaviour
{
    public string Trail;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("HandTag"))
        {
            PhotonVRManager.SetCosmetic("Trail", Trail);
        }
    }
}