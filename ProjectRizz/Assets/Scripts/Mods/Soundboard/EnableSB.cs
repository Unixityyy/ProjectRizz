using System.Collections;
using System.Collections.Generic;
using GorillaLocomotion;
using Unity.VisualScripting;
using UnityEngine;

public class EnableSB : MonoBehaviour
{
    public bool isEnable;
    public GameObject page1;
    public GameObject sb;
    public ModMenu menuScript;
    // Start is called before the first frame update
    void Start()
    {
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
                    ModDisable();
                }
                else if (isEnable)
                {
                    ModEnable();
                }
            }
        }
    }
    private void ModEnable()
    {
        sb.SetActive(true);
        page1.SetActive(false);
    }
    private void ModDisable()
    {
        page1.SetActive(true);
        sb.SetActive(false);
    }
}