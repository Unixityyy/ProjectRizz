using UnityEngine;
using System.IO;
using System;

public class ByteCheck : MonoBehaviour
{
    void Start()
    {
        if (DetectUABEA())
        {
            Application.Quit();
        }
    }

    private bool DetectUABEA()
    {
        string[] uabeaIndicators = { "uabea_config", "uabea_logs", "uabea_backup" };

        foreach (string indicator in uabeaIndicators)
        {
            string path = Path.Combine(Application.persistentDataPath, indicator);
            if (File.Exists(path))
            {
                return true;
            }
        }

        return false;
    }
}