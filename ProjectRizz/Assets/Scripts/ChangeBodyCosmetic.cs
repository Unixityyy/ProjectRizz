using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.VR;

public class ChangeBodyCosmetic : MonoBehaviour
{
    public string Body;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("HandTag"))
        {
            PhotonVRManager.SetCosmetic("Body", Body);
        }
    }
}
