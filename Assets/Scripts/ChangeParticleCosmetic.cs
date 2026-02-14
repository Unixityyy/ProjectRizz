using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.VR;

public class ChangeParticleCosmetic : MonoBehaviour
{
    public string Particle;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("HandTag"))
        {
            PhotonVRManager.SetCosmetic("Particles", Particle);
        }
    }
}
