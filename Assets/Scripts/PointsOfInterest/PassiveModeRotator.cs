using GLEAMoscopeVR.Settings;
using UnityEngine;
using UnityEngine.Assertions;

namespace GLEAMoscopeVR.POIs
{
    /// <summary>
    /// Handles rotating Points of Interest into the user's original, forward-facing direction when operating in Passive <see cref="ExperienceMode"/>.
    /// </summary>
    public class PassiveModeRotator : MonoBehaviour
    {
        /// <summary>
        /// Transform to return to when Passive <see cref="ExperienceMode"/> is de-activated.
        /// </summary>
        [Tooltip("Transform to return to when Passive Experience Mode is de-activated.")]
        public Transform OriginTransform = null;
        
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
        
        Transform current = null;
        Transform target = null;

        #region References
        ExperienceModeController _modeController;
        #endregion

        public bool CanRotate() => !isRotating;

        void Awake()
        {
            current = transform;
            target = transform;
        }

        void Start()
        {
            SubscribeToExperienceModeEvents();
        }
        
        private void HandleModeChanged()
        {
            if (_modeController.CurrentMode == ExperienceMode.Exploration)
            {
                SetTargetTransformAndRotate(OriginTransform);
            }
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

        private void SubscribeToExperienceModeEvents()
        {
            _modeController = ExperienceModeController.Instance;
            Assert.IsNotNull(_modeController, "$[PassiveModeRotator] Cannot find a reference to ExperienceModeController.");
            _modeController.OnExperienceModeChanged += HandleModeChanged;
        }
    }
}