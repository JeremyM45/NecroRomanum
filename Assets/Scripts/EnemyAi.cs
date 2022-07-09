using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAi : MonoBehaviour
{
  [SerializeField] private float attackRange;
  [SerializeField] private int attackDamage;
  [SerializeField] private LayerMask playerLayerMask;
  [SerializeField] private BoxCollider[] colliders;
  private bool playerInAttackRange = false;
  private bool attacking = false;
  private bool alive = true;
  private NavMeshAgent agent;
  private Animator animator;
  private LifeAndDeath ladScript;
  private GameObject player;
  
  private string[] deathAnimations = new string[] {"Death1", "Death2", "Death3"};
  private string deathAnim;
  void Awake()
  {
    animator = GetComponent<Animator>();
    agent = GetComponent<NavMeshAgent>();
    ladScript = GetComponent<LifeAndDeath>();
    player = GameObject.Find("Player");
    int rng = (int)Random.Range(0f, deathAnimations.Length);
    deathAnim = deathAnimations[rng];
  }

  // Update is called once per frame
  void Update()
  {
    if(ladScript.GetCurrentHealth() <= 0)
    {
      alive = false;
      agent.SetDestination(transform.position);
      animator.Play(deathAnim);
      foreach(BoxCollider boxCollider in colliders)
      {
        boxCollider.isTrigger = true;
      }
      Invoke("DestroyGameObject", 10f);
    }
    if(alive)
    {
      playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, playerLayerMask); 
      transform.LookAt(player.transform);
      if(!playerInAttackRange)
      {
        agent.SetDestination(player.transform.position);
      }
      else if(!attacking)
      {
        StartCoroutine(Attack());
      }
    }
    animator.SetFloat("Vel", agent.velocity.magnitude);
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
    else
    {
      Debug.Log("No Hit");
    }
    yield return new WaitForSeconds(0.5f);
    if(Physics.CheckSphere(transform.position, attackRange, playerLayerMask))
    {
      player.GetComponent<LifeAndDeath>().TakeDamage(attackDamage);
      animator.SetBool("Attacking", false);
    }
    else
    {
      Debug.Log("No Hit 2");
    }
    animator.SetBool("Attacking", false);
    yield return new WaitForSeconds(1.5f);
    attacking = false;
  }
  private void DestroyGameObject()
  {
    Destroy(gameObject);
  }
}
