using UnityEngine;

namespace GLEAMoscopeVR.Cameras
{
    public class ScaleToCamera : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Adjust this value to scale the base size of the object when scaled based on camera distance.")]
        float BaseSize = 2.4f;

        [SerializeField]
        [Tooltip("The camera used as the focus for scaling. If not set, will default to the scene's Main Camera.")]
        Camera ScaleCamera;

        float distance;
        float size;

        /// <summary>
        /// Sets the focal camera to the scene's main camera if not otherwise specified in the Editor.
        /// </summary>
        private void Awake()
        {
            if (ScaleCamera == null)
            {
                ScaleCamera = Camera.main;
            }
        }

        /// <summary>
        /// Scales the object so that it remains a static size regardless of distance from the camera and camera FOV.
        /// BaseSize is divided by 100000 to account for oddities with Canvas scaling, in order to make editing the Base Size value
        /// in the editor less painful.
        /// </summary>
        void Update()
        {
            distance = (ScaleCamera.transform.position - transform.position).magnitude;
            size = distance * (BaseSize / 100000) * ScaleCamera.fieldOfView;
            transform.localScale = Vector3.one * size;
        }
    }
}