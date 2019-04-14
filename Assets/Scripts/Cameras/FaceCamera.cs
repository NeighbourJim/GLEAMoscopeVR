using UnityEngine;

namespace GLEAMoscopeVR.Cameras
{
    /// <summary>
    /// Forces a game object to face the camera's forward vector.
    /// </summary>
    public class FaceCamera : MonoBehaviour
    {
        [SerializeField]
        Camera cam = null;

        void Awake()
        {
            if (cam == null) cam = Camera.main;
        }

        void LateUpdate()
        {
            transform.LookAt(cam.transform.position, cam.transform.forward);
        }
    }
}