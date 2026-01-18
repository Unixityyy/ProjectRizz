using GorillaLocomotion;
using Photon.Pun;
using Photon.VR;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

public class JSMaterialSetter : MonoBehaviour
{
    private ColorScript colorScript;
    public List<Material> materials;
    public PhotonView pv;
    public Player player;
    public SkinnedMeshRenderer robot2;
    private Color nonDefault = new Color(0.9f, 0.9f, 0.9f);
    private Color? lastSetColor;

    private void Start()
    {
        if (pv == null)
        {
            pv = GetComponent<PhotonView>();
        }
    }

    private void Update()
    {
        if (player == null)
        {
            if (pv.IsMine)
            {
                player = Player.Instance;
            }
        }

        if (colorScript == null)
        {
            colorScript = ColorScript.instance;
        }

        if (PhotonNetwork.IsConnected)
        {
            if (pv == null || !pv.IsMine || robot2 == null) return;
            if (materials == null) return;
            if (player == null || colorScript == null) return;

            if (pv.IsMine)
            {
                try
                {
                    if (player.JSMaterial >= 0 && player.JSMaterial < materials.Count)
                    {
                        if (player.JSMaterial == 0) 
                        {
                            float TrueRed = colorScript.Red / 10f;
                            float TrueBlue = colorScript.Blue / 10f;
                            float TrueGreen = colorScript.Green / 10f;
                            Color myColour = new Color(TrueRed, TrueGreen, TrueBlue);

                            if (myColour != lastSetColor)
                            {
                                robot2.material = materials[player.JSMaterial];
                                Debug.Log("setting color to "+ myColour);
                                lastSetColor = myColour;
                                PhotonVRManager.SetColour(myColour);
                            }
                        }
                        else
                        {
                            robot2.material = materials[player.JSMaterial];
                            lastSetColor = null;
                            PhotonVRManager.SetColour(nonDefault);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.Log("couldnt set mat, prob isnt in the list. err: "+ex);
                }
            }
        }
    }
}
