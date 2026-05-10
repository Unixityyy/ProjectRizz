using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Click : MonoBehaviour
{
    private GameObject btn;
    public Material returnTo;
    public Material clicked;
    public TextMeshPro clickText;
    public Saving saveScript;
    private new Renderer renderer;
    private bool debounce = false;


    // Start is called before the first frame update
    void Start()
    {
        btn = this.gameObject;
        renderer = btn.GetComponent<Renderer>();
        renderer.material = returnTo;
        debounce = false;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (debounce) return;
        if (other.CompareTag("HandTag"))
        {
            debounce = true;
            renderer.material = clicked;
            saveScript.clicks = saveScript.clicks + 1;
            clickText.text = "CLICKS: " + saveScript.clicks;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (!debounce) return;
        if (other.CompareTag("HandTag"))
        {
            debounce = false;
            renderer.material = returnTo;
        }
    }
}
