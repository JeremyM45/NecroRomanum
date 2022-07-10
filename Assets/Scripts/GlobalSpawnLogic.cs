using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class GlobalSpawnLogic : MonoBehaviour
{
  public int NumOfEnemiesLeft {get; private set;}
  public int NumOfEnemiesToSpawn {get; set;}
  public bool NewRoundCooldown {get; private set;}
  public int Round {get; private set;}
  [SerializeField] private float newRoundSpawnerCooldown;
  [SerializeField] private TextMeshProUGUI roundDisplay;
  [SerializeField] private TextMeshProUGUI enemyCounterDisplay;
  private EnemyAi[] enemies;
  private List<EnemyAi> enemiesAlive = new List<EnemyAi>();
  
  // Start is called before the first frame update
  void Start()
  {
    StartCoroutine(RoundCheck());
    NewRoundCooldown = true;
    NumOfEnemiesLeft = 0;
  }
  public void EnemiesLeftCalc(int deathAmount)
  {
    NumOfEnemiesLeft -= deathAmount;
    enemyCounterDisplay.SetText("Enemies: " + NumOfEnemiesLeft);
  }
  public void RoundSettings()
  {
    switch(Round)
    {
      case 1:
        NumOfEnemiesToSpawn = 8;
        break;
      case 2:
        NumOfEnemiesToSpawn = 16;
        break;
      case 3:
        NumOfEnemiesToSpawn = 24;
        break;
      case 4:
        NumOfEnemiesToSpawn = 36;
        break;
      case 5:
        NumOfEnemiesToSpawn = 56;
        break;
    }
    if(Round > 5)
    {
      NumOfEnemiesToSpawn = 72;
    }
  }
  private void NewRound()
  {
    Round++;
    roundDisplay.SetText(Round.ToString());
    NewRoundCooldown = true;
    RoundSettings();
    NumOfEnemiesLeft = NumOfEnemiesToSpawn;
    enemyCounterDisplay.SetText("Enemies: " + NumOfEnemiesLeft);
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
      // Debug.Log("Ran Check");
      // enemiesAlive.Clear();
      // enemies = FindObjectsOfType<EnemyAi>();
      // foreach(EnemyAi enemy in enemies)
      // {
      //   if(enemy.Alive)
      //   {
      //     enemiesAlive.Add(enemy);
      //   }
      // }
      // NumOfEnemiesAlive = enemiesAlive.Count;
      // Debug.Log("Num of Enemies : " + NumOfEnemiesAlive);
    }
  }
}
