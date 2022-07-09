using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickUp : MonoBehaviour
{
  [SerializeField] LayerMask whatIsHealthPack;
  [SerializeField] Transform player;
  [SerializeField] float pickUpRadius;
  // Start is called before the first frame update
  void Start()
  {
    
  }

  // Update is called once per frame
  void Update()
  {
    HealthPackInRange();
  }
  private void HealthPackInRange()
  {
    Collider[] healthPacksInRange = Physics.OverlapSphere(player.position, pickUpRadius, whatIsHealthPack);
    if(healthPacksInRange.Length > 0)
    {
      GetComponent<LifeAndDeath>().RestoreHealth();
      Destroy(healthPacksInRange[0].gameObject);
    }
  }
}
