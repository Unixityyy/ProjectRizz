using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FakeRig : MonoBehaviour
{
    public Transform fakeRig;
    public Transform fakePlayer;
    public Transform fakeHead;
    public Transform fakeLeft;
    public Transform fakeRight;
    public Transform rig;
    public Transform player;
    public Transform head;
    public Transform left;
    public Transform right;
    public ModMenu menuScript;
    
    private void setPos()
    {
        // Set rotations
        fakeRig.rotation = rig.rotation;
        fakePlayer.rotation = player.rotation;
        fakeHead.rotation = head.rotation;
        fakeLeft.rotation = left.rotation;
        fakeRight.rotation = right.rotation;

        // Set positions
        if (menuScript.invis)
        {
            fakeRig.position = new Vector3(rig.position.x, -6f, rig.position.z);
            fakePlayer.position = new Vector3(player.position.x, -6f, player.position.z);
            fakeHead.position = new Vector3(head.position.x, -6f, head.position.z);
            fakeLeft.position = new Vector3(left.position.x, -6f, left.position.z);
            fakeRight.position = new Vector3(right.position.x, -6f, right.position.z);
        }
        else
        {
            fakeRig.position = rig.position;
            fakePlayer.position = player.position;
            fakeHead.position = head.position;
            fakeLeft.position = left.position;
            fakeRight.position = right.position;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        setPos();
    }

    // Update is called once per frame
    void Update()
    {
        setPos();
    }
}
