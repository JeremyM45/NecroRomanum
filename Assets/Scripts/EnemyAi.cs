using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAi : MonoBehaviour
{
  [SerializeField] private Transform player;
  [SerializeField] private float attackRange;
  [SerializeField] private LayerMask playerLayerMask;
  private bool playerInAttackRange = false;
  private bool attacking = false;
  private NavMeshAgent agent;
  private Animator animator;
  void Awake()
  {
    animator = GetComponent<Animator>();
    agent = GetComponent<NavMeshAgent>();
  }

  // Update is called once per frame
  void Update()
  {
    playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, playerLayerMask); 
    transform.LookAt(player);
    
    if(!playerInAttackRange)
    {
      agent.SetDestination(player.position);
    }
    else if(!attacking)
    {
      StartCoroutine(Attack());
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
      Debug.Log("Hit");
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
      Debug.Log("Hit 2");
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
}
