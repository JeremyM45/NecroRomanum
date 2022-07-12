using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
  [SerializeField] private int cost;
  private Score playerScore;
  // Start is called before the first frame update
  void Start()
  {
    playerScore = GameObject.Find("Player").GetComponent<Score>();
  }
  IEnumerator PlayerCheck()
  {
    WaitForSeconds wait = new WaitForSeconds(1f); 
    yield return
  }
}
