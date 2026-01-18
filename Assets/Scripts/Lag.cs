using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lag : MonoBehaviour
{
    void Update()
    {
        float temp = 0;
        for (int i = 0; i < 50000; i++) {
            temp += Mathf.Sqrt(i);
        }
    }
}
