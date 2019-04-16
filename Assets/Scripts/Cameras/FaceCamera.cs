using UnityEngine;

namespace GLEAMoscopeVR.Cameras
{
    /// <summary>
    /// Forces a game object to face the camera's forward vector.
    /// </summary>
    [ExecuteInEditMode]
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
            //DC 2019/04/14 cam.transform.up FROM cam.transform.forward.
            transform.LookAt(cam.transform.position, cam.transform.up);
        }
    }
}