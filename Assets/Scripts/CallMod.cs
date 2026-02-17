using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;

public class CallMod : MonoBehaviour
{
    [Header("the modcall reason will be the name of this gameobject")]
    [Header("how to use:")]
    [Header("1. make an empty gameobject with all ur modcall reasons as cubes or meshes")]
    [Header("2. put this script on all of those")]
    [Header("3. enable istrigger on the colliders for those")]
    [Header("4. put ur webhook url in all of the scripts")]
    public PlayfabLogin playfablogin;
    [SerializeField]
    private string WebhookURL;

    private void Awake()
    {
        if (playfablogin == null)
        {
            playfablogin = PlayfabLogin.instance;
        }
    }

    private void ModCall(string Reason)
    {
        string jsonPayload = "{\"content\": \"<@&1355942327583248494> Player has called for mod! Reason: " + Reason + ", Player ID is " + playfablogin.MyPlayFabID + ".\", \"allowed_mentions\": {\"parse\": [\"everyone\"]}}";
        UnityWebRequest www = new UnityWebRequest(WebhookURL, "POST");
        byte[] jsonToSend = new UTF8Encoding().GetBytes(jsonPayload);
        www.uploadHandler = new UploadHandlerRaw(jsonToSend);
        www.downloadHandler = new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/json");
        www.SendWebRequest().completed += _ =>
        {
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Reporting Webhook Error: " + www.error);
            }
        };
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("HandTag"))
        {
            this.ModCall(gameObject.name);
            StartCoroutine(DelayTheButton());
        }
    }

    private IEnumerator DelayTheButton()
    {
        Transform parent = transform.parent;
        GameObject parentObject = parent?.gameObject;

        if (parentObject != null)
        {
            parentObject.SetActive(false);

            GameObject runner = new GameObject("ReenableRunner");
            DontDestroyOnLoad(runner);
            runner.AddComponent<TimedActivator>().Init(parentObject, 600f);
        }

        yield break;
    }
}
