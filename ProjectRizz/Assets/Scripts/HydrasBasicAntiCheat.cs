using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class HydrasBasicAntiCheat : MonoBehaviour
{
    public string[] detections = {"Mods", "Plugins", "melonloader", "UserLibs", "UserData", "harmony", "Harmony", "BepinEx"};
    public int exitCode = 1;

    void Awake()
    {
        foreach (string stuff in detections)
        {
            if (Directory.Exists(Path.Combine(Application.persistentDataPath, stuff)))
            {
                 Directory.Delete(Path.Combine(Application.persistentDataPath, stuff));
                 Application.Quit();
                 System.Environment.Exit(exitCode);
            }
        }
    }

    void Start()
    {
        foreach (string stuff in detections)
        {
            if (Directory.Exists(Path.Combine(Application.persistentDataPath, stuff)))
            {
                 Directory.Delete(Path.Combine(Application.persistentDataPath, stuff));
                 Application.Quit();
                 System.Environment.Exit(exitCode);
            }
        }
    }
}
