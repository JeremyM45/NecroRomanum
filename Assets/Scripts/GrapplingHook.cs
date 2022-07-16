using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GrapplingHook : MonoBehaviour
{
  public bool shouldKeepGrappling;
  public bool ableToCutRope;
  public Vector3 grappleDirection;
  [SerializeField] private Transform grappleHook;
  [SerializeField] private LineRenderer lineRenderer;
  [SerializeField] private Transform handPos;
  [SerializeField] private Transform playerBody;
  [SerializeField] private LayerMask grappleLayer;
  [SerializeField] private float maxDistance;
  [SerializeField] private float moveSpeed;
  [SerializeField] private Vector3 playerPosOffset;
  [SerializeField] private Vector3 cutRopeMoveDist;
  [SerializeField] private float grapplingCooldown;
  private bool isGrappling = false;
  
  private bool isShooting = false;
  private bool canGrapple = true;
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
        playerBody.position = Vector3.Lerp(playerBody.position, grapplePoint - playerPosOffset, (moveSpeed / 2) * Time.deltaTime);
        Invoke("CanCutRope", 0.2f);
        if(!shouldKeepGrappling)
        {
          Invoke("ResetCanGrapple", grapplingCooldown);
          grappleHook.SetParent(handPos);
          grappleHook.localPosition = Vector3.zero;
          lineRenderer.enabled = false;
          characterController.enabled = true;
          playerController.isGrappling = false;
          playerController.isFallingFromGrapple = true;
          ableToCutRope = false;
          isGrappling = false;
        }
      }
    }
    if(Input.GetKeyDown(KeyCode.Q) && canGrapple)
    {
      ShootGrapplingHook();
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
    if(obj.gameObject.layer == 9)
    {
      shouldKeepGrappling = false;
    }
  }
  private void ShootGrapplingHook()
  {
    if(!isGrappling && !isShooting)
    {
      isShooting = true;
      canGrapple = false;
      Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
      RaycastHit[] hits = Physics.RaycastAll(ray, maxDistance).OrderBy(h => h.distance).ToArray();
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
