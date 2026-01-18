using System.Collections;
using UnityEngine;

public class TimedActivator : MonoBehaviour
{
    private GameObject target;
    private float delay;

    public void Init(GameObject targetToEnable, float delaySeconds)
    {
        target = targetToEnable;
        delay = delaySeconds;
        StartCoroutine(Run());
    }

    private IEnumerator Run()
    {
        yield return new WaitForSeconds(delay);
        if (target != null) target.SetActive(true);
        Destroy(gameObject);
    }
}
