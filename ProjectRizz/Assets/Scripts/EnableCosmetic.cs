using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableCosmetic : MonoBehaviour
{
 public GameObject cosmeticToEnable;


 public void OnTriggerEnter()
 { 
    cosmeticToEnable.SetActive(true);
 } 


 public void OnTriggerExit() 
 { 
    cosmeticToEnable.SetActive(true); 
 }


}