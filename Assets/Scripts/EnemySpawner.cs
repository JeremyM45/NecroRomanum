using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
  [SerializeField] private LayerMask blockingObjects;
  [SerializeField] private GameObject objectToSpawn;
  private GlobalSpawnLogic globalSpawnLogic;
  private bool areaClear;
  void Start()
  {
    globalSpawnLogic = GameObject.Find("GlobalSpawnLogic").GetComponent<GlobalSpawnLogic>();
  }
  void Update()
  {
    areaClear = !Physics.CheckSphere(transform.position, 5f, blockingObjects);
    if(areaClear && globalSpawnLogic.NumOfEnemiesAlive <= 1)
    {
      Instantiate(objectToSpawn, transform.position, Quaternion.identity);
    }
  }
}
