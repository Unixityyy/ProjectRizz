using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModApps : MonoBehaviour
{
    public string URL;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other) {
        if (other.tag == "HandTag")
        {
            Application.OpenURL(URL);
        }
    }
}
