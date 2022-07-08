using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
  private void OnCollisionEnter()
  {
    Invoke("DestroyObject", 0.1f);
  }
  private void DestroyObject()
  {
    Destroy(gameObject);
  }
}
