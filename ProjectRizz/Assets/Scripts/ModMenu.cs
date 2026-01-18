using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR;
using easyInputs;
using static easyInputs.EasyInputs;

public class ModMenu : MonoBehaviour
{
    // VARIABLE WAREHOUSE!1!!!11111!1!!1
    public GameObject modMenu;
    private InputDevice targetDevice;
    public bool isPressed;
    public bool speedBoostEnabled;
    public bool hideFromLeaderboard;
    public bool noClipEnabled;
    public bool platformsEnabled;
    public bool spawnMaxwell;
    public bool invis;
    public bool barkFly;
    public bool backroomsGun;
    public bool page1;
    public bool page2;
    public bool page3;
    public Transform lSphere;

    // Start is called before the first frame update
    void Start()
    {
        List<InputDevice> devices = new List<InputDevice>();
        InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.Controller, devices);
        if (devices.Count > 0)
        {
            targetDevice = devices[0];
        }
    }

    // Update is called once per frame
    void Update()
    {
        modMenu.transform.rotation = lSphere.rotation;
        if (targetDevice.isValid)
        {
            if (GetPrimaryButtonDown(EasyHand.LeftHand))
            {
                if (!isPressed)
                {
                    ToggleModMenu();
                    isPressed = true;
                }
            }
            else
            {
                isPressed = false;
            }
        }
    }
    
    private void ToggleModMenu()
    {
        modMenu.SetActive(!modMenu.activeSelf);
    }
}
