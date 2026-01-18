using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SetSound : MonoBehaviour
{
    [SerializeField]
    public AudioSource audioBoard;
    public AudioClip audClip;
    // Update is called once per frame
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("HandTag"))
        {
            if (audClip == null)
            {
                audioBoard.clip = null; // Set the audio clip to none
            }
            else
            {
                audioBoard.clip = audClip;
            }
        }
    }
}