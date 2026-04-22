using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZeroGTrigger : MonoBehaviour
{
    public bool isEnable;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("HandTag"))
        {
            GorillaLocomotion.Player.Instance.playerRigidBody.useGravity = !isEnable;
        }
    }
}
