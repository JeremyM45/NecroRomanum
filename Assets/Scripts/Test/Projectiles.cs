using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectiles : MonoBehaviour
{
  [SerializeField] Rigidbody rb;
  [SerializeField] GameObject explosion;
  [SerializeField] LayerMask whatIsEnemies;
  PhysicMaterial physicMaterial;
  [Header("Stats")]
    [Range(0f,1f)]
      [SerializeField] float bounciness;
    [SerializeField] bool useGravity;
    float _blastRange;
    float _damage;

  [Header("Lifetime")]
  [SerializeField] float maxLifetime;
  [SerializeField] bool explodeOnTouch = true;
  [SerializeField] bool explodeOnImpact = true;

  // Start is called before the first frame update
  void Start()
  {
    Setup();
  }

  // Update is called once per frame
  void Update()
  {
    maxLifetime -= Time.deltaTime;
    if(maxLifetime <= 0 && _blastRange > 1)
    {
      Explode();
    }
    else if(maxLifetime <= 0)
    {
      Invoke("Delay", 0.005f);
    }
    
  }
  private void OnCollisionEnter(Collision collision)
  {
    if(collision.collider.CompareTag("Opfor"))
    {
      collision.collider.GetComponentInParent<EnemyAi>().SetUnderAttack(true);
    }
    if(collision.collider.CompareTag("Opfor") && explodeOnTouch || collision.collider.CompareTag("Bluefor") && explodeOnTouch || collision.collider.CompareTag("Player") && explodeOnTouch)
    {
      if(_blastRange > 1)
      {
        Explode();
      }
      else
      {
        collision.collider.gameObject.GetComponent<LifeAndDeath>().TakeDamage(_damage);
        Invoke("Delay", 0.005f);
      }
    }
    else if(explodeOnImpact)
    {
      if(_blastRange > 1)
      {
        Explode();
      }
      else if(collision.gameObject.CompareTag("Opfor") || collision.gameObject.CompareTag("Bluefor") || collision.gameObject.CompareTag("Player"))
      {
        collision.collider.gameObject.GetComponent<LifeAndDeath>().TakeDamage(_damage);
        Invoke("Delay", 0.005f);
      }
      else
      {
        Invoke("Delay", 0.005f);
      }
    }
  }
  private void Explode()
  {
    if(explosion != null)
    {
      Instantiate(explosion, transform.position, Quaternion.identity);
    }
    Collider[] enemies = Physics.OverlapSphere(transform.position, _blastRange, whatIsEnemies);
    for(int i = 0; i < enemies.Length; i++)
    {
      enemies[i].GetComponent<LifeAndDeath>().TakeDamage(_damage);
    }
    Invoke("Delay", 0.005f);
  }
  private void Delay()
  {
    Destroy(gameObject);
  }
  private void Setup()
  {
    physicMaterial = new PhysicMaterial();
    physicMaterial.bounciness = bounciness;
    physicMaterial.frictionCombine = PhysicMaterialCombine.Minimum;
    physicMaterial.bounceCombine = PhysicMaterialCombine.Maximum;
    GetComponent<SphereCollider>().material = physicMaterial;
    rb.useGravity = useGravity;
  }
  public void SetDamage(float damage)
  {
    _damage = damage;
  }
  public void SetBlastRange(float blastRange)
  {
    _blastRange = blastRange;
  }
}
