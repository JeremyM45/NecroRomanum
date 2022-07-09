using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
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
      areaClear = !Physics.CheckSphere(transform.position, 5f, blockingObjects);
      if(areaClear && globalSpawnLogic.NumOfEnemiesToSpawn > 0 && !globalSpawnLogic.NewRoundCooldown)
      {
        Instantiate(objectToSpawn, transform.position, Quaternion.identity);
        globalSpawnLogic.NumOfEnemiesToSpawn--;
      }
    }
  }
}
