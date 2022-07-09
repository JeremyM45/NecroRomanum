using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
  [SerializeField] private GameObject bulletDecal;
  private void OnCollisionEnter()
  {
    Instantiate(bulletDecal, transform.position, Quaternion.identity);
    Invoke("DestroyObject", 0.1f);
  }
  private void DestroyObject()
  {
    Destroy(gameObject);
  }
}
