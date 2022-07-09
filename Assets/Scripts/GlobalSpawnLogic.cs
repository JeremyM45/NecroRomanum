using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class GlobalSpawnLogic : MonoBehaviour
{
  public int NumOfEnemiesAlive {get; private set;}
  private EnemyAi[] enemies;
  private List<EnemyAi> enemiesAlive = new List<EnemyAi>();
  // Start is called before the first frame update
  void Start()
  {
    StartCoroutine(NumOfEnemiesAliveCheck());
  }

  // Update is called once per frame
  IEnumerator NumOfEnemiesAliveCheck()
  {
    WaitForSeconds wait = new WaitForSeconds(1f);
    while(true)
    {
      yield return wait;
      Debug.Log("Ran Check");
      enemiesAlive.Clear();
      enemies = FindObjectsOfType<EnemyAi>();
      foreach(EnemyAi enemy in enemies)
      {
        if(enemy.Alive)
        {
          enemiesAlive.Add(enemy);
        }
      }
      NumOfEnemiesAlive = enemiesAlive.Count;
      Debug.Log("Num of Enemies : " + NumOfEnemiesAlive);
    }
  }
}
