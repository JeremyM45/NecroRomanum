using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zoom : MonoBehaviour
{
  [SerializeField] private float zoomedFov = 45f;
  [SerializeField] private float timeToZoom = 0.3f;
  [SerializeField] private bool toggle;
  private Camera cam;
  private Coroutine zoomRoutine;
  private float initialCamFov;
  
  void Start()
  {
    cam = Camera.main;
    initialCamFov = cam.fieldOfView;
  }
  void Update()
  {
    if(Input.GetKeyDown(KeyCode.Mouse1))
    {
      if(zoomRoutine != null)
      {
        StopCoroutine(zoomRoutine);
        zoomRoutine = null;
      }
      zoomRoutine = StartCoroutine(ZoomCamera(true));
    }
    if(Input.GetKeyUp(KeyCode.Mouse1))
    {
      if(zoomRoutine != null)
      {
        StopCoroutine(zoomRoutine);
        zoomRoutine = null;
      }
      zoomRoutine = StartCoroutine(ZoomCamera(false));
    }
  }
  private IEnumerator ZoomCamera(bool isEnter)
  {
    float targetFov = isEnter ? zoomedFov : initialCamFov;
    float startingFov = cam.fieldOfView;
    float timeElapsed = 0f;
    while(timeToZoom > timeElapsed)
    {
      cam.fieldOfView = Mathf.Lerp(startingFov, targetFov, timeElapsed / timeToZoom);
      timeElapsed += Time.deltaTime;
      yield return null;
    }
    cam.fieldOfView = targetFov;
    zoomRoutine = null;
  }
}
