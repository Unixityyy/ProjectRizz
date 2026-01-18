using UnityEngine;
using Oculus.Platform;

public class DetectFall : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("HandTag"))
        {
            if (Core.IsInitialized())
            {
                Achievements.Unlock("outMap");
            }
            UnityEngine.Application.Quit();
        }
        else if (other.CompareTag("Grabbable"))
        {
            Rigidbody rb = other.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = true;
            }

            other.transform.position = new Vector3(10.5339193f, 3.42000008f, -8.64998913f);

            if (rb != null)
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.isKinematic = false;
            }
        }
    }
}
