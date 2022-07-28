using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
  public bool unlocked;
  [SerializeField] float cooldown;
  [SerializeField] private LayerMask blockingObjects;
  [SerializeField] private GameObject[] objectsToSpawn;
  private GlobalSpawnLogic globalSpawnLogic;
  private int currentRound;
  private float cooldownWait;
  private bool areaClear;
  
  void Start()
  {
    globalSpawnLogic = GameObject.Find("GlobalSpawnLogic").GetComponent<GlobalSpawnLogic>();
    StartCoroutine(SpawnCheck());
  }
  private void SpawnHelemeted()
  {
    Instantiate(objectsToSpawn[0], transform.position, Quaternion.identity);
    globalSpawnLogic.NumOfEnemiesToSpawn--;
    globalSpawnLogic.NumOfEnemiesAlive++;
    globalSpawnLogic.HelmetedEnemiesToSpawn--;
    globalSpawnLogic.HelmetedEnemiesAlive++;
  }
  private void SpawnNonHelemeted()
  {
    Instantiate(objectsToSpawn[1], transform.position, Quaternion.identity);
    globalSpawnLogic.NumOfEnemiesToSpawn--;
    globalSpawnLogic.NumOfEnemiesAlive++;
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
          if(globalSpawnLogic.HelmetedEnemiesToSpawn > 0)
          {
            int rng = Random.Range(0, 2);
            if(rng == 0 || globalSpawnLogic.NonHelmetedEnemiesToSpawn <= 0)
            {
              SpawnHelemeted();
            }
            else
            {
              SpawnNonHelemeted();
            }
          }
          else
          {
            SpawnNonHelemeted();
          }
          
        }
      }
    }
  }
}
