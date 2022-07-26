using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class UpgradeHealth : MonoBehaviour
{
  [SerializeField] private TextMeshProUGUI textDisplay;
  [SerializeField] private int cost;
  private Score playerScore;
  private LifeAndDeath playerLad;
  private bool canPurchase = true;
  void Start()
  {
    playerScore = GameObject.Find("Player").GetComponent<Score>();
    playerLad = GameObject.Find("Player").GetComponent<LifeAndDeath>();
  }
  void OnTriggerStay(Collider obj)
  {
    if(obj.name == "Player" && canPurchase)
    {
      textDisplay.SetText("Press 'E' to Upgrade Health for " + cost + " points");
      if(Input.GetKey(KeyCode.E) && playerScore.CurrentScore >= cost)
      {
        int playerMaxHealth = playerLad.MaxHealth;
        switch(playerMaxHealth)
        {
          case 50:
            playerLad.SetMaxHealth(100);
            cost *= 2;
            textDisplay.SetText("");
            break;
          case 100:
            playerLad.SetMaxHealth(200);
            textDisplay.SetText("");
            transform.GetComponent<BoxCollider>().enabled = false;
            break;
        }
        canPurchase = false;
        playerScore.removePoints(cost);
        Invoke("ResetCanPurchase", 2f);
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
