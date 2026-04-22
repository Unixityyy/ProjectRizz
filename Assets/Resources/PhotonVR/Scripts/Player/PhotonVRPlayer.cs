using Photon.Pun;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Photon.VR.Player
{
    public class PhotonVRPlayer : MonoBehaviourPun
    {
        [Header("Objects")]
        public Transform Head;
        public Transform Body;
        public Transform LeftHand;
        public Transform RightHand;
        [Tooltip("The objects that will get the colour of the player applied to them")]
        public List<SkinnedMeshRenderer> ColourObjects;

        [Space]
        [Tooltip("Feel free to add as many slots as you feel necessary")]
        public List<CosmeticSlot> CosmeticSlots = new List<CosmeticSlot>();

        [Header("Other")]
        public TextMeshPro NameText;
        public bool HideLocalPlayer = true;

        private void Awake()
        {
            if (photonView.IsMine)
            {
                PhotonVRManager.Manager.LocalPlayer = this;
                if (HideLocalPlayer)
                {
                    Head.gameObject.SetActive(false);
                    Body.gameObject.SetActive(false);
                    RightHand.gameObject.SetActive(false);
                    LeftHand.gameObject.SetActive(false);
                    NameText.gameObject.SetActive(false);
                }
            }

            DontDestroyOnLoad(gameObject);
            _RefreshPlayerValues();
        }

        private void Update()
        {
            if (photonView.IsMine)
            {
                Head.transform.position = PhotonVRManager.Manager.Head.transform.position;
                Head.transform.rotation = PhotonVRManager.Manager.Head.transform.rotation;

                RightHand.transform.position = PhotonVRManager.Manager.RightHand.transform.position;
                RightHand.transform.rotation = PhotonVRManager.Manager.RightHand.transform.rotation;

                LeftHand.transform.position = PhotonVRManager.Manager.LeftHand.transform.position;
                LeftHand.transform.rotation = PhotonVRManager.Manager.LeftHand.transform.rotation;
            }
        }

        public void RefreshPlayerValues() => photonView.RPC("RPCRefreshPlayerValues", RpcTarget.All);

        [PunRPC]
        private void RPCRefreshPlayerValues()
        {
            _RefreshPlayerValues();
        }

        private void _RefreshPlayerValues()
        {
            if (NameText != null)
                NameText.text = photonView.Owner.NickName;

            if (photonView.Owner.CustomProperties.ContainsKey("Colour"))
            {
                foreach (SkinnedMeshRenderer renderer in ColourObjects)
                {
                    if (renderer != null)
                    {
                        renderer.material.color = JsonUtility.FromJson<Color>(
                            (string)photonView.Owner.CustomProperties["Colour"]
                        );
                    }
                }
            }

            if (photonView.Owner.CustomProperties.ContainsKey("Cosmetics"))
            {
                Dictionary<string, string> cosmetics = (Dictionary<string, string>)photonView.Owner.CustomProperties["Cosmetics"];
                if (cosmetics == null) return;

                foreach (KeyValuePair<string, string> cosmetic in cosmetics)
                {
                    foreach (CosmeticSlot slot in CosmeticSlots)
                    {
                        if (slot.SlotName == cosmetic.Key)
                        {
                            foreach (Transform cos in slot.Object)
                            {
                                cos.gameObject.SetActive(cos.name == cosmetic.Value);
                            }
                        }
                    }
                }
            }
        }

        [Serializable]
        public class CosmeticSlot
        {
            public string SlotName;
            public Transform Object;
        }
    }
}