using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
  [SerializeField] private GameObject muzzleFlash;
  [SerializeField] private float flashEffectTime = 0.05f;
  [SerializeField] private Transform firePoint;
  [SerializeField] private Projectile projectile;
  [SerializeField] private Camera playerCam;
  [Header("Stats")]
  [SerializeField] private float timeBetweenShots;
  [SerializeField] private float horizontalForce;
  [SerializeField] private float verticalForce;
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
    Vector3 targetPoint;
    if(Physics.Raycast(ray, out hit))
    {
      targetPoint = hit.point;
    }
    else
    {
      targetPoint = ray.GetPoint(76);
    }
    muzzleFlash.SetActive(true);
    StartCoroutine(MuzzleFlash());
    Vector3 directionWithoutSpread = targetPoint - firePoint.position;
    Projectile currentBullet = Instantiate(projectile, firePoint.position, Quaternion.identity) as Projectile;
    Rigidbody currentBulletRb = currentBullet.GetComponent<Rigidbody>();
    currentBullet.transform.forward = directionWithoutSpread.normalized;
    currentBulletRb.AddForce(directionWithoutSpread.normalized * horizontalForce, ForceMode.Impulse);
    currentBulletRb.AddForce(playerCam.transform.up * verticalForce, ForceMode.Impulse);
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
