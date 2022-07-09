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
  [SerializeField] private Camera playerCam;
  [Header("Stats")]
  [SerializeField] private float timeBetweenShots;
  [SerializeField] private int damage;
  [SerializeField] private float range;
  private bool readyToShoot = true;
  private int appliedDamage;
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
      GameObject obj;
      if(hit.transform.gameObject.layer == 9)
      {
        obj = Instantiate(enemyHitDecal, hit.point, Quaternion.LookRotation(hit.normal));
        obj.transform.position += obj.transform.forward / 10;
        LifeAndDeath enemyLad = hit.transform.GetComponentInParent<LifeAndDeath>();
        EnemyAi enemyAi = hit.transform.GetComponentInParent<EnemyAi>();
        if(hit.transform.name == "Body")
        {
          appliedDamage = damage;
        }
        else if(hit.transform.name == "Head")
        {
          if(enemyAi.HasHelmet)
          {
            appliedDamage = damage / 4;
            enemyAi.HelmetTakeDamage(damage);
          }
          else
          {
            appliedDamage = damage * 3;
          }
        }
        enemyLad.TakeDamage(appliedDamage);
      }
      else if(hit.transform.gameObject.layer != 8)
      {
        obj = Instantiate(bulletDecal, hit.point, Quaternion.LookRotation(hit.normal));
      }
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
