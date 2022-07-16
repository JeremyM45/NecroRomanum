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
      currentGun.damage = 8;
      currentGun.maxTotalAmmo = 160;
      currentGun.maxRoundsPerMag = 12;
      currentGun.reloadTime = 0.5f;
    } 
    else
    {
      currentGun.damage = 8;
      currentGun.maxTotalAmmo = 240;
      currentGun.maxRoundsPerMag = 16;
      currentGun.isAutomatic = true;
      currentGun.timeBetweenShots = 0.08f;
    }
  }
  private void UpgradeM1(bool isFirstTimeUpgrade)
  {
    if(isFirstTimeUpgrade)
    {
      currentGun.damage = 20;
      currentGun.maxTotalAmmo = 120;
      currentGun.maxRoundsPerMag = 12;
      currentGun.penetrationAmount = 8;
      currentGun.reloadTime = 1f;
    } 
    else
    {
      currentGun.damage = 20;
      currentGun.maxTotalAmmo = 360;
      currentGun.maxRoundsPerMag = 36;
      currentGun.isBurst = true;
      currentGun.roundsInBurst = 3;
      currentGun.timeBetweenShotsInBurst = 0.05f;
    }
  }
  private void UpgradeDB(bool isFirstTimeUpgrade)
  {
    if(isFirstTimeUpgrade)
    {
      currentGun.damage = 2;
      currentGun.penetrationAmount = 2;
      currentGun.maxTotalAmmo = 92;
      currentGun.maxRoundsPerMag = 4;
      currentGun.reloadTime = 0.7f;
    }
    else
    {
      currentGun.maxTotalAmmo = 128;
      currentGun.maxRoundsPerMag = 8;
      currentGun.isAutomatic = true;
    }
  }
  private void UpgradeThompson(bool isFirstTimeUpgrade)
  {
    if(isFirstTimeUpgrade)
    {
      currentGun.damage = 6;
      currentGun.penetrationAmount = 2;
      currentGun.maxTotalAmmo = 500;
      currentGun.maxRoundsPerMag = 100;
      currentGun.timeBetweenShots = 0.07f;
    }
    else
    {
      currentGun.maxTotalAmmo = 0;
      currentGun.maxRoundsPerMag = 999;
      currentGun.magOnly = true;
      currentGun.timeBetweenShots = 0.04f;
      currentGun.penetrationAmount = 4;
    }
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
