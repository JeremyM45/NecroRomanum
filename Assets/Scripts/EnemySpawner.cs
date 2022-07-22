using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
  public bool unlocked;
  [SerializeField] float cooldown;
  [SerializeField] private LayerMask blockingObjects;
  [SerializeField] private GameObject objectToSpawn;
  private GlobalSpawnLogic globalSpawnLogic;
  private int currentRound;
  private float cooldownWait;
  private bool areaClear;
  
  void Start()
  {
    globalSpawnLogic = GameObject.Find("GlobalSpawnLogic").GetComponent<GlobalSpawnLogic>();
    StartCoroutine(SpawnCheck());
  }
  IEnumerator SpawnCheck()
  {
    WaitForSeconds wait = new WaitForSeconds(cooldown);
    while(true)
    {
      yield return wait;
      if(unlocked)
      {
        areaClear = !Physics.CheckSphere(transform.position, 5f, blockingObjects);
        if(areaClear && globalSpawnLogic.NumOfEnemiesToSpawn > 0 && !globalSpawnLogic.NewRoundCooldown && globalSpawnLogic.NumOfEnemiesAlive < globalSpawnLogic.MaxEnemiesAlive)
        {
          Instantiate(objectToSpawn, transform.position, Quaternion.identity);
          globalSpawnLogic.NumOfEnemiesToSpawn--;
          globalSpawnLogic.NumOfEnemiesAlive++;
          globalSpawnLogic.enemyAliveCounterDisplay.SetText("Alive: " + globalSpawnLogic.NumOfEnemiesAlive);
        }
      }
    }
  }
}
