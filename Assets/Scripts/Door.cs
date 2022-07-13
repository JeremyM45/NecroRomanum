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
  private bool bought;
  // Start is called before the first frame update
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
        Invoke("DestroyDoor", 0.2f);
      }
    }
  }
  void OnTriggerExit()
  {
    textDisplay.SetText("");
  }
  void DestroyDoor()
  {
    Destroy(gameObject);
  }
}
