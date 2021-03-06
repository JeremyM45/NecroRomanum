using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LifeAndDeath : MonoBehaviour
{
  public int MaxHealth;
  public bool CanTakeDamage {get; private set;} = true;
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
      currentHealth = MaxHealth;
    }
  }
  void Update()
  {
    if(isPlayer)
    {
      healthDisplay.SetText(currentHealth + " / " + MaxHealth);
      if(currentHealth <= 0)
      {
        transform.GetComponent<GameOver>().EndGame();
      }
    }
  }
  public void TakeDamage(int damage)
  {
    CanTakeDamage = false;
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
    Invoke("ResetCanTakeDamage", 0.01f);
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
    MaxHealth = hp;
    currentHealth = MaxHealth;
    maxRegenAmount = hp;
  }
  private void ResetCanTakeDamage()
  {
    CanTakeDamage = true;
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
