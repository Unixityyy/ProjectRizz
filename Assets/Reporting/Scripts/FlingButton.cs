using UnityEngine;

public class FlingButton : MonoBehaviour
{
    [SerializeField] public int ButtonNumber;
    [SerializeField] public LeaderBoard LB;
    [SerializeField] public string HandTag = "HandTag";

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(HandTag))
        {
            LB.FlingPress(ButtonNumber);
        }
    }

}

