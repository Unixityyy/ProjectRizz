using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Maxwell : MonoBehaviour
{
    private void Start()
    {
    }
    private void Update()
    {
        if (PhotonNetwork.IsConnected)
        {
        Vector3 currentRotation = transform.eulerAngles;
                currentRotation.x = -90f;
                currentRotation.z += -180f * Time.deltaTime;
                transform.eulerAngles = currentRotation;
        }
    }
}
