using System.Runtime.Serialization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Gun : MonoBehaviour
{
  public bool Reloading {get; set;} = false;
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
  [SerializeField] private float accuracySpread;
  [SerializeField] private bool isShotgun;
  [SerializeField] private int pelletsInShell;
  private List<Vector3> decalPos = new List<Vector3>();
  private bool readyToShoot = true;
  private bool shooting = false;
  private int pelletsFired;
  private int appliedDamage;
  private int currentRoundsInMag;
  private int totalAmmo;
  private Animator playerAnimator;
  private Animator gunAnimator;
  private string gunName;
  void Start()
  {
    muzzleFlash.SetActive(false);
    currentRoundsInMag = maxRoundsPerMag;
    totalAmmo = maxTotalAmmo;
    playerAnimator = GameObject.Find("Hands").GetComponent<Animator>();
    gunAnimator = transform.gameObject.GetComponent<Animator>();
    gunName = transform.name;
  }
  void Update()
  {
    ammoCounter.SetText(currentRoundsInMag + " / " + totalAmmo);
    if(Input.GetKeyDown(KeyCode.Mouse0) && readyToShoot && !Reloading && currentRoundsInMag > 0)
    {
      Shoot();
    }
    else if(Input.GetKeyDown(KeyCode.R) && !shooting && !Reloading && currentRoundsInMag != maxRoundsPerMag)
    {
      StartCoroutine(Reload());
    }
    else if(Input.GetKeyDown(KeyCode.Mouse0) && readyToShoot && !Reloading && currentRoundsInMag <= 0)
    {
      StartCoroutine(Reload());
    }
  }
  private void Shoot()
  {
    readyToShoot = false;
    shooting = true;
    currentRoundsInMag--;
    float spread = accuracySpread / 1000;
    float spreadX = Random.Range(0.5f - spread, 0.5f + spread); 
    float spreadY = Random.Range(0.5f - spread, 0.5f + spread); 
    Ray shootPoint = playerCam.ViewportPointToRay(new Vector3(spreadX, spreadY, 0.1f));
    RaycastHit hit;
    if(Physics.Raycast(shootPoint, out hit, range))
    {
      Debug.Log(hit.transform.name);
      GameObject obj;
      if(hit.transform.gameObject.layer == 9)
      {
        if(!CheckIfCloseDecal(hit.point))
        {
          obj = Instantiate(enemyHitDecal, hit.point, Quaternion.LookRotation(hit.normal));
          obj.transform.position += obj.transform.forward / 10;
        }
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
      else if(hit.transform.gameObject.layer != 8 && !CheckIfCloseDecal(hit.point))
      {
        obj = Instantiate(bulletDecal, hit.point, Quaternion.LookRotation(hit.normal));
      }
    }
    muzzleFlash.SetActive(true);
    if(isShotgun && pelletsFired < pelletsInShell)
    {
      pelletsFired++;
      currentRoundsInMag++;
      Shoot();
    }
    else if(isShotgun && pelletsFired >= pelletsInShell)
    {
      pelletsFired = 0;
      shooting = false;
      StartCoroutine(MuzzleFlash());
      StartCoroutine(ResetShot());
      decalPos.Clear();
      playerAnimator.Play(gunName + "Fire");
    }
    else if(!isShotgun)
    {
      shooting = false;
      StartCoroutine(MuzzleFlash());
      StartCoroutine(ResetShot());
      decalPos.Clear();
      playerAnimator.Play(gunName + "Fire");
      gunAnimator.Play(gunName + "Fire");
    }
  }
  public void FillAmmo()
  {
    currentRoundsInMag = maxRoundsPerMag;
    totalAmmo = maxTotalAmmo;
  }
  private bool CheckIfCloseDecal(Vector3 currentHitPoint)
  {
    if(decalPos.Count > 1)
    {
      foreach(Vector3 hitPoint in decalPos)
      {
        if(Vector3.Distance(hitPoint, currentHitPoint) < 0.1f)
        {
          return true;
        }
      }
      decalPos.Add(currentHitPoint);
      return false;
    }
    else
    {
      decalPos.Add(currentHitPoint);
      return false;
    }
    
  }
  public void ReloadGun()
  {
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
    readyToShoot = true;
    Reloading = false;
  }
  public bool IsAmmoFull()
  {
    if(currentRoundsInMag == maxRoundsPerMag && totalAmmo == maxTotalAmmo)
    {
      return true;
    }
    else
    {
      return false;
    }
  }
  private IEnumerator Reload()
  {
    playerAnimator.Play(transform.name + "Lower");
    playerAnimator.SetBool("Reloading", true);
    readyToShoot = false;
    Reloading = true;
    yield return new WaitForSeconds(reloadTime);
    ReloadGun();
    Reloading = false;
    playerAnimator.SetBool("Reloading", false);
    readyToShoot = true;
  }
  private IEnumerator MuzzleFlash()
  {
    yield return new WaitForSeconds(flashEffectTime);
    muzzleFlash.SetActive(false);
  }
  private IEnumerator ResetShot()
  {
    yield return new WaitForSeconds(timeBetweenShots);
    
    readyToShoot = true;
  }
}
