using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableModerator : MonoBehaviour
{
    public List<GameObject> stuff;
    public List<GameObject> unstuff;
    public void DisableModPowers()
    {
        foreach (var go in stuff)
        {
            go.SetActive(false);
        }

        foreach (var go in unstuff)
        {
            go.SetActive(true);
        }

        gameObject.SetActive(false);
    }
}
