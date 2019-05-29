using GLEAMoscopeVR.Settings;
using UnityEngine;
using UnityEngine.Assertions;

namespace GLEAMoscopeVR.POIs
{
    /// <summary>
    /// Handles rotating Points of Interest into the user's original, forward-facing direction when operating in Passive <see cref="ExperienceMode"/>.
    /// Todo: handle passive to intro behaviour once implemented.
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
        public bool ShouldSlerp = false;
        [Tooltip("If true, speed is clamped.\nIf false, speed is unclamped.")]
        public bool IsClamped = true;
        [SerializeField]
        private float rotationSpeed = 0.25f;
        [SerializeField]
        private float retargetTolerance = 1f;

        [Header("Debugging")]
        [SerializeField] private ExperienceMode previousMode = ExperienceMode.Exploration;
        [SerializeField] private ExperienceMode currentMode = ExperienceMode.Introduction;
        [SerializeField]
        private float remainingAngle = 0;
        bool shouldRotate = false;
        bool isRotating = false;

        public bool IsRotating => isRotating;

        Transform current = null;
        Transform target = null;

        private CameraBlink cameraBlink;

        #region References
        ExperienceModeController _modeController;
        #endregion

        public bool CanSetRotationTarget() => remainingAngle < retargetTolerance;
        
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
            currentMode = _modeController.CurrentMode;

            if (currentMode == previousMode) return;

            if (previousMode == ExperienceMode.Passive && currentMode == ExperienceMode.Exploration)// || currentMode == ExperienceMode.Introduction))
            {
                ListenForBlinkAndReturnToOrigin();
                return;
            }

            if (_modeController.CurrentMode == ExperienceMode.Exploration && remainingAngle > 0)
            {
                if(remainingAngle > 0)
                {
                    ResetState();
                }

                SetTargetTransformAndRotate(OriginTransform);
            }

            previousMode = currentMode;
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
            if (ShouldSlerp)
            {
                Slerp();
            }
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
            if (!CanSetRotationTarget())
            {
                Debug.Log($"Can't set target, currently rotating.");
                return;
            }

            target = targetTransform;
            shouldRotate = true;
            isRotating = true;
        }

        private void ListenForBlinkAndReturnToOrigin()
        {
            cameraBlink.EyeClosed.AddListener(ReturnToOrigin);
            previousMode = currentMode;
            ResetState();
        }

        private void ReturnToOrigin()
        {
            transform.rotation = OriginTransform.rotation;
            cameraBlink.EyeClosed.RemoveListener(ReturnToOrigin);
        }

        private void ResetState()
        {
            target = transform;
            shouldRotate = false;
            isRotating = false;
            remainingAngle = 0;
        }

        private void SubscribeToExperienceModeEvents()
        {
            _modeController = FindObjectOfType<ExperienceModeController>().Instance;
            Assert.IsNotNull(_modeController, "$[PassiveModeRotator] Cannot find a reference to ExperienceModeController.");
            _modeController.OnExperienceModeChanged += HandleModeChanged;

            cameraBlink = Camera.main.GetComponentInChildren<CameraBlink>();
            Assert.IsNotNull(cameraBlink, $"[PassiveModeRotator] cannot find CameraBlink component in main camera children.");
        }
    }
}