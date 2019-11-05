using UnityEngine;
using GLEAMoscopeVR.Interaction;

namespace GLEAMoscopeVR.Cameras
{
    public class RotateToFaceCamera : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("The gameobject to look at. If not assigned, will default to the Main Camera in the scene.")]
        GameObject ToLookAt;

        [SerializeField]
        [Tooltip("Whether the panel should face the camera. Toggle this programmatically with SetLookAtCamera()")]
        bool LookAtCamera = true;

        public bool ShouldRotate = true;
        public bool SlerpRotate = false;
        public float SlerpRotateSpeed = 0.5f;
        public float tetherRange = 0.5f;
        
        /// <summary>
        /// On Awake, check to see if ToLookAt has been assigned in the editor. 
        /// If it hasn't default to the scene's main camera.
        /// </summary>
        private void Awake()
        {
            if (ToLookAt == null)
            {
                ToLookAt = Camera.main.gameObject;
            }
        }

        /// <summary>
        /// Every frame, face the panel toward the ToLookAt gameobject.
        /// This method by default faces an object AWAY from ToLookAt because of oddities with Canvas transforms.
        /// </summary>
        void LateUpdate()
        {
            if (LookAtCamera)
            {
                if (ShouldRotate)
                {
                    if (!CheckIsReticleHitting())
                    {
                        transform.rotation = Quaternion.LookRotation(ToLookAt.transform.forward, ToLookAt.transform.up);
                    }
                }
            }
        }

        /// <summary>
        /// Check if the reticle is currently hitting the panel or any of its child objects.
        /// </summary>
        /// <returns></returns>
        public bool CheckIsReticleHitting()
        {
            if (ToLookAt.GetComponent<CameraRayCaster>() != null)
            {
                if (ToLookAt.GetComponent<CameraRayCaster>().GetCurrentCentreHit() == gameObject)
                {
                    return true;
                }

                foreach (Transform child in transform)
                {
                    if (ToLookAt.GetComponent<CameraRayCaster>().GetCurrentCentreHit() == child.gameObject)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public bool GetLookAtCamera()
        {
            return LookAtCamera;
        }

        public void SetLookAtCamera(bool value)
        {
            LookAtCamera = value;
        }
    }
}