using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ProjectileGun : MonoBehaviour
{
  [SerializeField] Camera playerCamera;
  [SerializeField] bool allowInvoke = true;
  [SerializeField] Transform firePoint;
  [SerializeField] Inputs input;
  [SerializeField] bool _equipped = false;
  [SerializeField] Rigidbody rb;
  [SerializeField] Collider _collider;
  int projectilesLeft;
  int projectilesShot;
  [SerializeField] bool shooting;
  bool readyToShoot;
  bool reloading;
  [Header("Visuals")]
    [SerializeField] GameObject muzzleFlash;
    [SerializeField] AudioSource shotSound;
    [SerializeField] TextMeshProUGUI ammoDisplay;
  [Header("Projectile")]
    [SerializeField] Projectiles projectile;
    [SerializeField] float horizontalForce;
    [SerializeField] float vericalForce;
    [SerializeField] float damage;
    [SerializeField] float blastRange;
  [Header("Stats")]
    [SerializeField] float timeBetweenShots;
    [SerializeField] float accuracySpread;
    [SerializeField] float reloadTime;
    [SerializeField] float timeBetweenShotsInBurst;
    [SerializeField] int projectilesPerTriggerPress;
    [SerializeField] int magSize;
    [SerializeField] bool allowFullAuto;
    [SerializeField] bool isShotGun;
    private bool _inPickUpRange;
    [SerializeField] Transform parentBody;
    

    private void Awake()
    {
      projectilesLeft = magSize;
      readyToShoot = true;
      _inPickUpRange = false;
    }

    // Update is called once per frame
    void Update()
    {
      if(_equipped)
      {
        PlayerInput();
        if(ammoDisplay != null)
        {
          if(isShotGun)
          {
            (ammoDisplay).SetText(projectilesLeft / projectilesPerTriggerPress + " / " + magSize / projectilesPerTriggerPress);
          }
          else
          {
            (ammoDisplay).SetText(projectilesLeft + " / " + magSize);
          }
        }
      }
    }
    private void PlayerInput()
    {
      if(allowFullAuto)
      {
        shooting = Input.GetKey(input.shootKey);
      }
      else
      {
        shooting = Input.GetKeyDown(input.shootKey);
      }
      if(Input.GetKeyDown(input.reloadKey) && !reloading)
      {
        Reload();
      }
      if(readyToShoot && shooting && !reloading && projectilesLeft <= 0)
      {
        Reload();
      }
      if(readyToShoot && shooting && !reloading && projectilesLeft > 0)
      {
        projectilesShot = 0;
        Shoot();
      }
      
    }
    private void Reload()
    {
      reloading = true;
      Invoke("ReloadFinished", reloadTime);
    }
    private void ReloadFinished()
    {
      projectilesLeft = magSize;
      reloading = false;
    }
    public void Shoot()
    {
      readyToShoot = false;
      if(playerCamera != null)
      {
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;
        Vector3 targetPoint;
        if(Physics.Raycast(ray, out hit))
        {
          targetPoint = hit.point;
        }
        else
        {
          targetPoint = ray.GetPoint(76);
        }

        Vector3 directionWithoutSpread = targetPoint - firePoint.position;
        float spreadX = Random.Range(-accuracySpread, accuracySpread);
        float spreadY = Random.Range(-accuracySpread, accuracySpread);
        Vector3 directionWithSpread = directionWithoutSpread + new Vector3(spreadX, spreadY, 0);
        Projectiles currentBullet = Instantiate(projectile, firePoint.position, Quaternion.identity) as Projectiles;
        currentBullet.SetDamage(damage);
        currentBullet.SetBlastRange(blastRange);
        currentBullet.transform.forward = directionWithSpread.normalized;
        currentBullet.GetComponent<Rigidbody>().AddForce(directionWithSpread.normalized * horizontalForce, ForceMode.Impulse);
        currentBullet.GetComponent<Rigidbody>().AddForce(playerCamera.transform.up * vericalForce, ForceMode.Impulse);
      }
      else
      {
        Projectiles currentBullet = Instantiate(projectile, new Vector3(parentBody.position.x, parentBody.position.y, parentBody.position.z), transform.rotation) as Projectiles;
        currentBullet.SetDamage(damage);
        currentBullet.SetBlastRange(blastRange);
        currentBullet.GetComponent<Rigidbody>().AddForce(transform.forward * horizontalForce, ForceMode.Impulse);
        currentBullet.GetComponent<Rigidbody>().AddForce(transform.up * vericalForce, ForceMode.Impulse);
      }
      

      if(muzzleFlash !=null)
      {
        Instantiate(muzzleFlash, firePoint.position, Quaternion.identity);
      }
      if(shotSound != null)
      {
        shotSound.Play();
      }
      projectilesLeft--;
      projectilesShot++;
      if(allowInvoke)
      {
        Invoke("ResetShot", timeBetweenShots);
        allowInvoke = false;
      }
      if(projectilesShot < projectilesPerTriggerPress && projectilesLeft > 0)
      {
        Invoke("Shoot", timeBetweenShotsInBurst);
      }
    }
    private void ResetShot()
    {
      readyToShoot = true;
      allowInvoke = true;
    }
    public void SetEquipped(bool equipped)
    {
      _equipped = equipped;
    }
    public bool GetEquipped()
    {
      return _equipped;
    }
    public Rigidbody GetRigidbody()
    {
      return rb;
    }
    public Collider GetCollider()
    {
      return _collider;
    }
    public bool GetInPickUpRange()
    {
      return _inPickUpRange;
    }
    public void SetInPickUpRange(bool inPickUpRange)
    {
      _inPickUpRange = inPickUpRange;
    }
    public float GetTimeBetweenShots()
    {
      return timeBetweenShots;
    }
    public Transform GetTransform()
    {
      return transform;
    }
    public void SetTransfrom(float x, float y, float z)
    {
      transform.localScale = new Vector3(x, y, z);
    }
}
