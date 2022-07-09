using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
  public bool CanMove {get; private set;} = true;
  private bool ShouldJump => Input.GetKey(jumpKey) && playerController.isGrounded;

  [Header("Controls")]
  [SerializeField] private KeyCode jumpKey = KeyCode.Space;

  [Header("Look Parameters")]
  [SerializeField, Range(1, 1000)] private float lookSpeed = 2f;
  [SerializeField, Range(1, 90)] private float verticalLookLimit = 90f;

  [Header("Movement Parameters")]
  [SerializeField] private float walkSpeed = 10f;
  [SerializeField] private float jumpForce = 10f;
  [SerializeField] private float gravity = 30f;

  [SerializeField] private Transform arms;
  private Animator playerAnimator;
  private CharacterController playerController;
  private Vector3 moveDirection;
  private Vector2 currentInput;
  private float rotaionX = 0f;
  
  // Start is called before the first frame update
  void Awake()
  {
    playerController = GetComponent<CharacterController>();
    Cursor.lockState = CursorLockMode.Locked;
    Cursor.visible = false;

  }

  // Update is called once per frame
  void Update()
  {
    if(CanMove)
    {
      HandleMovement();
      HandleLook();
      
      if(ShouldJump)
      {
        HandleJump();
      }

      ApplyFinalMovements();
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
    rotaionX -= Input.GetAxis("Mouse Y") * lookSpeed * Time.deltaTime;
    rotaionX = Mathf.Clamp(rotaionX, -verticalLookLimit, verticalLookLimit);
    arms.transform.localRotation = Quaternion.Euler(rotaionX, 0, 0);
    transform.Rotate(new Vector3(0, Input.GetAxis("Mouse X") * lookSpeed * Time.deltaTime, 0));
  }
  private void ApplyFinalMovements()
  {
    if(!playerController.isGrounded)
    {
      moveDirection.y -= gravity * Time.deltaTime;
    }
    else if(playerController.isGrounded && moveDirection.y < -1f)
    {
      moveDirection.y = 0f;
    }
    
    playerController.Move(moveDirection * Time.deltaTime);
  }
  private void HandleJump()
  {
    moveDirection.y = jumpForce;
  }
}
