using System.Security.Cryptography;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
  public bool CanMove {get; set;} = true;
  public bool isFallingFromGrapple;
  public bool isGrappling;
  public bool DashUnlocked;
  private bool ShouldJump => Input.GetKey(jumpKey) && (grounded || isGrappling);
  private bool ShouldDash => Input.GetKeyDown(dashKey) && canDash && DashUnlocked;
  [Header("Controls")]
  [SerializeField] private KeyCode jumpKey = KeyCode.Space;
  [SerializeField] private KeyCode dashKey = KeyCode.LeftShift;

  [Header("Look Parameters")]
  [SerializeField, Range(1, 1000)] private float lookSpeed = 2f;
  [SerializeField, Range(1, 90)] private float verticalLookLimit = 90f;

  [Header("Movement Parameters")]
  [SerializeField] private float walkSpeed = 10f;
  [SerializeField] private float jumpForce = 10f;
  [SerializeField] private float gravity = 30f;
  [Header("Dash Parameters")]
  public int MaxDashCharges;
  public float DashCooldown = 2f;
  public int DashCharges;
  [SerializeField] private float dashAmount = 30f;
  [SerializeField] private float dashIncrement = 2f;

  [Header("Sounds")]
  [SerializeField] AudioClip[] dashSounds;
  [SerializeField] private Transform arms;
  private AudioSource audioSource;
  private CharacterController playerController;
  private Vector3 moveDirection;
  private Vector2 currentInput;
  private float rotaionX = 0f;
  private bool dashing = false;
  private bool canDash = true;
  private bool grounded = true;
  private GrapplingHook grappleHookLogic;
  
  // Start is called before the first frame update
  void Awake()
  {
    playerController = GetComponent<CharacterController>();
    audioSource = GetComponent<AudioSource>();
    Cursor.lockState = CursorLockMode.Locked;
    Cursor.visible = false;
    grappleHookLogic = GetComponent<GrapplingHook>();
    DashCharges = MaxDashCharges;
  }

  // Update is called once per frame
  void Update()
  {
    if(CanMove)
    {
      HandleLook();
      if(ShouldJump)
      {
        HandleJump();
      }
      if(ShouldDash && DashCharges > 0 && canDash)
      {
        StartCoroutine(Dash());
      }
      if(!dashing && !isGrappling)
      {
        HandleMovement();
        ApplyFinalMovements();
      }
    }
  }
  private void HandleMovement()
  {
    currentInput = new Vector2(walkSpeed * Input.GetAxisRaw("Vertical"), walkSpeed * Input.GetAxisRaw("Horizontal"));
    float moveDirectionY = moveDirection.y;
    moveDirection = (transform.TransformDirection(Vector3.forward) * currentInput.x) + (transform.TransformDirection(Vector3.right) * currentInput.y);
    moveDirection.y = moveDirectionY;
  }
  private void HandleLook()
  {
    rotaionX -= Input.GetAxisRaw("Mouse Y") * lookSpeed * Time.deltaTime;
    rotaionX = Mathf.Clamp(rotaionX, -verticalLookLimit, verticalLookLimit);
    arms.transform.localRotation = Quaternion.Euler(rotaionX, 0, 0);
    transform.Rotate(new Vector3(0, Input.GetAxisRaw("Mouse X") * lookSpeed * Time.deltaTime, 0));
  }
  // private void HandleSlopeMovement()
  // {
  //   bool grounded = Physics.Raycast(transform.position, transform.up * -1, 0.25f);

  // }
  private void ApplyFinalMovements()
  {
    grounded = Physics.Raycast(transform.position, transform.up * -1, 0.25f);
    if(isFallingFromGrapple)
    {
      if(!grounded)
      {
        float moveDirectionY = moveDirection.y;
        moveDirection = grappleHookLogic.grappleDirection * 10;
        moveDirection.y = moveDirectionY;
        moveDirection.y -= gravity * Time.deltaTime;;
      }
      else if(grounded)
      {
        isFallingFromGrapple = false;
      }
    }
    else if(!grounded)
    {
      moveDirection.y -= gravity * Time.deltaTime;
    }
    else if(grounded && moveDirection.y < -1f)
    {
      moveDirection.y = 0f;
    }
    moveDirection = AdjustMoveDirectionToSlope(moveDirection);
    playerController.Move(moveDirection * Time.deltaTime);
  }
  private void HandleJump()
  {
    grappleHookLogic.shouldKeepGrappling = false;
    moveDirection.y = jumpForce;
  }
  private void ResetDashCharge()
  {
    if(DashCharges < MaxDashCharges)
    {
      DashCharges++;
    }
  }
  private Vector3 AdjustMoveDirectionToSlope(Vector3 direction)
  {
    Ray ray = new Ray(transform.position, Vector3.down);
    if(Physics.Raycast(ray, out RaycastHit hit, 0.2f))
    {
      Quaternion slopeRotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
      Vector3 adjustedDirection = slopeRotation * direction;
      if(adjustedDirection.y < 0)
      {
        return adjustedDirection;
      }
    }
    return direction;
  }
  private IEnumerator Dash()
  {
    WaitForSeconds wait = new WaitForSeconds(0.001f);
    isFallingFromGrapple = false;
    dashing = true;
    canDash = false;
    Vector3 dashDirection;
    Vector2 input = new Vector2(Input.GetAxisRaw("Vertical"), Input.GetAxisRaw("Horizontal"));
    if(input.y != -1f && input.y != 1f)
    {
      if(input.x == 0)
      {
        input.x = 1;
      }
      dashDirection = transform.TransformDirection(Vector3.forward) * input.x;
    }
    else
    {
      dashDirection = transform.TransformDirection(Vector3.right) * input.y;
    }
    int rng = Random.Range(0, dashSounds.Length);
    audioSource.PlayOneShot(dashSounds[rng]);
    float i = 0;
    while(i < dashAmount)
    {
      playerController.Move(dashDirection * dashIncrement * Time.deltaTime);
      i += dashIncrement;
      yield return wait;
    }
    dashing = false;
    DashCharges--;
    WaitForSeconds cooldown = new WaitForSeconds(0.01f);
    yield return cooldown;
    canDash = true;
    Invoke("ResetDashCharge", DashCooldown);
  }
}
