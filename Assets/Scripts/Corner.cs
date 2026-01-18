using System.Collections;
using UnityEngine;
using Oculus.Platform;

public class PlayerColliderCheck : MonoBehaviour
{
    private bool isPlayerInside = false;
    private Coroutine checkCoroutine;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("entered");
        if (other.CompareTag("Body"))
        {
            isPlayerInside = true;
            if (checkCoroutine == null)
            {
                checkCoroutine = StartCoroutine(CheckPlayerInCollider());
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Body"))
        {
            isPlayerInside = false;
            if (checkCoroutine != null)
            {
                StopCoroutine(checkCoroutine);
                checkCoroutine = null;
            }
        }
    }

    private IEnumerator CheckPlayerInCollider()
    {
        yield return new WaitForSeconds(3f);
        if (isPlayerInside)
        {
            Debug.Log("corner 3 secs yay");
            if (Core.IsInitialized())
            {
                Achievements.Unlock("cornerSit");
            }
        }
        checkCoroutine = null;
    }
}
