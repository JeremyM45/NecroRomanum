using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UpgradeGun : MonoBehaviour
{
  [SerializeField] private TextMeshProUGUI textDisplay;
  private GameObject player;
  private Score playerScore;
  private Gun currentGun;
  private GameObject baseGun;
  private GameObject firstUpgrade;
  private GameObject secondUpgrade;
  private bool canUpgrade = true;
  private int cost;
  void Start()
  {
    player = GameObject.Find("Player");
    playerScore = player.GetComponent<Score>();
  }
  void OnTriggerStay(Collider obj)
  {
    if(canUpgrade)
    {
      currentGun = player.GetComponentInChildren<Gun>();
      baseGun = currentGun.transform.Find(currentGun.name + "BaseMesh").gameObject;
      firstUpgrade = currentGun.transform.Find(currentGun.name + " First Upgrade").gameObject;
      secondUpgrade = currentGun.transform.Find(currentGun.name + " Second Upgrade").gameObject;
      if(obj.transform.name == "Player")
      {
        if(baseGun.activeSelf == true)
        {
          cost = 250;
          if(Input.GetKeyDown(KeyCode.E) && playerScore.CurrentScore >= cost)
          {
            canUpgrade = false;
            if(currentGun.name == "M1911")
            {
              UpgradeM1911();
            }
            currentGun.transform.Find(currentGun.name + " First Upgrade").gameObject.SetActive(true);
            currentGun.transform.Find(currentGun.name + "BaseMesh").gameObject.SetActive(false);
            playerScore.removePoints(cost);
            Invoke("ResetCooldown", 0.5f);
          }
          
        }
        else if(firstUpgrade.activeSelf == true)
        {
          cost = 500;
          if(Input.GetKeyDown(KeyCode.E) && playerScore.CurrentScore >= cost)
          {
            canUpgrade = false;
            if(currentGun.name == "M1911")
            {
              UpgradeM1911();
            }
            currentGun.transform.Find(currentGun.name + " First Upgrade").gameObject.SetActive(false);
            currentGun.transform.Find(currentGun.name + " Second Upgrade").gameObject.SetActive(true);
            playerScore.removePoints(cost);
            Invoke("ResetCooldown", 0.5f);
          }
        }
        textDisplay.SetText("Press 'E' To Upgrade " + currentGun.name + " for " + cost + " points");
        if(secondUpgrade.activeSelf == true)
        {
          textDisplay.SetText("");
        }
      }
    }
  }
  private void OnTriggerExit()
  {
    textDisplay.SetText("");
  }
  private void ResetCooldown()
  { 
    canUpgrade = true;
  }
  private void UpgradeM1911()
  {
    if(baseGun.activeSelf == true)
    {
      currentGun.damage = 6;
      currentGun.maxTotalAmmo = 160;
      currentGun.maxRoundsPerMag = 12;
    } 
    else if(firstUpgrade.activeSelf == true)
    {
      currentGun.damage = 8;
      currentGun.maxTotalAmmo = 240;
      currentGun.maxRoundsPerMag = 16;
      currentGun.isAutomatic = true;
    }
    currentGun.FillAmmo();
  }
}
