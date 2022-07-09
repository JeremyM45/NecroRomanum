using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
  [SerializeField] ProjectileGun[] weapons;
  [SerializeField] ProjectileGun currentWeapon;
  [SerializeField] ProjectileGun blankWeapon;
  [SerializeField] int currentWeaponIndex = 0;
  [SerializeField] int numberOfWeapons;
  [SerializeField] Transform player;
  [SerializeField] Transform gunContainer;
  [SerializeField] Transform playerCamera;
    // Start is called before the first frame update
    void Start()
    {
      numberOfWeapons = weapons.Length;
      SwitchWeapons(currentWeaponIndex);
    }

    // Update is called once per frame
    void Update()
    {
      if(Input.GetKeyDown(GetComponent<Inputs>().switchWeaponsKey))
      {
        currentWeaponIndex++;
        if(currentWeaponIndex >= weapons.Length)
        {
          currentWeaponIndex = 0;
        }
        SwitchWeapons(currentWeaponIndex);
      }
    }
    void SwitchWeapons(int index)
    {
      weapons[index].gameObject.SetActive(true);
      currentWeapon = weapons[index];
      currentWeapon.SetEquipped(true);
      currentWeapon.transform.SetParent(gunContainer);
      currentWeapon.transform.localPosition = Vector3.zero;
      currentWeapon.transform.localRotation = Quaternion.Euler(Vector3.zero);
      currentWeapon.transform.localScale = Vector3.one;
      currentWeapon.GetRigidbody().isKinematic = true;
      currentWeapon.GetCollider().isTrigger = true;
      if(index < numberOfWeapons && index != 0)
      {
        weapons[index-1].gameObject.SetActive(false);
        ProjectileGun lastWeapon = weapons[index-1];
        lastWeapon.SetEquipped(false);
      }
      else if(index == 0)
      {
        weapons[weapons.Length - 1].gameObject.SetActive(false);
        ProjectileGun lastWeapon = weapons[weapons.Length - 1];
        lastWeapon.SetEquipped(false);
      }
    }
    public void AddBlankWeapon()
      {
        weapons[currentWeaponIndex] = null;
        weapons[currentWeaponIndex] = blankWeapon;
      }
    public ProjectileGun GetCurrentWeapon()
    {
      return currentWeapon;
    }
    public Transform GetPlayer()
    {
      return player;
    }
    public Transform GetGunContainer()
    {
      return gunContainer;
    }
    public void SetWeapon(ProjectileGun newWeapon)
    {
      weapons[currentWeaponIndex] = newWeapon;
      currentWeapon = newWeapon;
    }
}
