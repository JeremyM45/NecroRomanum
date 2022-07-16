using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplingHook : MonoBehaviour
{
  [SerializeField] private Transform grappleHook;
  [SerializeField] private LineRenderer lineRenderer;
  [SerializeField] private Transform handPos;
  [SerializeField] private Transform playerBody;
  [SerializeField] private LayerMask grappleLayer;
  [SerializeField] private float maxDistance;
  [SerializeField] private float moveSpeed;
  [SerializeField] private Vector3 playerPosOffset;
  private bool isGrappling = false;
  private bool isShooting = false;
  private Vector3 grapplePoint;
  private PlayerController playerController;
  private CharacterController characterController;
  void Start()
  {
    playerController = transform.GetComponent<PlayerController>();
    characterController = transform.GetComponent<CharacterController>();
    lineRenderer.enabled = false;
  }
  void Update()
  {
    if(Input.GetKeyDown(KeyCode.Q))
    {
      ShootGrapplingHook();
    }
    if(isGrappling)
    {
      grappleHook.position = Vector3.Lerp(grappleHook.position, grapplePoint, moveSpeed * Time.deltaTime);
      if(Vector3.Distance(grappleHook.position, grapplePoint) < 0.5f)
      {
        characterController.enabled = false;
        playerController.isGrappling = true;
        playerBody.position = Vector3.Lerp(playerBody.position, grapplePoint - playerPosOffset, (moveSpeed * 0.75f) * Time.deltaTime);
        if(Vector3.Distance(playerBody.position, grapplePoint - playerPosOffset) < 1f)
        {
          characterController.enabled = true;
          playerController.isGrappling = false;
          isGrappling = false;
          grappleHook.SetParent(handPos);
          grappleHook.localPosition = Vector3.zero;
          lineRenderer.enabled = false;
        }
      }
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
  private void ShootGrapplingHook()
  {
    if(!isGrappling && !isShooting)
    {
      isShooting = true;
      RaycastHit hit;
      Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
      if(Physics.Raycast(ray, out hit, maxDistance, grappleLayer))
      {
        grapplePoint = hit.point;
        isGrappling = true;
        grappleHook.parent = null;
        grappleHook.LookAt(grapplePoint);
        lineRenderer.enabled = true;
      }
      isShooting = false;
    }
  }
}
