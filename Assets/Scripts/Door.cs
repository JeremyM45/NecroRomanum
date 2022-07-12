using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Door : MonoBehaviour
{
  [SerializeField] private int cost;
  [SerializeField] private float range;
  [SerializeField] LayerMask playerLayer;
  [SerializeField] TextMeshProUGUI textDisplay;
  private Score playerScore;
  private bool playerStillInRange;
  // Start is called before the first frame update
  void Start()
  {
    playerScore = GameObject.Find("Player").GetComponent<Score>();
    StartCoroutine(PlayerCheck());
    textDisplay.SetText("");
  }
  IEnumerator PlayerCheck()
  {
    WaitForSeconds wait = new WaitForSeconds(0.2f);
    while(true)
    {
      yield return wait;
      bool playerInRange = Physics.CheckSphere(transform.position, range, playerLayer);
      if(playerInRange && !playerStillInRange)
      {
        playerStillInRange = true;
        textDisplay.SetText("Press 'E' to Open Door for " + cost + " points");
        if(Input.GetKey(KeyCode.E) && playerScore.CurrentScore >= cost)
        {
          Destroy(gameObject);
        }
      }
      else if(!playerInRange && playerStillInRange)
      {
        playerStillInRange = false;
        textDisplay.SetText("");
      }
    }
  }
}
