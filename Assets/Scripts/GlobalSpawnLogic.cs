using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class GlobalSpawnLogic : MonoBehaviour
{
  public int NumOfEnemiesLeft {get; private set;}
  public int MaxEnemiesAlive {get; private set;}
  public int NumOfEnemiesAlive {get; set;}
  public float MinSpeed {get; private set;}
  public float MaxSpeed {get; private set;}
  public int Hp {get; private set;}
  public int NumOfEnemiesToSpawn {get; set;}
  public int HelmetedEnemiesToSpawn {get; set;}
  public int NonHelmetedEnemiesToSpawn {get; set;}
  public int HelmetedEnemiesAlive {get; set;}
  public int Damage {get; set;}
  public bool NewRoundCooldown {get; private set;}
  public bool AllHelmeted {get; private set;}
  public int Round;
  [SerializeField] private float newRoundSpawnerCooldown;
  [SerializeField] private TextMeshProUGUI roundDisplay;
  private EnemyAi[] enemies;
  private List<EnemyAi> enemiesAlive = new List<EnemyAi>();
  void Start()
  {
    StartCoroutine(RoundCheck());
    NewRoundCooldown = true;
    NumOfEnemiesLeft = 0;
    NumOfEnemiesToSpawn = 0;
    NumOfEnemiesAlive = 0;
    HelmetedEnemiesToSpawn = 0;
    HelmetedEnemiesAlive = 0;
    roundDisplay.SetText("I");
    
  }
  public void EnemiesLeftCalc(int deathAmount)
  {
    NumOfEnemiesLeft -= deathAmount;
    NumOfEnemiesAlive -= deathAmount;
  }
  public void HelmetedEnemiesLeftCalc(int deathAmount)
  {
    NumOfEnemiesLeft -= deathAmount;
    NumOfEnemiesAlive -= deathAmount;
    HelmetedEnemiesAlive -= deathAmount;
  }
  public void RoundSettings()
  {
    switch(Round)
    {
      case 1:
        MaxEnemiesAlive = 4;
        Damage = 5;
        NumOfEnemiesToSpawn = 8;
        MinSpeed = 3;
        MaxSpeed = 5;
        Hp = 4;
        break;
      case 2:
        MaxEnemiesAlive = 8;
        NumOfEnemiesToSpawn = 16;
        HelmetedEnemiesToSpawn = 2;
        MinSpeed = 4;
        MaxSpeed = 6;
        Hp = 6;
        break;
      case 3:
        MaxEnemiesAlive = 12;
        NumOfEnemiesToSpawn = 24;
        HelmetedEnemiesToSpawn = 8;
        MinSpeed = 6;
        MaxSpeed = 8;
        Hp = 10;
        break;
      case 4:
        MaxEnemiesAlive = 18;
        NumOfEnemiesToSpawn = 36;
        HelmetedEnemiesToSpawn = 16;
        MaxSpeed = 10;
        Hp = 14;
        break;
      case 5:
        MaxEnemiesAlive = 24;
        NumOfEnemiesToSpawn = 64;
        HelmetedEnemiesToSpawn = 32;
        MaxSpeed = 12;
        Hp = 18;
        break;
    }
    if(Round > 10)
    {
      MaxEnemiesAlive = 48;
      NumOfEnemiesToSpawn = 256;
      HelmetedEnemiesToSpawn = 256;
      MaxSpeed = 16;
    }
    else if (Round > 8)
    {
      Damage = 10;
    }
    else if(Round > 5)
    {
      MaxEnemiesAlive = 36;
      NumOfEnemiesToSpawn = 128;
      HelmetedEnemiesToSpawn = 64;
      Hp = 22;
      MaxSpeed = 14;
    }
    NonHelmetedEnemiesToSpawn = NumOfEnemiesToSpawn - HelmetedEnemiesToSpawn;
  }
  private void NewRound()
  {
    Round++;
    string roundNum = IntToRoman(Round);
    roundDisplay.SetText(roundNum);
    NewRoundCooldown = true;
    RoundSettings();
    NumOfEnemiesLeft = NumOfEnemiesToSpawn;
    Invoke("NewRoundCooldownReset", newRoundSpawnerCooldown);
  }
  private void NewRoundCooldownReset()
  {
    NewRoundCooldown = false;
  }
  private IEnumerator RoundCheck()
  {
    WaitForSeconds wait = new WaitForSeconds(1f);
    while(true)
    {
      yield return wait;
      if(NumOfEnemiesLeft <= 0)
      {
        NewRound();
      }
    }
  }
  public string IntToRoman(int num) 
  {
    string result = "";
    while (num >= 1000) {
      result += "M";
      num -= 1000;            
    }
    while (num >= 100) {
      if (num >= 900) {           
        result += "CM";
        num -= 900;              
      }
      if (num >= 400 && num < 500) {               
        result += "CD";
        num -= 400;                
      }
      if (num >= 500) {                
        result += "D";
        num -= 500;              
      }   
      if (num >= 100) {                
        result += "C";
        num -= 100;           
      }
    }
    while (num >= 10) {
      if (num >= 90) {            
        result += "XC";
        num -= 90;                
      }
      if (num >= 40 && num < 50) {                
        result += "XL";
        num -= 40;                
      }
      if (num >= 50) {
        result += "L";
        num -= 50;
      }
      if (num >= 10) {
        result += "X";
        num -= 10;
      }         
    }
    while (num >= 1) {
      if (num == 9) {
        result += "IX";
        num -= 9;
      }
      if (num == 4) {
        result += "IV";
        num -= 4;
      }
      if (num >= 5) {                
        result += "V";
        num -= 5;                
      }
      if (num >= 1) {                
        result += "I";
        num -= 1;
      }
    }        
    return result;       
  }
}
