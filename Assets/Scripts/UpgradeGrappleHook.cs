using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UpgradeGrappleHook : MonoBehaviour
{
  [SerializeField] private TextMeshProUGUI textDisplay;
  [SerializeField] private int cost;
  private Score playerScore;
  private GrapplingHook playerGrapplingHook;
  private bool canPurchase = true;
  void Start()
  {
    playerScore = GameObject.Find("Player").GetComponent<Score>();
    playerGrapplingHook = GameObject.Find("Player").GetComponent<GrapplingHook>();
  }
  void OnTriggerStay(Collider obj)
  {
    if(obj.name == "Player" && canPurchase)
    {
      if(!playerGrapplingHook.GrapplingHookUnlocked)
      {
        textDisplay.SetText("Press 'E' to Unlock Grappling Hook for " + cost + " points");
        if(Input.GetKey(KeyCode.E) && playerScore.CurrentScore >= cost)
        {
          playerGrapplingHook.GrapplingHookUnlocked = true;
          canPurchase = false;
          Invoke("ResetCanPurchase", 2f);
          playerScore.removePoints(cost);
          cost *= 2;
          textDisplay.SetText("");
        }
      }
      else
      {
        textDisplay.SetText("Press 'E' to Upgrade Grappling Hook for " + cost + " points");
        if(Input.GetKey(KeyCode.E) && playerScore.CurrentScore >= cost)
        {
          playerGrapplingHook.MaxDistance = 50;
          playerGrapplingHook.GrapplingCooldown = 1f;
          canPurchase = false;
          playerScore.removePoints(cost);
          transform.GetComponent<BoxCollider>().enabled = false;
          textDisplay.SetText("");
        }
      }
    }
  }
  void OnTriggerExit(Collider obj)
  {
    if(obj.name == "Player")
    {
      textDisplay.SetText("");
    }
  }
  void ResetCanPurchase()
  {
    canPurchase = true;
  }
}
