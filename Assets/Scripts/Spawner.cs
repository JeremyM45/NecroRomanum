using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
  [SerializeField] private LayerMask blockingObjects;
  [SerializeField] private GameObject objectToSpawn;
  private bool areaClear;
  void Update()
  {
    areaClear = !Physics.CheckSphere(transform.position, 5f, blockingObjects);
    if(areaClear)
    {
      Instantiate(objectToSpawn, transform.position, Quaternion.identity);
    }
  }
}
