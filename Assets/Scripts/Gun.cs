using System.Linq;
using System.Collections.Specialized;
using System.Runtime.Serialization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Gun : MonoBehaviour
{
  public bool Reloading = false;
  public bool CanEquip = false;
  public float timeBetweenShots;
  public int maxRoundsPerMag;
  public float reloadTime;
  public int maxTotalAmmo;
  public int damage;
  public float range;
  public int penetrationAmount;
  public float accuracySpread;
  public bool isShotgun;
  public int pelletsInShell;
  public bool isAutomatic;
  public bool isBurst;
  public bool magOnly;
  public int roundsInBurst;
  public float timeBetweenShotsInBurst;
  public float minDecalPosDist {get; set;} = 0.1f;
  [SerializeField] private GameObject bulletDecal;
  [SerializeField] private GameObject enemyHitDecal;
  [SerializeField] private GameObject muzzleFlash;
  [SerializeField] private float flashEffectTime = 0.05f;
  [SerializeField] private Camera playerCam;
  [SerializeField] private TextMeshProUGUI ammoCounter;
  [Header("Sounds")]
  [SerializeField] private AudioClip shot;
  [SerializeField] private AudioClip empty;
  [SerializeField] private AudioClip reload;

  private List<Vector3> decalPos = new List<Vector3>();
  private bool readyToShoot = true;
  private bool shooting = false;
  private int pelletsFired;
  private int roundsInBurstFired;
  private int appliedDamage;
  private int currentRoundsInMag;
  private int totalAmmo;
  private Animator playerAnimator;
  private AudioSource audioSource;
  private string gunName;
  void Start()
  {
    muzzleFlash.SetActive(false);
    currentRoundsInMag = maxRoundsPerMag;
    totalAmmo = maxTotalAmmo;
    playerAnimator = GameObject.Find("Hands").GetComponent<Animator>();
    gunName = transform.name;
    audioSource = GetComponent<AudioSource>();
    if(gunName == "M1911")
    {
      CanEquip = true;
    }
  }
  void Update()
  {
    if(!magOnly)
    {
      ammoCounter.SetText(currentRoundsInMag + " / " + totalAmmo);
    }
    else
    {
      ammoCounter.SetText(currentRoundsInMag.ToString());
    }
    if(Input.GetKey(KeyCode.Mouse0) && isAutomatic && readyToShoot && !Reloading && currentRoundsInMag > 0 || Input.GetKeyDown(KeyCode.Mouse0) && readyToShoot && !Reloading && currentRoundsInMag > 0)
    {
      Shoot();
    }
    else if(Input.GetKeyDown(KeyCode.R) && !shooting && !Reloading && currentRoundsInMag != maxRoundsPerMag && totalAmmo > 0)
    {
      StartRelaodRoutine();
    }
    else if(Input.GetKeyDown(KeyCode.Mouse0) && readyToShoot && !Reloading && currentRoundsInMag == 0 && totalAmmo == 0)
    {
      audioSource.PlayOneShot(empty);
    }
  }
  private void Shoot()
  {
    readyToShoot = false;
    shooting = true;
    float spread = accuracySpread / 1000;
    float spreadX = Random.Range(0.5f - spread, 0.5f + spread); 
    float spreadY = Random.Range(0.5f - spread, 0.5f + spread); 
    Ray shootPoint = playerCam.ViewportPointToRay(new Vector3(spreadX, spreadY, 0.1f));
    RaycastHit[] hits;
    hits = Physics.RaycastAll(shootPoint, range).OrderBy(h => h.distance).ToArray();
    int enemeisHit = 0;
    foreach(RaycastHit hit in hits)
    {
      GameObject obj;
      if(hit.transform.gameObject.layer == 9)
      {
        LifeAndDeath enemyLad = hit.transform.GetComponentInParent<LifeAndDeath>();
        EnemyAi enemyAi = hit.transform.GetComponentInParent<EnemyAi>();
        if(!CheckIfCloseDecal(hit.point))
        {
          if(pelletsFired < 1)
          {
            obj = Instantiate(enemyHitDecal, hit.point, Quaternion.LookRotation(hit.normal));
            obj.transform.position += obj.transform.forward / 10;
          }
          else if(pelletsFired > 1 && enemyAi.Alive)
          {
            obj = Instantiate(enemyHitDecal, hit.point, Quaternion.LookRotation(hit.normal));
            obj.transform.position += obj.transform.forward / 10;
          }
        }
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
              int appliedPen;
              if(penetrationAmount == 0)
              {
                appliedPen = 1;
              }
              else
              {
                appliedPen = penetrationAmount;
              }
              appliedDamage = (damage * appliedPen) / 4;
              enemyAi.HelmetTakeDamage(damage);
            }
            else
            {
              appliedDamage = damage * 3;
            }
          }
          if(enemeisHit > 0)
          {
            appliedDamage /= (enemeisHit * 2);
          }
          enemeisHit++;
          enemyLad.TakeDamage(appliedDamage);
          if(enemeisHit > penetrationAmount)
          {
            break;
          }
        }
      }
      else if(hit.transform.gameObject.layer == 6)
      {
        if(!CheckIfCloseDecal(hit.point))
        {
          obj = Instantiate(bulletDecal, hit.point, Quaternion.LookRotation(hit.normal));
        }
        break;
      }
    }
    muzzleFlash.SetActive(true);
    if(isShotgun)
    {
      pelletsFired++;
      if(pelletsFired < pelletsInShell)
      {
        currentRoundsInMag++;
        Shoot();
      }
      else if(pelletsFired >= pelletsInShell)
      {
        pelletsFired = 0;
        shooting = false;
        StartCoroutine(MuzzleFlash());
        StartCoroutine(ResetShot());
        decalPos.Clear();
        playerAnimator.Play(gunName + "Fire");
        audioSource.PlayOneShot(shot);
      }
    }
    if(isBurst)
    {
      roundsInBurstFired++;
      if(roundsInBurstFired < roundsInBurst)
      {
        Invoke("Shoot", timeBetweenShotsInBurst);
        StartCoroutine(MuzzleFlash());
        playerAnimator.Play(gunName + "Fire");
        audioSource.PlayOneShot(shot);
      }
      else if(roundsInBurstFired >= roundsInBurst)
      {
        roundsInBurstFired = 0;
        shooting = false;
        StartCoroutine(MuzzleFlash());
        StartCoroutine(ResetShot());
        decalPos.Clear();
        playerAnimator.Play(gunName + "Fire");
        audioSource.PlayOneShot(shot);
      }
    }
    if(!isShotgun && !isBurst)
    {
      shooting = false;
      StartCoroutine(MuzzleFlash());
      StartCoroutine(ResetShot());
      audioSource.PlayOneShot(shot);
      decalPos.Clear();
      playerAnimator.Play(gunName + "Fire");
    }
    currentRoundsInMag--;
    if(currentRoundsInMag <= 0 && totalAmmo > 0)
    {
      Invoke("StartRelaodRoutine", 0.2f);
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
        if(Vector3.Distance(hitPoint, currentHitPoint) < 0.5f)
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
  public void CanShoot()
  {
    shooting = false;
    readyToShoot = true;
  }
  private void StartRelaodRoutine()
  {
    StartCoroutine(Reload());
  }
  private IEnumerator Reload()
  {
    playerAnimator.Play(transform.name + "Lower");
    playerAnimator.SetBool("Reloading", true);
    readyToShoot = false;
    Reloading = true;
    WaitForSeconds wait = new WaitForSeconds(0.2f);
    yield return wait;
    audioSource.PlayOneShot(reload);
    yield return new WaitForSeconds(reloadTime - 0.2f);
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
