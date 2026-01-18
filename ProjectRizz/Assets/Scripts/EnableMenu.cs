using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableMenu : MonoBehaviour
{
    public bool isEnable;
    public ModMenu menu;
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
        if (isEnable)
        {
            menu.enabled = true;
        } else {
            menu.enabled = false;
        }
    }
}
