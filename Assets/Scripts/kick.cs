using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Oculus.Platform;

public class kick : MonoBehaviour
{
    public PhotonView ptView;

    void OnTriggerEnter(Collider other)
    {
        if (ptView.IsMine)
        {
            return;
        }
        else
        {
            if (Core.IsInitialized())
            {
                Achievements.Unlock("modKick");
            }
            UnityEngine.Application.Quit();
        }
    }
}