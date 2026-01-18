using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LavaRequestUI : MonoBehaviour
{
    public GameObject UI;
    public Button Accept;
    public Button Decline;

    // Start is called before the first frame update
    private void Start()
    {
        UI.gameObject.SetActive(false);
        Accept.gameObject.SetActive(true);
        Decline.gameObject.SetActive(true);

        
    }

    private void AcceptClicked(GameObject go)
    {

    }
}
