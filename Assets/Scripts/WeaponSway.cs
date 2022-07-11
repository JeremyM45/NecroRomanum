using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// public class WeaponSway : MonoBehaviour
// {
//     [Header("Position")]
//     public float amount = 0.02f,
//         maxAmount = 0.06f,
//         smoothAmount = 6f;

//     [Header("Rotation")]
//     public float rotationAmount = 4f,
//         maxRotationAmount = 5f,
//         smoothRotation = 12f;

//     [Space]
//     public bool rotationX = true, rotationY = true, rotationZ = true;

//     private Vector3 initialPosition;
//     private Quaternion initialRotation;

//     private float InputX, InputY;

//     private void Start()
//     {
//         initialPosition = transform.localPosition;
//         initialRotation = transform.localRotation;
//     }

//     private void Update()
//     {
//         CalculateSway();
//         MoveSway();
//         TiltSway();
//     }

//     private void CalculateSway()
//     {
//         InputX = Input.GetAxis("Mouse X");
//         InputY = Input.GetAxis("Mouse Y");
//     }

//     private void MoveSway()
//     {
//         float moveX = Mathf.Clamp(InputX * amount, -maxAmount, maxAmount);
//         float moveY = Mathf.Clamp(InputY * amount, -maxAmount, maxAmount);

//         Vector3 finalPosition = new Vector3(moveX, moveY, 0);

//         transform.localPosition = Vector3.Lerp(transform.localPosition, finalPosition + initialPosition, Time.deltaTime * smoothAmount);
//     }

//     private void TiltSway()
//     {
//         float tiltY = Mathf.Clamp(InputX * rotationAmount, -maxRotationAmount, maxRotationAmount);
//         float tiltX = Mathf.Clamp(InputY * rotationAmount, -maxRotationAmount, maxRotationAmount);

//         Quaternion finalRotation = Quaternion.Euler(new Vector3(rotationX ? -tiltX : 0f, rotationY ? tiltY : 0f, rotationZ ? tiltY : 0f));
            
//         transform.localRotation = Quaternion.Slerp(transform.localRotation, finalRotation * initialRotation, smoothRotation * Time.deltaTime);
//     }
// }
public class WeaponSway : MonoBehaviour
{
  [Header("Position")]
  [SerializeField] private float swayAmount;
  [SerializeField] private float swaySmoothAmount;
  [SerializeField] private float maxSwayAmount;
  [Header("Rotation")]
  [SerializeField] private float rotationAmount;
  [SerializeField] private float rotationSmoothAmount;
  [SerializeField] private float maxRotationAmount;
  [Header("Bools")]
  [SerializeField] private bool rotationX = true;
  [SerializeField] private bool rotationY = true;
  [SerializeField] private bool rotationZ = true;
  private Vector3 initialPosition;
  private Quaternion initialRotation;
  private Vector3 vel;
  private float movementX;
  private float movementY;
  // Start is called before the first frame update
  void Start()
  {
    initialPosition = transform.localPosition;
    initialRotation = transform.localRotation;
  }

  // Update is called once per frame
  void Update()
  {
    CalcSway();
    MoveSway();
    TitlSway();
  }
  private void CalcSway()
  {
    movementX = -Input.GetAxis("Mouse X");
    movementY = -Input.GetAxis("Mouse Y");
  }
  private void MoveSway()
  {
    float moveX = Mathf.Clamp(movementX * swayAmount, -maxSwayAmount, maxSwayAmount);
    float moveY = Mathf.Clamp(movementY * swayAmount, -maxSwayAmount, maxSwayAmount);
    Vector3 finalPos = new Vector3(moveX, moveY, 0);
    transform.localPosition = Vector3.SmoothDamp(transform.localPosition, finalPos + initialPosition, ref vel, Time.deltaTime * swaySmoothAmount);
  }
  private void TitlSway()
  {
    float tiltY = Mathf.Clamp(movementX * rotationAmount, -maxRotationAmount, maxRotationAmount);
    float tiltX = Mathf.Clamp(movementY * rotationAmount, -maxRotationAmount, maxRotationAmount);
    Quaternion finalRot = Quaternion.Euler(new Vector3(rotationX ? -tiltX : 0f, rotationY ? tiltY : 0f, rotationZ ? tiltY : 0f));
    transform.localRotation = Quaternion.Slerp(transform.localRotation, finalRot * initialRotation, Time.deltaTime * swaySmoothAmount);
  }
}
