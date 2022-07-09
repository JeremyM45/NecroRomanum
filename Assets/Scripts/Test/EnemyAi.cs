using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAi : MonoBehaviour
{
  [SerializeField] NavMeshAgent agent;
  [SerializeField] Transform player;
  [SerializeField] LayerMask whatIsGround;
  [SerializeField] LayerMask whatIsPlayer;
  [SerializeField] LayerMask whatIsEnemies;
  [SerializeField] ProjectileGun weapon;
  [SerializeField] float distanceToPlayer;
  private bool alreadyAttacked;
  [Header("Patroling")]
    [SerializeField] Vector3 walkPoint;
    [SerializeField] float walkPointRange;
    private bool walkPointSet;
  [Header("States")]
    [SerializeField] float attackRange;
    [SerializeField] float communicationRange;
    [SerializeField] bool playerInAttackRange;
    [SerializeField] bool _underAttack;
  [Header("Field of View")]
    [SerializeField] float fovRadius;
    [Range(0,360)]
    [SerializeField] float fovAngle;
    [SerializeField] LayerMask targetMask;
    [SerializeField] LayerMask obstructionMask;
    [SerializeField] Transform orientation;
    private bool playerInSight;
  private void Awake()
  {
    player = GameObject.Find("Player").transform;
    agent = GetComponent<NavMeshAgent>();
  }
  void Start()
  {
    StartCoroutine(FOVRoutine());
  }
  // Update is called once per frame
  void Update()
  {
    if(player != null)
    {
      distanceToPlayer = (transform.position - player.position).magnitude;
      playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);
      Collider[] enemiesInCommuncationRange = Physics.OverlapSphere(transform.position, communicationRange, whatIsEnemies);
      if(enemiesInCommuncationRange.Length > 1)
      {
        if(_underAttack)
        {
          foreach (Collider enemy in enemiesInCommuncationRange)
          {
            enemy.GetComponent<EnemyAi>().SetUnderAttack(true);
          }
        }
      }
      if(!playerInSight && !playerInAttackRange && !_underAttack)
      {
        Patrol();
      }
      else if(playerInSight && !playerInAttackRange)
      {
        ChasePlayer();
      }
      else if((playerInAttackRange && playerInSight) || _underAttack)
      {
        AttackPlayer();
      }
    }
  }
  private IEnumerator FOVRoutine()
  {
    WaitForSeconds wait = new WaitForSeconds(0.2f);
    while(true)
    {
      yield return wait;
      FOVCheck();
    }
  }
  private void FOVCheck()
  {
    Collider[] playerChecks = Physics.OverlapSphere(transform.position, fovRadius, targetMask);
    if(playerChecks.Length != 0)
    {
      Transform target = playerChecks[0].transform;
      Vector3 directionToTarget = (target.position - transform.position).normalized;
      if(Vector3.Angle(transform.forward, directionToTarget) < fovAngle / 2)
      {
        float distanceToTarget = Vector3.Distance(transform.position, target.position);
        if(!Physics.Raycast(orientation.position, directionToTarget, distanceToTarget, obstructionMask))
        {
          playerInSight = true;
        }
        else
        {
          playerInSight = false;
        }
      }
      else
      {
        playerInSight = false;
      }
    }
    else if(playerInSight)
    {
      playerInSight = false;
    }
  }
  private void Patrol()
  {
    if(!walkPointSet)
    {
      SearchWalkPoint();
    }
    else if(walkPointSet)
    {
      agent.SetDestination(walkPoint);
    }
    Vector3 distanceToWalkPoint = transform.position - walkPoint;
    if(distanceToWalkPoint.magnitude < 1f)
    {
      walkPointSet = false;
    }
  }
  private void SearchWalkPoint()
  {
    float randomZ = Random.Range(-walkPointRange, walkPointRange);
    float randomX = Random.Range(-walkPointRange, walkPointRange);
    walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);
    if(Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
    {
      walkPointSet = true;
    }
  }
  private void ChasePlayer()
  {
    transform.LookAt(player);
    agent.SetDestination(player.position);
  }
  private void AttackPlayer()
  {
    if(distanceToPlayer > (attackRange / 2))
    {
      agent.SetDestination(player.position);
    }
    else
    {
      agent.SetDestination(transform.position);
    }
    transform.LookAt(player);
    if(!alreadyAttacked && playerInSight)
    {
      weapon.GetComponent<ProjectileGun>().Shoot();
      alreadyAttacked = true;
      Invoke("ResetAttack", weapon.GetTimeBetweenShots());
    }
    
  }
  private void ResetAttack()
  {
    alreadyAttacked = false;
  }
  public void SetUnderAttack(bool underAttack)
  {
    _underAttack = underAttack;
  }
  private void OnDrawGizmosSelected()
  {
    Gizmos.color = Color.red;
    Gizmos.DrawWireSphere(transform.position, attackRange);
    Gizmos.color = Color.yellow;
    Gizmos.DrawWireSphere(transform.position, fovRadius);
    Gizmos.color = Color.blue;
    Gizmos.DrawWireSphere(transform.position, communicationRange);

    Vector3 viewAngleLeft = DirectionFromAngle(transform.eulerAngles.y, -fovAngle / 2);
    Vector3 viewAngleRight = DirectionFromAngle(transform.eulerAngles.y, fovAngle / 2);
    Gizmos.color = Color.green;
    Gizmos.DrawLine(transform.position, transform.position + viewAngleLeft * fovRadius);
    Gizmos.DrawLine(transform.position, transform.position + viewAngleRight * fovRadius);
    if(playerInSight)
    {
      Gizmos.color = Color.red;
      Gizmos.DrawLine(transform.position, player.position);
    }
  }
  private Vector3 DirectionFromAngle(float eulerY, float angleInDegrees)
  {
    angleInDegrees += eulerY;
    return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
  }
  public NavMeshAgent GetAgent()
  {
    return agent;
  }
}
