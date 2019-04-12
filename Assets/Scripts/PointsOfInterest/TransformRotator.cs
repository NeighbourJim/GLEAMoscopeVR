using UnityEngine;

namespace MM.GLEAMoscopeVR.POIs
{
    public class TransformRotator : MonoBehaviour
    {
        public Transform OriginTransform;
        
        [Header("Options")]
        [Tooltip("If true, spherical interpolation is used.\nIf false, linear interpolation is used.")]
        public bool ShouldSlerp = true;
        [Tooltip("If true, speed is clamped.\nIf false, speed is unclamped.")]
        public bool IsClamped = true;

        [Header("Rotation State")]
        [SerializeField]
        private float rotationSpeed = 0.25f;

        [Header("State")]
        [SerializeField]
        private bool isRotating = false;
        [SerializeField]
        private bool shouldRotate = false;
        
        Transform current;
        Transform target;
        
        public bool CanRotate() => !isRotating;

        void Awake()
        {
            current = transform;
            target = transform;
        }

        void Update()
        {
            current = transform;
            if (shouldRotate)
            {
                Rotate();
            }
        }

        private void Rotate()
        {
            // Spherical Interpolation
            if (ShouldSlerp)
            {
                Slerp();
            }
            // Linear Interpolation
            else
            {
                Lerp();
            }

            if (transform.rotation == target.rotation)
            {
                ResetState();
            }
        }
        
        private void Lerp()
        {
            if (IsClamped)
            {
                transform.rotation = Quaternion.Lerp(
                    current.rotation,
                    target.rotation,
                    rotationSpeed * Time.deltaTime);
            }
            else
            {
                transform.rotation = Quaternion.LerpUnclamped(
                    current.rotation,
                    target.rotation,
                    rotationSpeed * Time.deltaTime);
            }
        }

        private void Slerp()
        {
            if (IsClamped)
            {
                transform.rotation = Quaternion.Slerp(
                    current.rotation,
                    target.rotation,
                    rotationSpeed * Time.deltaTime);
            }
            else
            {
                transform.rotation = Quaternion.SlerpUnclamped(
                    current.rotation,
                    target.rotation,
                    rotationSpeed * Time.deltaTime);
            }
        }

        public void SetTargetTransformAndRotate(Transform targetTransform)
        {
            if (isRotating)
            {
                Debug.Log($"Can't set target, currently rotating.");
                return;
            }

            target = targetTransform;
            isRotating = true;
            shouldRotate = true;
        }

        private void ResetState()
        {
            target = transform;
            shouldRotate = false;
            isRotating = false;
        }
    }
}