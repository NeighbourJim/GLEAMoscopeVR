using GLEAMoscopeVR.Settings;
using UnityEngine;
using UnityEngine.Assertions;

namespace GLEAMoscopeVR.POIs
{
    /// <summary>
    /// 20190416 MM - hacky fix to ensure that if mode button is activated while a POI is rotating into view, it still rotates correctly back to origin
    /// Todo: MM Replace once a decision is made regarding interaction "modes"
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
        [SerializeField]
        private float rotationSpeed = 0.25f;
        [SerializeField]
        private float resetTolerance = 6f;
      
        [Header("Debugging")]
        [SerializeField]
        private float remainingAngle = 0;

        bool shouldRotate = false;

        Transform current = null;
        Transform target = null;

        #region References
        ExperienceModeController _modeController;
        #endregion

        public bool CanRotate() => remainingAngle < resetTolerance;

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
                if (remainingAngle > 0)
                {
                    ResetState();
                }
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

            remainingAngle = Quaternion.Angle(transform.rotation, target.rotation);

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
            if (!CanRotate())
            {
                Debug.Log($"Can't set target, currently rotating.");
                return;
            }

            target = targetTransform;
            shouldRotate = true;
        }

        private void ResetState()
        {
            target = transform;
            shouldRotate = false;
            remainingAngle = 0;
        }

        private void SubscribeToExperienceModeEvents()
        {
            _modeController = ExperienceModeController.Instance;
            Assert.IsNotNull(_modeController, "$[PassiveModeRotator] Cannot find a reference to ExperienceModeController.");
            _modeController.OnExperienceModeChanged += HandleModeChanged;
        }
    }
}