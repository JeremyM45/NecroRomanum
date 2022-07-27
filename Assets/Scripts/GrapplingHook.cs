using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GrapplingHook : MonoBehaviour
{
  public bool shouldKeepGrappling;
  public bool ableToCutRope;
  public Vector3 grappleDirection;
  public float MaxDistance;
  public float GrapplingCooldown;
  [SerializeField] private Transform grappleHook;
  [SerializeField] private LineRenderer lineRenderer;
  [SerializeField] private Transform handPos;
  [SerializeField] private Transform playerBody;
  [SerializeField] private LayerMask grappleLayer;
  [SerializeField] private float moveSpeed;
  [SerializeField] private Vector3 playerPosOffset;
  [SerializeField] private Vector3 cutRopeMoveDist;
  private bool isGrappling = false;
  
  private bool isShooting = false;
  private bool canGrapple = true;
  private bool jumpedUp;
  private Rigidbody playerRb;
  private Vector3 grapplePoint;
  private PlayerController playerController;
  private CharacterController characterController;
  void Start()
  {
    playerController = GetComponent<PlayerController>();
    characterController = GetComponent<CharacterController>();
    playerRb = GetComponent<Rigidbody>();
    lineRenderer.enabled = false;
  }
  void Update()
  {
    if(isGrappling)
    {
      if(Input.GetKeyDown(KeyCode.Q) && ableToCutRope)
      {
        shouldKeepGrappling = false;
      }
      grappleHook.position = Vector3.Lerp(grappleHook.position, grapplePoint, moveSpeed * Time.deltaTime);
      if(Vector3.Distance(grappleHook.position, grapplePoint) < 0.5f)
      {
        playerController.isGrappling = true;
        
        grappleDirection = grappleHook.TransformDirection(Vector3.forward);
        if(!jumpedUp)
        {
          Vector3 dir = transform.TransformDirection(transform.up * 2);
          characterController.Move(dir);
          jumpedUp = true;
        }
        playerBody.position = Vector3.Lerp(playerBody.position, grapplePoint - playerPosOffset, (moveSpeed / 4) * Time.deltaTime);
        Invoke("CanCutRope", 0.2f);
        if(!shouldKeepGrappling)
        {
          grappleHook.SetParent(handPos);
          grappleHook.localPosition = Vector3.zero;
          lineRenderer.enabled = false;
          characterController.enabled = true;
          playerController.isGrappling = false;
          playerController.isFallingFromGrapple = true;
          ableToCutRope = false;
          isGrappling = false;
          jumpedUp = false;
        }
      }
    }
    if(Input.GetKeyDown(KeyCode.Q) && canGrapple && !isShooting && !isGrappling)
    {
      ShootGrapplingHook();
      Invoke("ResetCanGrapple", GrapplingCooldown);
    }
  }
  private void FixedUpdate()
  {
    if(Vector3.Distance(playerBody.position, grapplePoint - playerPosOffset) < 1f)
    {
      shouldKeepGrappling = false;
    }
  }
  private void LateUpdate()
  {
    if(lineRenderer.enabled)
    {
      lineRenderer.SetPosition(0, grappleHook.position);
      lineRenderer.SetPosition(1, handPos.position);
    }
  }
  private void OnCollisionEnter(Collision obj)
  {
    if(isGrappling)
    {
      if(obj.gameObject.layer != 8 && ableToCutRope)
      {
        shouldKeepGrappling = false;
      }
    }
  }
  private void ShootGrapplingHook()
  {
    if(!isGrappling && !isShooting)
    {
      isShooting = true;
      canGrapple = false;
      Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
      RaycastHit[] hits = Physics.RaycastAll(ray, MaxDistance).OrderBy(h => h.distance).ToArray();
      if(hits.Length > 0)
      {
        RaycastHit hit = hits[0];
        if(hit.collider.gameObject.layer != 8)
        {
          grapplePoint = hit.point;
          isGrappling = true;
          grappleHook.parent = null;
          grappleHook.LookAt(grapplePoint);
          lineRenderer.enabled = true;
          playerController.isGrappling = true;
          shouldKeepGrappling = true;
        }
      }
      isShooting = false;
    }
  }
  private void ResetCanGrapple()
  {
    canGrapple = true;
  }
  private void CanCutRope()
  {
    ableToCutRope = true;
  }
}
