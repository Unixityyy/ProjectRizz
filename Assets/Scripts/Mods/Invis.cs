using System.Collections;
using System.Collections.Generic;
using GorillaLocomotion;
using Unity.VisualScripting;
using UnityEngine;

public class Invis : MonoBehaviour
{
    public ModMenu menuScript;
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
                    menuScript.invis = false;
                    ModDisable();
                }
                else if (isEnable)
                {
                    gameObject.SetActive(false);
                    opposer.SetActive(true);
                    menuScript.invis = true;
                    ModEnable();
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