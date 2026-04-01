using Newtonsoft.Json;
using Photon.Pun;
using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
# if UNITY_EDITOR
using static UnityEditor.PlayerSettings;
# endif

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

        [Space]
        [Header("Cosmetics to check")]
        [Tooltip("The cosmetics that will NEVER be trusted, meaning the client will ask playfab if the client owns the cosmetic, before putting it on for that player.")]
        public List<string> alwaysCheckThese;

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

            // It will delete automatically when you leave the room
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

        private async void _RefreshPlayerValues()
        {
            // Name
            if (NameText != null)
                NameText.text = photonView.Owner.NickName;

            // Colour
            foreach (SkinnedMeshRenderer renderer in ColourObjects)
                if (renderer != null)
                    renderer.material.color = JsonUtility.FromJson<Color>(
                        (string)photonView.Owner.CustomProperties["Colour"]
                    );

            // Cosmetics — verify each slot token before rendering
            Dictionary<string, string> cosmetics = 
                (Dictionary<string, string>)photonView.Owner.CustomProperties["Cosmetics"];
            Dictionary<string, string> tokens = 
                (Dictionary<string, string>)photonView.Owner.CustomProperties["CosmeticTokens"];

            if (cosmetics == null) return;

            foreach (KeyValuePair<string, string> cosmetic in cosmetics)
            {
                string slotName = cosmetic.Key;
                string claimedCosmeticId = cosmetic.Value;
                string token = (tokens != null && tokens.ContainsKey(slotName)) ? tokens[slotName] : null;

                bool isValid = false;
                if (!string.IsNullOrEmpty(token))
                    isValid = await VerifyCosmetic(token, claimedCosmeticId, slotName);

                foreach (CosmeticSlot slot in CosmeticSlots)
                {
                    if (slot.SlotName != slotName) continue;
                    foreach (Transform cos in slot.Object)
                        cos.gameObject.SetActive(isValid && cos.name == claimedCosmeticId);
                }
            }
        }

        private static readonly HttpClient _http = new HttpClient();

        private async Task<bool> VerifyCosmetic(string token, string claimedCosmeticId, string claimedSlot)
        {
            try
            {
                var payload = JsonConvert.SerializeObject(new { token });
                var content = new StringContent(payload, Encoding.UTF8, "application/json");

                var response = await _http.PostAsync("https://api.unixityyy.dev/api/v1/cosmetic/verify", content);
                if (!response.IsSuccessStatusCode) return false;

                var json = JsonConvert.DeserializeObject<Dictionary<string, object>>(
                    await response.Content.ReadAsStringAsync()
                );

                return (bool)json["valid"]
                    && (string)json["cosmeticId"] == claimedCosmeticId
                    && (string)json["slotName"] == claimedSlot;
            }
            catch (Exception e)
            {
                Debug.LogWarning($"VerifyCosmetic failed for slot {claimedSlot}: {e.Message}");
                return false;
            }
        }

        [Serializable]
        public class CosmeticSlot
        {
            public string SlotName;
            public Transform Object;
        }

        [Serializable]
        public class PlayerOwnsCosmeticResult
        {
            public bool ownsCosmetic;
        }
    }
}