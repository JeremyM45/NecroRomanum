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
  void Start()
  {
    player = GameObject.Find("Player");
    playerScore = player.GetComponent<Score>();
  }
  void Update()
  {
    playerInRange = Physics.CheckSphere(transform.position, range, playerLayerMask);
    if(!playerInRange)
    {
      textDisplay.SetText("");
    }
    if(playerInRange)
    {
      textDisplay.SetText("Press 'E' To Fill Ammo for " + cost + " points");
      if(Input.GetKeyDown(KeyCode.E) && playerScore.CurrentScore >= cost)
      {
        player.GetComponentInChildren<Gun>().FillAmmo();
        playerScore.removePoints(cost);
      }
    }
  }
}
