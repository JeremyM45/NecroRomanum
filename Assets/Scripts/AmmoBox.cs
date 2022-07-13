using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AmmoBox : MonoBehaviour
{
  [SerializeField] private float range;
  [SerializeField] private int cost;
  [SerializeField] private LayerMask playerLayerMask;
  [SerializeField] private TextMeshProUGUI textDisplay;
  private GameObject player;
  private Score playerScore;
  private Gun playerGun;
  private bool playerInRange;
  private bool playerStillInRange = false;
  void Start()
  {
    player = GameObject.Find("Player");
    playerScore = player.GetComponent<Score>();
  }
  void OnTriggerStay(Collider obj)
  {
    Gun currentGun = player.GetComponentInChildren<Gun>();
    if(obj.transform.name == "Player")
    {
      textDisplay.SetText("Press 'E' To Fill Ammo for " + cost + " points");
      Debug.Log(playerScore.CurrentScore);
      if(Input.GetKey(KeyCode.E) && playerScore.CurrentScore >= cost && !currentGun.IsAmmoFull())
      {
        player.GetComponentInChildren<Gun>().FillAmmo();
        playerScore.removePoints(cost);
      }
    }
  }
  void OnTriggerExit()
  {
    textDisplay.SetText("");
  }
}

