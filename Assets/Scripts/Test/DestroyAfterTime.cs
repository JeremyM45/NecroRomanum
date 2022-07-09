using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterTime : MonoBehaviour
{
  [SerializeField] float destroyTimer;

  // Update is called once per frame
  void Update()
  {
    Invoke("DestroyObject", destroyTimer);
  }
  private void DestroyObject()
  {
    Destroy(gameObject);
  }
}
