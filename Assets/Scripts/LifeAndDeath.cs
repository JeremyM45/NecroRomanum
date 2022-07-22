using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LifeAndDeath : MonoBehaviour
{
  [SerializeField] private int maxHealth;
  [SerializeField] private int currentHealth;
  [SerializeField] private TextMeshProUGUI healthDisplay;
  [SerializeField] private bool isPlayer;
  [Header("RegenHP")]
  [SerializeField] private int maxRegenAmount;
  [SerializeField] private float timeBetweenRegenIncrements;
  [SerializeField] private float timeBeforeRegenStarts;
  [SerializeField] private bool canAutoRegenHp;
  [SerializeField] private int autoRegenAmount;
  [SerializeField] private int autoRegenIncrement;
  private Coroutine regenHealthRoutine;
  void Awake()
  {
    if(!isPlayer)
    {
      SetMaxHealth(GameObject.Find("GlobalSpawnLogic").GetComponent<GlobalSpawnLogic>().Hp);
    }
    else
    {
      currentHealth = maxHealth;
    }
  }
  void Update()
  {
    if(isPlayer)
    {
      healthDisplay.SetText(currentHealth + " / " + maxHealth);
      if(currentHealth <= 0)
      {
        transform.GetComponent<GameOver>().EndGame();
      }
    }
  }
  public void TakeDamage(int damage)
  {
    currentHealth -= damage;
    if(regenHealthRoutine != null)
    {
      StopAllCoroutines();
    }
    if(currentHealth < 0)
    {
      currentHealth = 0;
    }
    else if(canAutoRegenHp)
    {
      RegenerateHealth(autoRegenAmount, autoRegenIncrement);
    }
  }
  public int GetCurrentHealth()
  {
    return currentHealth;
  }
  public void RegenerateHealth(int regenAmount, int regenIncrement)
  {
    regenHealthRoutine = StartCoroutine(RegenHealth(regenAmount, regenIncrement));
  }
  public void SetMaxHealth(int hp)
  {
    maxHealth = hp;
    currentHealth = maxHealth;
  }
  private IEnumerator RegenHealth(int regenAmount, int regenIncrement)
  {
    if(timeBeforeRegenStarts != 0)
    {
      yield return new WaitForSeconds(timeBeforeRegenStarts);
    }
    WaitForSeconds wait = new WaitForSeconds(timeBetweenRegenIncrements);
    int amountToIncrease = currentHealth + regenAmount;
    while(currentHealth < amountToIncrease)
    {
      currentHealth += regenIncrement;
      if(currentHealth > maxRegenAmount)
      {
        currentHealth = maxRegenAmount;
        yield break;
      }
      yield return wait;
    }
    regenHealthRoutine = null;
  }
}
