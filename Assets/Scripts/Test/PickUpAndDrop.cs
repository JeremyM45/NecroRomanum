using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpAndDrop : MonoBehaviour
{
  [SerializeField] Inputs input;
  [SerializeField] Transform PlayerCam;
  [SerializeField] float dropForwardForce;
  [SerializeField] float dropUpwardForce;
  [SerializeField] float pickUpRangeFloat;
  [SerializeField] List<ProjectileGun> gunsInPickUpRange;
  [SerializeField] LayerMask whatIsGun;
  [SerializeField] ProjectileGun closestGun;
  private ProjectileGun currentWeapon;
  private Rigidbody rb; 


  // Update is called once per frame
  void Update()
  {
    if(Input.GetKeyDown(KeyCode.C))
    {
      InPickUpRange();
    }
    if(Input.GetKeyDown(KeyCode.X))
    {
      ClosestGunInPickUpRange();
    }
    if(Input.GetKeyDown(input.dropKey))
    {
      Drop();
    }
    if(Input.GetKeyDown(input.pickUpKey) && closestGun != null)
    {
      PickUp(closestGun);
    }
  }
  void FixedUpdate()
  {
    InPickUpRange();
    if(gunsInPickUpRange.Count == 0)
    {
      closestGun = null;
    }
    else if(gunsInPickUpRange.Count > 0)
    {
      ClosestGunInPickUpRange();
    }
  }
  private void Drop()
  {
    currentWeapon = GetComponent<PlayerInventory>().GetCurrentWeapon();
    rb = currentWeapon.GetComponent<ProjectileGun>().GetRigidbody();
    currentWeapon.SetEquipped(false);
    rb.isKinematic = false;
    currentWeapon.GetCollider().isTrigger = false;
    currentWeapon.transform.SetParent(null);
    rb.AddForce(PlayerCam.forward * dropForwardForce, ForceMode.Impulse);
    rb.AddForce(PlayerCam.up * dropUpwardForce, ForceMode.Impulse);
    float rnd = UnityEngine.Random.Range(-1f, 1f);
    rb.AddTorque(new Vector3(rnd, rnd, rnd) * 10);
    GetComponent<PlayerInventory>().AddBlankWeapon();
    
  }
  private void InPickUpRange()
  {
    Collider[] guns = Physics.OverlapSphere(transform.position, pickUpRangeFloat, whatIsGun);
    foreach(Collider gun in guns)
    {
      ProjectileGun triggeredGun = gun.GetComponent<ProjectileGun>();
      if(!triggeredGun.GetEquipped() && !triggeredGun.GetInPickUpRange())
      {
        gunsInPickUpRange.Add(triggeredGun);
        triggeredGun.SetInPickUpRange(true);
      }
    }
    foreach(ProjectileGun gun in gunsInPickUpRange.ToList())
    {
      if(gun.GetEquipped())
      {
        gunsInPickUpRange.Remove(gun);
        gun.SetInPickUpRange(false);
      }
      if(!Array.Exists(guns, element => element == gun.GetCollider()))
      {
        gunsInPickUpRange.Remove(gun);
        gun.SetInPickUpRange(false);
      }
    }
  } 
  private void ClosestGunInPickUpRange()
  {
    for(int i = 0; i < gunsInPickUpRange.Count; i++)
    {
      if(i == 0)
      {
        closestGun = gunsInPickUpRange[i];
      }
      else
      {
        Vector3 distanceToPlayer = PlayerCam.position - gunsInPickUpRange[i].transform.position;
        Vector3 lastGunsDistanceToPlayer = PlayerCam.position - gunsInPickUpRange[i - 1].transform.position;
        if(distanceToPlayer.magnitude < lastGunsDistanceToPlayer.magnitude)
        {
          closestGun = gunsInPickUpRange[i];
        }
      }
    }
  }
  public void PickUp(ProjectileGun gun)
  {
    Drop();
    gun.transform.SetParent(GetComponent<PlayerInventory>().GetGunContainer());
    gun.transform.localPosition = Vector3.zero;
    gun.transform.localRotation = Quaternion.Euler(Vector3.zero);
    transform.localScale = Vector3.one;
    gun.GetRigidbody().isKinematic = true;
    gun.GetCollider().isTrigger = true;
    gun.SetEquipped(true);
    GetComponent<PlayerInventory>().SetWeapon(gun);
  }
}
