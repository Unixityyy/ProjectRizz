using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.InputSystem;

public class HMDPos : MonoBehaviour
{
    private XRHMD hmd;
    void Update()
    {
        hmd = InputSystem.GetDevice<XRHMD>();
        if (hmd != null)
        {
            transform.position = hmd.devicePosition.ReadValue();
        }
    }
}