using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sleep : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    public IEnumerator SleepWait(int secs)
    {
        yield return new WaitForSeconds(secs);
    }
}
