using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchWeapons : MonoBehaviour
{
  private Gun[] guns;
  private Animator playerAnimator;
  private int currentGunIndex;
  private int newGunIndex;
  private bool switchingWeapons = false;
  // Start is called before the first frame update
  void Start()
  {
    playerAnimator = GameObject.Find("Hands").GetComponent<Animator>();
    guns = transform.GetComponentsInChildren<Gun>();
    SetAllGunsInactive();
    guns[0].gameObject.SetActive(true);
    currentGunIndex = 0;
    playerAnimator.SetBool(guns[0].transform.name, true);
  }

  // Update is called once per frame
  void Update()
  {
    HandleWeaponSwitch();
  }
    private void HandleWeaponSwitch()
  {
    if(Input.GetKeyDown(KeyCode.Alpha1) && guns[0].gameObject.activeSelf == false && !switchingWeapons)
    {
      newGunIndex = 0;
      StartCoroutine(SwitchWeapon());
    }
    if(Input.GetKeyDown(KeyCode.Alpha2) && guns[1].gameObject.activeSelf == false && !switchingWeapons)
    {
      newGunIndex = 1;
      StartCoroutine(SwitchWeapon());
    }
    if(Input.GetKeyDown(KeyCode.Alpha3) && guns[2].gameObject.activeSelf == false && !switchingWeapons)
    {
      newGunIndex = 2;
      StartCoroutine(SwitchWeapon());
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
    foreach(Gun gun in guns)
    {
      gun.gameObject.SetActive(false);
      playerAnimator.SetBool(gun.transform.name, false);
    }
  }
  private IEnumerator SwitchWeapon()
  {
    switchingWeapons = true;
    WaitForSeconds wait = new WaitForSeconds(0.1f);
    playerAnimator.Play(guns[currentGunIndex].transform.name + "Lower");
    playerAnimator.SetBool(guns[currentGunIndex].transform.name, false);
    yield return wait;
    guns[currentGunIndex].gameObject.SetActive(false);
    guns[newGunIndex].gameObject.SetActive(true);
    playerAnimator.Play(guns[newGunIndex].transform.name + "Raise");
    playerAnimator.SetBool(guns[newGunIndex].transform.name, true);
    currentGunIndex = newGunIndex;
    switchingWeapons = false;
    CheckIfReloading();
  }
}
