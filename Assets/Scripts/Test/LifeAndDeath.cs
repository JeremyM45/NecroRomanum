using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LifeAndDeath : MonoBehaviour
{
  private float _currentHealth;
  [SerializeField] float maxHealth;
  [SerializeField] Material deathMaterial;
  [SerializeField] Material damageMaterial;
  private Material defaultMaterial;
  [SerializeField] float destroyTimer;
  [SerializeField] TextMeshProUGUI hpText;
  // Start is called before the first frame update
  void Start()
  {
    defaultMaterial = GetComponent<MeshRenderer>().material;
    _currentHealth = maxHealth;
  }

  // Update is called once per frame
  void Update()
  {
    if(hpText != null)
    {
      if(_currentHealth > 0)
      {
        hpText.SetText("HP: " + _currentHealth + " / " + maxHealth);
      }
      else if(_currentHealth <= 0)
      {
        hpText.SetText("HP: 0" + " / " + maxHealth);
      }
    }
    if(_currentHealth <= 0)
    {
      if(this.GetComponent<EnemyAi>())
      {
        this.GetComponent<EnemyAi>().GetAgent().SetDestination(transform.position);
        this.GetComponent<EnemyAi>().enabled = false;
      }
      if(this.GetComponent<PlayerMovement>())
      {
        this.GetComponent<PlayerMovement>().enabled = false;
      }
      if(this.GetComponentInChildren<PlayerCam>())
      {
        this.GetComponentInChildren<PlayerCam>().enabled = false;
      }
      if(this.GetComponentInChildren<MoveCamera>())
      {
        this.GetComponentInChildren<MoveCamera>().enabled = false;
      }
    }
  }
  public void TakeDamage(float damage)
  {
    _currentHealth -= damage;
    if(_currentHealth > 0)
    {
      GetComponent<MeshRenderer>().material = damageMaterial;
      Invoke("ResetMaterial", 0.3f);
    }
    else if(_currentHealth <= 0)
    {
      GetComponent<MeshRenderer>().material = deathMaterial;
      Invoke("DestroyObject", destroyTimer);
    }
  }
  private void DestroyObject()
  {
    Destroy(gameObject);
  }
  private void ResetMaterial()
  {
    if(_currentHealth > 0)
    {
      GetComponent<MeshRenderer>().material = defaultMaterial;
    }
  }
  public void RestoreHealth()
  {
    _currentHealth = maxHealth;
  }
}

