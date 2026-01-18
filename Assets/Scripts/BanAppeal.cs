using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BanAppeal : MonoBehaviour
{
    public Button dc;
    // Start is called before the first frame update
    void Start()
    {
        dc.onClick.AddListener(OpenDc);
        
    }

    private void OpenDc()
    {
        Application.OpenURL("https://discord.gg/Ny8X2kjbzN");
    }
}
