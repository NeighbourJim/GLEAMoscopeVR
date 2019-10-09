﻿using UnityEngine;

namespace GLEAMoscopeVR.Cameras
{
    /// <summary>
    /// Source: http://www.andrewnoske.com/wiki/Unity_-_Panorama_Viewer
    /// </summary>
    public class CameraMovement : MonoBehaviour
    {
        public float RotationSpeed = 100.0f;

        void Update()
        {
            if (Input.GetMouseButton(0))
            {
                // If left mouse button down:
                float rotateAboutX = Input.GetAxis("Mouse Y") * Time.deltaTime * RotationSpeed; // Mouse movement up/down.
                float rotateAboutY = -Input.GetAxis("Mouse X") * Time.deltaTime * RotationSpeed; // Mouse movement right/left.
                gameObject.transform.Rotate(rotateAboutX, rotateAboutY, 0.0f); // Rotate the (camera) object around X and Y.

                // Lock z rotation to 0 so camera doesn't 'roll' sideways.
                // We only want the camera's yaw (y) and pitch (x) to change.
                var newRotation = gameObject.transform.rotation.eulerAngles;
                newRotation.z = 0;
                gameObject.transform.rotation = Quaternion.Euler(newRotation);
            }
        }
    }
}