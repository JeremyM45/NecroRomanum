using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WeaponUnlock : MonoBehaviour
{
  [SerializeField] Gun gunToUnlock;
  [SerializeField] TextMeshProUGUI textDisplay;
  [SerializeField] int cost;
  private Score playerScore;
  void Start()
  {
    playerScore = GameObject.Find("Player").GetComponent<Score>();
    textDisplay.SetText("");
  }
  private void OnTriggerStay(Collider obj)
  {
    if(obj.transform.name == "Player" && !gunToUnlock.CanEquip)
    {
      textDisplay.SetText("Press E to Buy " + gunToUnlock.name + " for " + cost + " Points");
      if(Input.GetKey(KeyCode.E) && playerScore.CurrentScore >= cost)
      {
        textDisplay.SetText("");
        playerScore.removePoints(cost);
        gunToUnlock.CanEquip = true;
      }
    }
  }
  void OnTriggerExit()
  {
    textDisplay.SetText("");
  }
}
