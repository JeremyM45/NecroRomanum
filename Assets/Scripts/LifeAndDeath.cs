using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeAndDeath : MonoBehaviour
{
  [SerializeField] private int maxHealth;
  [SerializeField] private int currentHealth;
  void Awake()
  {
    currentHealth = maxHealth;
  }
  public void TakeDamage(int damage)
  {
    currentHealth -= damage;
    if(currentHealth < 0)
    {
      currentHealth = 0;
    }
    Debug.Log("Health: " + currentHealth + " / " + maxHealth);
  }
  public int GetCurrentHealth()
  {
    return currentHealth;
  }
}
