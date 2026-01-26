using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableNames : MonoBehaviour
{
    public bool isEnable;
    public LeaderBoard board;
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
            board.DisplayUsernames = true;
        } else {
            board.DisplayUsernames = false;
        }
    }
}
