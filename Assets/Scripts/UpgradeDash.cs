using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UpgradeDash : MonoBehaviour
{
  [SerializeField] private TextMeshProUGUI textDisplay;
  [SerializeField] private int cost;
  private Score playerScore;
  private PlayerController playerController;
  private bool canPurchase = true;
  void Start()
  {
    playerScore = GameObject.Find("Player").GetComponent<Score>();
    playerController = GameObject.Find("Player").GetComponent<PlayerController>();
  }
  void OnTriggerStay(Collider obj)
  {
    if(obj.name == "Player" && canPurchase)
    {
      textDisplay.SetText("Press 'E' to Upgrade Dash for " + cost + " points");
      if(Input.GetKey(KeyCode.E) && playerScore.CurrentScore >= cost)
      {
        playerController.MaxDashCharges = 2;
        playerController.DashCharges = 2;
        playerController.DashCooldown = 2f;
        canPurchase = false;
        playerScore.removePoints(cost);
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
