using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAi : MonoBehaviour
{
  public bool Alive {get; private set;}  = true;
  public bool HasHelmet {get; private set;} = true;
  public bool Headshot {get; set;} = false;
  [SerializeField] private GameObject helmet; 
  [SerializeField] private LayerMask playerLayerMask;
  [SerializeField] private BoxCollider[] colliders;
  [Header("Stats")]
  [SerializeField] private int helmetHP;
  [SerializeField] private float attackRange;
  [SerializeField] private int attackDamage;
  [SerializeField] private float minSpeed;
  [SerializeField] private float maxSpeed;
  [Header("On Kill")]
  [SerializeField] private int baseKillScore;
  [SerializeField] private int headshotKillScore;
  [SerializeField] private int baseKillHPRegen;
  [SerializeField] private int headshotKillHPRegen;
  private bool playerInAttackRange = false;
  private bool attacking = false;
  
  private bool canMove = true;
  private NavMeshAgent agent;
  private Animator animator;
  private LifeAndDeath ladScript;
  private GameObject player;
  private GlobalSpawnLogic globalSpawnLogic;
  
  private string[] deathAnimations = new string[] {"Death1", "Death2", "Death3"};
  private string deathAnim;
  void Awake()
  {
    globalSpawnLogic = GameObject.Find("GlobalSpawnLogic").GetComponent<GlobalSpawnLogic>();
    animator = GetComponent<Animator>();
    agent = GetComponent<NavMeshAgent>();
    ladScript = GetComponent<LifeAndDeath>();
    player = GameObject.Find("Player");
    int rng = (int)Random.Range(0f, deathAnimations.Length);
    deathAnim = deathAnimations[rng];
    SetSpeed();
  }

  // Update is called once per frame
  void Update()
  {
    if(ladScript.GetCurrentHealth() <= 0 && Alive == true)
    {
      Death();
    }
    if(Alive)
    {
      playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, playerLayerMask); 
      transform.eulerAngles = new Vector3 (0, transform.eulerAngles.y, 0);
      if(!playerInAttackRange && canMove)
      {
        agent.SetDestination(player.transform.position);
      }
      if(playerInAttackRange)
      {
        transform.LookAt(player.transform);
        if(!attacking && canMove)
        {
          StartCoroutine(Attack());
        }
      }
    }
    animator.SetFloat("Vel", agent.velocity.magnitude);
  }
  public void HelmetTakeDamage(int damage)
  {
    helmetHP -= damage;
    if(helmetHP <= 0)
    {
      StartCoroutine(HelmetHit());
    }
  }
  private void Death()
  {
    agent.SetDestination(transform.position);
    Alive = false;
    agent.enabled = false;
    if(Headshot)
    {
      player.GetComponent<Score>().AddPoints(headshotKillScore);
      int killRegenIncrement = headshotKillHPRegen / 5;
      player.GetComponent<LifeAndDeath>().RegenerateHealth(headshotKillHPRegen, killRegenIncrement);
    }
    else
    {
      player.GetComponent<Score>().AddPoints(baseKillScore);
      int killRegenIncrement = baseKillHPRegen / 5;
      player.GetComponent<LifeAndDeath>().RegenerateHealth(baseKillHPRegen, killRegenIncrement);
    }
    
    globalSpawnLogic.EnemiesLeftCalc(1);
    animator.Play(deathAnim);
    foreach(BoxCollider boxCollider in colliders)
    {
      boxCollider.isTrigger = true;
    }
    Invoke("DestroyGameObject", 10f);
  }
  private void SetSpeed()
  {
    float speed = Random.Range(minSpeed, maxSpeed);
    agent.speed = speed;
  }
  IEnumerator Attack()
  {
    attacking = true;
    animator.SetBool("Attacking", true);
    yield return new WaitForSeconds(0.2f);
    if(Physics.CheckSphere(transform.position, attackRange, playerLayerMask))
    {
      player.GetComponent<LifeAndDeath>().TakeDamage(attackDamage);
      animator.SetBool("Attacking", false);
      yield return new WaitForSeconds(2f);
      attacking = false;
      yield break;
    }
    yield return new WaitForSeconds(0.5f);
    if(Physics.CheckSphere(transform.position, attackRange, playerLayerMask))
    {
      player.GetComponent<LifeAndDeath>().TakeDamage(attackDamage);
      animator.SetBool("Attacking", false);
    }
    animator.SetBool("Attacking", false);
    yield return new WaitForSeconds(1.5f);
    attacking = false;
  }
  IEnumerator HelmetHit()
  {
    canMove = false;
    HasHelmet = !HasHelmet;
    animator.Play("HelmetHit");
    yield return new WaitForSeconds(0.2f);
    helmet.SetActive(false);
    yield return new WaitForSeconds(0.2f);
    canMove = true;
  }
  private void DestroyGameObject()
  {
    Destroy(gameObject);
  }
}
