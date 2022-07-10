using System.Runtime.Serialization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Gun : MonoBehaviour
{
  [SerializeField] private GameObject bulletDecal;
  [SerializeField] private GameObject enemyHitDecal;
  [SerializeField] private GameObject muzzleFlash;
  [SerializeField] private float flashEffectTime = 0.05f;
  [SerializeField] private Camera playerCam;
  [SerializeField] private TextMeshProUGUI ammoCounter;
  [Header("Stats")]
  [SerializeField] private float timeBetweenShots;
  [SerializeField] private int maxRoundsPerMag;
  [SerializeField] private float reloadTime;
  [SerializeField] private int maxTotalAmmo;
  [SerializeField] private int damage;
  [SerializeField] private float range;
  private bool readyToShoot = true;
  private bool shooting = false;
  private bool reloading = false;
  private int appliedDamage;
  private int currentRoundsInMag;
  private int totalAmmo;
  void Start()
  {
    muzzleFlash.SetActive(false);
    currentRoundsInMag = maxRoundsPerMag;
    totalAmmo = maxTotalAmmo;
  }
  void Update()
  {
    ammoCounter.SetText(currentRoundsInMag + " / " + totalAmmo);
    if(Input.GetKeyDown(KeyCode.Mouse0) && readyToShoot && !reloading && currentRoundsInMag > 0)
    {
      Shoot();
    }
    else if(Input.GetKeyDown(KeyCode.R) && !shooting && !reloading && currentRoundsInMag != maxRoundsPerMag)
    {
      StartCoroutine(Reload());
    }
    else if(Input.GetKeyDown(KeyCode.Mouse0) && readyToShoot && !reloading && currentRoundsInMag <= 0)
    {
      StartCoroutine(Reload());
    }
  }
  private void Shoot()
  {
    readyToShoot = false;
    shooting = true;
    currentRoundsInMag--;
    Ray ray = playerCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
    RaycastHit hit;
    if(Physics.Raycast(ray, out hit, range))
    {
      GameObject obj;
      if(hit.transform.gameObject.layer == 9)
      {
        obj = Instantiate(enemyHitDecal, hit.point, Quaternion.LookRotation(hit.normal));
        obj.transform.position += obj.transform.forward / 10;
        LifeAndDeath enemyLad = hit.transform.GetComponentInParent<LifeAndDeath>();
        EnemyAi enemyAi = hit.transform.GetComponentInParent<EnemyAi>();
        if(enemyAi.Alive)
        {
          if(hit.transform.name == "Body")
          {
            enemyAi.Headshot = false;
            appliedDamage = damage;
          }
          else if(hit.transform.name == "Head")
          {
            enemyAi.Headshot = true;
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
      }
      else if(hit.transform.gameObject.layer != 8)
      {
        obj = Instantiate(bulletDecal, hit.point, Quaternion.LookRotation(hit.normal));
      }
    }
    muzzleFlash.SetActive(true);
    StartCoroutine(MuzzleFlash());
    shooting = false;
    StartCoroutine(ResetShot());
  }
  public void FillAmmo()
  {
    currentRoundsInMag = maxRoundsPerMag;
    totalAmmo = maxTotalAmmo;
  }
  IEnumerator Reload()
  {
    readyToShoot = false;
    reloading = true;
    yield return new WaitForSeconds(reloadTime);
    int roundsToReplace = maxRoundsPerMag - currentRoundsInMag;
    if(roundsToReplace > totalAmmo)
    {
      currentRoundsInMag = totalAmmo;
      totalAmmo = 0;
    }
    else
    { 
      currentRoundsInMag = maxRoundsPerMag;
      totalAmmo -= roundsToReplace;
    }
    reloading = false;
    readyToShoot = true;
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
