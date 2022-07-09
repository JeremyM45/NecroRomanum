using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
  [SerializeField] Transform orientation;
  
  [SerializeField] Inputs input;
  [SerializeField] float airMultiplier;
  [SerializeField] MovementState state;
  [SerializeField] enum MovementState
  {
    walking,
    sprinting,
    crouching,
    air
  }
  float horizontalInput;
  float verticalInput;
  Vector3 moveDirection;
  Rigidbody rb;
  
  [Header("Movement")]
    [SerializeField] float walkSpeed;
    [SerializeField] float sprintSpeed;
    [SerializeField] float groundDrag;
    float movmentSpeed;
  [Header("Ground Check")]
    [SerializeField] float playerHeight;
    [SerializeField] LayerMask groundLayer;
    bool grounded;
  [Header("Jumping")]
    [SerializeField] float jumpForce;
    [SerializeField] float timeBetweenJumps;
    bool readyToJump;
  [Header("Crouching")]
    [SerializeField] float crouchSpeed;
    [SerializeField] float crouchYScale;
    float startYScale;
  [Header("Slope Handling")]
    [SerializeField] float maxSlopeAngle;
    [SerializeField] float slopeSlideSpeed;
    [SerializeField] bool onSteepSlope;
    RaycastHit slopeHit;
    bool exitingSlope;
  // Start is called before the first frame update
  void Start()
  {
    rb = GetComponent<Rigidbody>();
    rb.freezeRotation = true;
    startYScale = transform.localScale.y;
    readyToJump = true;
  }

  // Update is called once per frame
  void Update()
  {
    grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, groundLayer);
    PlayerInput();
    StateHandler();
    SpeedControl();
    if(grounded)
    {
      rb.drag = groundDrag;
    }
    else
    {
      rb.drag = 0;
    }
    rb.useGravity = !OnSlope();
    if(OnSteepSlope())
    {
      SteepSlopeMovement();
    }
  }
  private void FixedUpdate()
  {
    MovePlayer();
  }
  private void PlayerInput()
  {
    horizontalInput = Input.GetAxisRaw("Horizontal");
    verticalInput = Input.GetAxisRaw("Vertical");
    if(Input.GetKey(input.jumpKey) && grounded && readyToJump)
    {
      readyToJump = false;
      Jump();
      Invoke("ResetJump", timeBetweenJumps);
    }
    if(Input.GetKeyDown(input.crouchKey))
    {
      ProjectileGun gun = GetComponentInChildren<ProjectileGun>();
      Debug.Log(gun);
      float gunDefaultY = gun.GetTransform().localScale.y;
      transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
      gun.SetTransfrom(gun.transform.localScale.x, (1 / crouchYScale), gun.transform.localScale.z);
    }
    if(Input.GetKeyUp(input.crouchKey))
      
      {
        transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
        ProjectileGun gun = GetComponentInChildren<ProjectileGun>();
        gun.SetTransfrom(gun.transform.localScale.x, 1, gun.transform.localScale.z);
      }
  }
  private void StateHandler()
  {
    if(grounded && Input.GetKey(input.crouchKey))
    {
      state = MovementState.crouching;
      movmentSpeed = crouchSpeed;
    }
    else if(grounded && Input.GetKey(input.sprintKey))
    {
      state = MovementState.sprinting;
      movmentSpeed = sprintSpeed;
    }
    else if(grounded)
    {
      state = MovementState.walking;
      movmentSpeed = walkSpeed;
    }
    else
    {
      state = MovementState.air;
    } 
  }
  private void MovePlayer()
  {
    moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
    if(OnSlope() && !exitingSlope)
    {
      rb.AddForce(GetSlopeMoveDirection() * movmentSpeed * 20f, ForceMode.Force);
      if(rb.velocity.y > 0)
      {
        rb.AddForce(Vector3.down * 80f, ForceMode.Force);
      }
    }
    else if(grounded)
    {
      rb.AddForce(moveDirection.normalized * movmentSpeed * 10f, ForceMode.Force);
    }
    else if(!grounded)
    {
      rb.AddForce(moveDirection.normalized * movmentSpeed * 10f * airMultiplier, ForceMode.Force);
    }
  }
  private void SpeedControl()
  {
    if(OnSlope() && !exitingSlope)
    {
      if(rb.velocity.magnitude > movmentSpeed)
      {
        rb.velocity = rb.velocity.normalized * movmentSpeed;
      }
    }
    else
    {
      Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
      if(flatVel.magnitude > movmentSpeed)
      {
        Vector3 limitedVel = flatVel.normalized * movmentSpeed;
        rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
      }
    }
    
  }
  private void Jump()
  {
    exitingSlope = true;
    rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
    rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
  }
  private void ResetJump()
  {
    readyToJump = true;
    exitingSlope = false;
  }
  private bool OnSlope()
  {
    if(Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f))
    {
      float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
      return angle < maxSlopeAngle && angle != 0;
    }
    return false;
  }
  private bool OnSteepSlope()
  {
    if(Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.5f))
    {
      float angle = Vector3.Angle(slopeHit.normal, Vector3.up);
      if(angle > maxSlopeAngle)
      {
        onSteepSlope = true;
        return true;
      }
    }
    onSteepSlope = false;
    return false;
  }
  private void SteepSlopeMovement()
  {
    Vector3 slopeDirection = Vector3.up - slopeHit.normal * Vector3.Dot(Vector3.up, slopeHit.normal);
    float slideSpeed = movmentSpeed + slopeSlideSpeed + Time.deltaTime;
    moveDirection = slopeDirection * -slideSpeed;
    moveDirection.y = moveDirection.y - slopeHit.point.y;
    rb.AddForce(moveDirection, ForceMode.Force);
  }
  private Vector3 GetSlopeMoveDirection()
  {
    return Vector3.ProjectOnPlane(moveDirection, slopeHit.normal).normalized;
  }
}
