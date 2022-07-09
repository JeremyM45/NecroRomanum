using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCam : MonoBehaviour
{
  [SerializeField] float xSensitivity;
  [SerializeField] float ySensitivity;
  [SerializeField] float fieldOfView;
  [SerializeField] int zoomAmount;
  [SerializeField] bool secondZoom;
  [SerializeField] Transform orientation;
  [SerializeField] Inputs input;
  float xRotation;
  float yRotation;
  int zoomKeyCount;
  Camera cam;
    // Start is called before the first frame update
    void Start()
    {
      cam = GetComponent<Camera>();
      cam.fieldOfView = fieldOfView;
      Cursor.lockState = CursorLockMode.Locked;
      Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
      float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * xSensitivity;
      float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * xSensitivity;
      xRotation -= mouseY;
      yRotation += mouseX;
      xRotation = Mathf.Clamp(xRotation, -90f, 90f);
      transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
      orientation.rotation = Quaternion.Euler(0, yRotation, 0);

      if(Input.GetKeyDown(input.zoomKey) && zoomKeyCount == 0)
      {
        cam.fieldOfView = fieldOfView / zoomAmount;
        zoomKeyCount++;
      }
      else if(Input.GetKeyDown(input.zoomKey) && zoomKeyCount == 1)
      {
        if(secondZoom)
        {
          cam.fieldOfView = fieldOfView / (zoomAmount * 2);
          zoomKeyCount++;
        }
        else
        {
          cam.fieldOfView = fieldOfView;
          zoomKeyCount = 0;
        }
      }
      else if(Input.GetKeyDown(input.zoomKey) && zoomKeyCount == 2)
      {
        cam.fieldOfView = fieldOfView;
        zoomKeyCount = 0;
      }
    }
}
