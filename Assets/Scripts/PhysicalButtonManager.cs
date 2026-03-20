using UnityEngine;
using System.Collections.Generic;

public class PhysicalButtonManager : MonoBehaviour
{
    [System.Serializable]
    public class TriggerPair
    {
        public GameObject button;
        public GameObject targetObject;
    }

    public List<TriggerPair> pairs;

    private void Start()
    {
        foreach (var pair in pairs)
        {
            if (pair.button != null)
            {
                TriggerDetector detector = pair.button.GetComponent<TriggerDetector>();
                if (detector == null)
                {
                    detector = pair.button.AddComponent<TriggerDetector>();
                }

                detector.OnTriggered = () => HandleActivation(pair.targetObject);
            }
        }
    }

    private void HandleActivation(GameObject activeTarget)
    {
        foreach (var pair in pairs)
        {
            if (pair.targetObject != null)
            {
                pair.targetObject.SetActive(pair.targetObject == activeTarget);
            }
        }
    }
}

public class TriggerDetector : MonoBehaviour
{
    public System.Action OnTriggered;

    private void OnTriggerEnter(Collider other)
    {
        OnTriggered?.Invoke();
    }
}