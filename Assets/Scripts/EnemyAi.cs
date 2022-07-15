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
  [Header("Audio")]
  [SerializeField] private AudioClip[] idleSounds;
  [SerializeField] private AudioClip[] attackSounds;
  [SerializeField] private AudioClip[] deathSounds;
  [SerializeField] private float minTimeToSound;
  [SerializeField] private float maxTimeToSound;
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
  private bool canMakeSound;
  private bool canMove = true;
  private NavMeshAgent agent;
  private Animator animator;
  private LifeAndDeath ladScript;
  private GameObject player;
  private GlobalSpawnLogic globalSpawnLogic;
  private AudioSource audioSource;
  
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
    audioSource = GetComponent<AudioSource>();
    StartCoroutine(IdleSoundCheck());
    SetSpeed(globalSpawnLogic.MinSpeed, globalSpawnLogic.MaxSpeed);
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
    PlayDeathSound();
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
  private void SetSpeed(float min, float max)
  {
    minSpeed = min;
    maxSpeed = max;
    float speed = Random.Range(minSpeed, maxSpeed);
    agent.speed = speed;
  }
  private void PlayAttackSound()
  {
    if(attackSounds.Length > 1)
    { 
      int rng = Random.Range(0, attackSounds.Length);
      audioSource.PlayOneShot(attackSounds[rng]);
    }
    else
    {
      audioSource.PlayOneShot(attackSounds[0]);
    }
  }
  private void PlayDeathSound()
  {
    bool playDeathSound = Random.Range(0, 2) > 0;
    if(playDeathSound)
    {
      if(attackSounds.Length > 1)
      { 
        int rng = Random.Range(0, deathSounds.Length);
        audioSource.PlayOneShot(deathSounds[rng]);
      }
      else
      {
        audioSource.PlayOneShot(deathSounds[0]);
      }
    }
  }
  private IEnumerator Attack()
  {
    attacking = true;
    PlayAttackSound();
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
  private IEnumerator HelmetHit()
  {
    canMove = false;
    agent.SetDestination(transform.position);
    HasHelmet = !HasHelmet;
    animator.Play("HelmetHit");
    yield return new WaitForSeconds(0.2f);
    helmet.SetActive(false);
    yield return new WaitForSeconds(0.2f);
    canMove = true;
  }
  private IEnumerator IdleSoundCheck()
  {
    WaitForSeconds wait = new WaitForSeconds(Random.Range(minTimeToSound, maxTimeToSound));
    while(Alive)
    {
      int rng = Random.Range(0, idleSounds.Length);
      audioSource.PlayOneShot(idleSounds[rng]);
      yield return wait;
    }
  }
  private void DestroyGameObject()
  {
    Destroy(gameObject);
  }
}
