using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothRotate : MonoBehaviour
{
    public Camera cam = null;

    public float zOffset = 5f;
    public float yOffset = 1.6f;
    public float rotateSpeed = 1f;

    public float tetherRange = 0.5f;

    private void Awake()
    {
        if(cam == null)
        {
            cam = Camera.main;
        }
    }

    private void Update()
    {
        if (Vector3.Distance(transform.position, (cam.transform.position + (cam.transform.forward * zOffset) + (cam.transform.up * yOffset))) > tetherRange)
        {
            transform.position = Vector3.Slerp(transform.position, cam.transform.position + (cam.transform.forward * zOffset) + (cam.transform.up * yOffset), rotateSpeed * Time.deltaTime);
        }
    }
}
