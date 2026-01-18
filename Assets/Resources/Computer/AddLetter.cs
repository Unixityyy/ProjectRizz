using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddLetter : MonoBehaviour
{
    public NameScript nameScript;
    public string Letter;

    private void OnTriggerEnter(Collider other)
    {
        if(other.transform.tag == "HandTag") 
        {
            nameScript.NameVar += Letter;
        }
    }
}
