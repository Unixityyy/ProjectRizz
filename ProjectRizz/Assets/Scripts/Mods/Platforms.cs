using System.Collections;
using System.Collections.Generic;
using GorillaLocomotion;
using Unity.VisualScripting;
using UnityEngine;
using easyInputs;
using static easyInputs.EasyInputs;

public class Platforms : MonoBehaviour
{
    public ModMenu menuScript;
    public GameObject opposer;
    public bool isEnable;
    public bool isSpawned = false;
    private GameObject Cube;
    public GameObject ObjecttoSpawn;
    public Transform hand;
    public EasyHand controller;

    // Start is called before the first frame update
    void Start()
    {
        if (!isEnable)
        {
            gameObject.SetActive(false);
        }
        else if (isEnable)
        {
            gameObject.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (GetGripButtonDown(EasyHand.LeftHand) || GetGripButtonDown(EasyHand.RightHand))
        {
            if (menuScript.platformsEnabled)
            {
                if (!isSpawned)
                {
                    Destroy(Cube);
                    isSpawned = true;
                    float X = hand.transform.position.x;
                    float Z = hand.transform.position.z;
                    float Y = hand.transform.position.y - 0.1f;
                    Cube = Instantiate(ObjecttoSpawn, new Vector3(X, Y, Z), Quaternion.identity);
                }
            }
            else
            {
                isSpawned = false;
                Destroy(Cube);
            }
        }
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
                    menuScript.platformsEnabled = false;
                    ModDisable();
                }
                else if (isEnable)
                {
                    gameObject.SetActive(false);
                    opposer.SetActive(true);
                    menuScript.platformsEnabled = true;
                    ModEnable();
                }
            }
        }
    }

    private void ModEnable()
    {
        // Add any additional functionality for enabling the mod here
    }

    private void ModDisable()
    {
        // Add any additional functionality for disabling the mod here
    }
}
