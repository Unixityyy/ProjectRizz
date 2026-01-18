using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableCosmetic : MonoBehaviour
{

   public GameObject cosmeticToDisable;


   void OnTriggerEnter()
   {
     cosmeticToDisable.SetActive(false);
   }


   void OnTriggerExit()
   {
      cosmeticToDisable.SetActive(false);
   }
}
