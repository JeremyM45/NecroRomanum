using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchWeapons : MonoBehaviour
{
  private Gun[] guns;
  private Animator playerAnimator;
  // Start is called before the first frame update
  void Start()
  {
    playerAnimator = GameObject.Find("Hands").GetComponent<Animator>();
    guns = transform.GetComponentsInChildren<Gun>();
    SetAllGunsInactive();
    guns[0].gameObject.SetActive(true);
  }

  // Update is called once per frame
  void Update()
  {
    HandleWeaponSwitch();
  }
    private void HandleWeaponSwitch()
  {
    if(Input.GetKeyDown(KeyCode.Alpha1) && guns[0].gameObject.activeSelf == false)
    {
      SetAllGunsInactive();
      playerAnimator.SetBool("M1911", true);
      guns[0].gameObject.SetActive(true);
      CheckIfReloading();
    }
    if(Input.GetKeyDown(KeyCode.Alpha2) && guns[1].gameObject.activeSelf == false)
    {
      
      SetAllGunsInactive();
      playerAnimator.SetBool("M1", true);
      guns[1].gameObject.SetActive(true);
      CheckIfReloading();
    }
    if(Input.GetKeyDown(KeyCode.Alpha3) && guns[2].gameObject.activeSelf == false)
    {
      SetAllGunsInactive();
      playerAnimator.SetBool("DB", true);
      guns[2].gameObject.SetActive(true);
      CheckIfReloading();
    }
  }
  private void CheckIfReloading()
  {
    foreach(Gun gun in guns)
    {
      if(gun.Reloading == true)
      {
        gun.ReloadGun();
      }
    }
  }
  private void SetAllGunsInactive()
  {
    playerAnimator.SetBool("M1911", false);
    playerAnimator.SetBool("M1", false);
    playerAnimator.SetBool("DB", false);
    foreach(Gun gun in guns)
    {
      gun.gameObject.SetActive(false);
    }
  }
}
