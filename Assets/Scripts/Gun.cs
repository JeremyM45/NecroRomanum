using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
  [SerializeField] private Transform firePoint;
  [SerializeField] private Projectile projectile;
  [SerializeField] private Camera playerCam;
  [Header("Stats")]
  [SerializeField] private float timeBetweenShots;
  [SerializeField] private float horizontalForce;
  [SerializeField] private float verticalForce;
  
  private bool readyToShoot = true;

  // Update is called once per frame
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
    Vector3 directionWithoutSpread = targetPoint - firePoint.position;
    Projectile currentBullet = Instantiate(projectile, firePoint.position, Quaternion.identity) as Projectile;
    Rigidbody currentBulletRb = currentBullet.GetComponent<Rigidbody>();
    currentBullet.transform.forward = directionWithoutSpread.normalized;
    currentBulletRb.AddForce(directionWithoutSpread.normalized * horizontalForce, ForceMode.Impulse);
    currentBulletRb.AddForce(playerCam.transform.up * verticalForce, ForceMode.Impulse);
    Invoke("ResetShot", timeBetweenShots);
  }
  private void ResetShot()
  {
    readyToShoot = true;
  }
}
