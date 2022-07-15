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
        }
        else if(firstUpgrade.activeSelf == true)
        {
          cost = 500;
        }
        textDisplay.SetText("Press 'E' To Upgrade " + currentGun.name + " for " + cost + " points");
        if(secondUpgrade.activeSelf == true)
        {
          textDisplay.SetText("");
        }
        if(Input.GetKey(KeyCode.E) && playerScore.CurrentScore >= cost)
        {
          canUpgrade = false;
          UpgradeCurrentGun();
          playerScore.removePoints(cost);
          Invoke("ResetCooldown", 0.5f);
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
  private void UpgradeCurrentGun()
  {
    bool isFirstTimeUpgrade = IsFirstTimeUpgrade();
    if(currentGun.name == "M1911")
    {
      UpgradeM1911(isFirstTimeUpgrade);
    }
    else if(currentGun.name == "M1Garand")
    {
      UpgradeM1(isFirstTimeUpgrade);
    }
    else if(currentGun.name == "DoubleBarrel")
    {
      UpgradeDB(isFirstTimeUpgrade);
    }
    else if(currentGun.name == "Thompson")
    {
      UpgradeThompson(isFirstTimeUpgrade);
    }
    else
    {
      Debug.Log("Name Match Fail");
    }
    SetNewUpgradedGun(isFirstTimeUpgrade);
    currentGun.FillAmmo();
  }
  private void UpgradeM1911(bool isFirstTimeUpgrade)
  {
    
    if(isFirstTimeUpgrade)
    {
      currentGun.damage = 6;
      currentGun.maxTotalAmmo = 160;
      currentGun.maxRoundsPerMag = 12;
    } 
    else
    {
      currentGun.damage = 8;
      currentGun.maxTotalAmmo = 240;
      currentGun.maxRoundsPerMag = 16;
      currentGun.isAutomatic = true;
    }
  }
  private void UpgradeM1(bool isFirstTimeUpgrade)
  {

  }
  private void UpgradeDB(bool isFirstTimeUpgrade)
  {
    
  }
  private void UpgradeThompson(bool isFirstTimeUpgrade)
  {
    
  }
  private void SetNewUpgradedGun(bool firstTimeUpgrade)
  {
    if(firstTimeUpgrade)
    {
      firstUpgrade.gameObject.SetActive(true);
      baseGun.gameObject.SetActive(false);
    }
    else
    {
      secondUpgrade.gameObject.SetActive(true);
      firstUpgrade.gameObject.SetActive(false);
    }
  }
  private bool IsFirstTimeUpgrade()
  {
    if(baseGun.activeSelf == true)
    {
      return true;
    }
    else
    {
      return false;
    }
  }
}
