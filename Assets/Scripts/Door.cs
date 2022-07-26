using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Door : MonoBehaviour
{
  [SerializeField] private int cost;
  [SerializeField] LayerMask playerLayer;
  [SerializeField] TextMeshProUGUI textDisplay;
  [SerializeField] EnemySpawner[] spawners;
  [SerializeField] Door OtherDoorToUnlock;
  private Score playerScore;
  private bool bought;
  void Start()
  {
    playerScore = GameObject.Find("Player").GetComponent<Score>();
    textDisplay.SetText("");
    bought = false;
  }
  void OnTriggerStay(Collider obj)
  {
    if(obj.transform.name == "Player" && !bought)
    {
      textDisplay.SetText("Press 'E' to Open Door for " + cost + " points");
      if(Input.GetKey(KeyCode.E) && playerScore.CurrentScore >= cost)
      {
        textDisplay.SetText("");
        bought = true;
        playerScore.removePoints(cost);
        if(OtherDoorToUnlock != null)
        {
          OtherDoorToUnlock.DestroyDoor();
        }
        Invoke("DestroyDoor", 0.2f);
      }
    }
  }
  void OnTriggerExit()
  {
    textDisplay.SetText("");
  }
  public void DestroyDoor()
  {
    foreach(EnemySpawner spawner in spawners)
    {
      spawner.unlocked = true;
    }
    Destroy(gameObject);
  }
}
