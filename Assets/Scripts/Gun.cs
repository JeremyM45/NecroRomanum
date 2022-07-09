using System.Runtime.Serialization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
  [SerializeField] private GameObject bulletDecal;
  [SerializeField] private GameObject enemyHitDecal;
  [SerializeField] private GameObject muzzleFlash;
  [SerializeField] private float flashEffectTime = 0.05f;
  // [SerializeField] private Transform firePoint;
  // [SerializeField] private Projectile projectile;
  [SerializeField] private Camera playerCam;
  [Header("Stats")]
  [SerializeField] private float timeBetweenShots;
  [SerializeField] private float range;
  private bool readyToShoot = true;
  void Start()
  {
    muzzleFlash.SetActive(false);
  }
  void Update()
  {
    if(Input.GetKeyDown(KeyCode.Mouse0) && readyToShoot)
    {
      Shoot();
    }
  }
  private void Shoot()
  {
    readyToShoot = false;
    Ray ray = playerCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
    RaycastHit hit;
    if(Physics.Raycast(ray, out hit, range))
    {
      Debug.Log(hit.transform.name);
      if(hit.transform.gameObject.layer == 9)
      {
        GameObject obj = Instantiate(enemyHitDecal, hit.point, Quaternion.LookRotation(hit.normal));
      }
      else
      {
        GameObject obj = Instantiate(bulletDecal, hit.point, Quaternion.LookRotation(hit.normal));
      }
      // obj.transform.position += obj.transform.forward / 1000;
    }
    muzzleFlash.SetActive(true);
    StartCoroutine(MuzzleFlash());
    StartCoroutine(ResetShot());
  }
  IEnumerator MuzzleFlash()
  {
    yield return new WaitForSeconds(flashEffectTime);
    muzzleFlash.SetActive(false);
  }
  IEnumerator ResetShot()
  {
    yield return new WaitForSeconds(timeBetweenShots);
    readyToShoot = true;
  }
}
