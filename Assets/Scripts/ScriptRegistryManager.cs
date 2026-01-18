using UnityEngine;
using UnityEngine.Networking;
using System.IO;
using System.Collections;
using System.Text.RegularExpressions;
using System.Collections.Generic;

[System.Serializable]
public class ScriptEntry
{
    public string filename;
    public string title;
    public string author;
    public string description;
    public string credits;
}

[System.Serializable]
public class ScriptRegistry
{
    public List<ScriptEntry> scripts;
}

public class ScriptRegistryManager : MonoBehaviour
{
    private string rawRepoUrl = "https://raw.githubusercontent.com/Unixityyy/pjrz-script-registry/main/";
    
    public void FetchRegistry(System.Action<ScriptRegistry> callback)
    {
        StartCoroutine(GetRegistry(callback));
    }

    private IEnumerator GetRegistry(System.Action<ScriptRegistry> callback)
    {
        using (UnityWebRequest www = UnityWebRequest.Get(rawRepoUrl + "registry.json"))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                string cleanJson = Regex.Replace(www.downloadHandler.text, @"//.*|/\*[\s\S]*?\*/", "");
                ScriptRegistry registry = JsonUtility.FromJson<ScriptRegistry>(cleanJson);
                callback?.Invoke(registry);
            }
            else
            {
                Debug.LogError("Failed to fetch registry: " + www.error);
            }
        }
    }

    public void InstallScript(string filename)
    {
        StartCoroutine(DownloadScript(filename));
    }

    private IEnumerator DownloadScript(string filename)
    {
        string url = rawRepoUrl + "scripts/" + filename;
        
        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                string folderPath = Path.Combine(Application.persistentDataPath, "JS");
                string filePath = Path.Combine(folderPath, "main.js");

                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                File.WriteAllText(filePath, www.downloadHandler.text);
                Debug.Log($"Successfully installed {filename} to {filePath}");
            }
            else
            {
                Debug.LogError("Failed to download script: " + www.error);
            }
        }
    }
}