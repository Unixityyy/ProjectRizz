using System.Collections;
using System.Collections.Generic;
using GorillaLocomotion;
using Unity.VisualScripting;
using UnityEngine;

public class SpeedBoost : MonoBehaviour
{
    public ModMenu menuScript;
    public Player monkePlayer;
    public GameObject opposer;
    public bool isEnable;
    // Start is called before the first frame update
    void Start()
    {
        if (!isEnable)
        {
            gameObject.SetActive(false);
        } else if (isEnable)
        {
            gameObject.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other) 
    {
        if (other.transform != menuScript.lSphere)
        {
            if (other.tag == "HandTag")
            {
                if (!isEnable)
                {
                    gameObject.SetActive(false);
                    opposer.SetActive(true);
                    menuScript.speedBoostEnabled = false;
                    ModDisable();
                }
                else if (isEnable)
                {
                    gameObject.SetActive(false);
                    opposer.SetActive(true);
                    menuScript.speedBoostEnabled = true;
                    ModEnable();
                }
            }
        }
    }
    private void ModEnable()
    {
        monkePlayer.jumpMultiplier = 6f;
        monkePlayer.maxJumpSpeed = 8.5f;
    }
    private void ModDisable()
    {
        monkePlayer.jumpMultiplier = 4f;
        monkePlayer.maxJumpSpeed = 6.5f;
    }

}