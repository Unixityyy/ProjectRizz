using System.Collections;
using System.Collections.Generic;
using GorillaLocomotion;
using Unity.VisualScripting;
using UnityEngine;

public class LeaderboardHide : MonoBehaviour
{
    public ModMenu menuScript;
    public GameObject opposer;
    public bool isEnable;
    public LeaderBoard lb;
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
                    menuScript.hideFromLeaderboard = false;
                    lb.menuHashed = false;
                }
                else if (isEnable)
                {
                    gameObject.SetActive(false);
                    opposer.SetActive(true);
                    menuScript.hideFromLeaderboard = true;
                    lb.menuHashed = false;
                }
            }
        }
    }
    private void ModEnable()
    {

    }
    private void ModDisable()
    {

    }

}