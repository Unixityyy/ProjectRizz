using UnityEngine;
using TMPro;

public class Version : MonoBehaviour
{
    public TextMeshPro VersionText;
    // Start is called before the first frame update
    void Start()
    {
        VersionText.text = "VERSION: " + Application.version;
    }
}
