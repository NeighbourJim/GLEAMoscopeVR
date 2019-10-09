using GLEAMoscopeVR.Cameras;
using GLEAMoscopeVR.Events;
using GLEAMoscopeVR.Events.GLEAMoscopeVR.Events;
using GLEAMoscopeVR.POIs;
using GLEAMoscopeVR.Settings;
using UnityEngine;
using UnityEngine.Assertions;

namespace GLEAMoscopeVR.Environment
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

        [Header("Rotation Settings")]
        [SerializeField, Tooltip("If true, spherical interpolation is used.\nIf false, linear interpolation is used.")]
        private bool shouldSlerp = true;
        [SerializeField, Tooltip("If true, speed is clamped.\nIf false, speed is unclamped.")]
        private bool isClamped = true;
        [SerializeField]
        private float rotationSpeed = 0.125f;
        [SerializeField]
        private float retargetTolerance = 10f;

        public CameraBlinkCanvas CameraBlink;
        public InfoPanelWarTable WarTablePanel;

        [SerializeField] private ExperienceMode previousMode;
        [SerializeField] private ExperienceMode currentMode;
        
        private float remainingAngle = 0;
        private bool shouldRotate = false;
        private bool shouldBlink = false;

        Transform currentTransform = null;
        Transform targetTransform = null;

        public bool IsRotating { get; private set; } = false;
        public bool CanSetRotationTarget() => remainingAngle < retargetTolerance;

        #region Unity Methods
        void Start()
        {
            SetIntialStateValues();
            SetAndCheckRefererences();
        }
        
        void OnEnable()
        {
            EventManager.Instance.AddListener<ExperienceModeChangedEvent>(HandleExperienceModeChanged);
            EventManager.Instance.AddListener<PassiveBlinkSettingChanged>(e => shouldBlink = e.BlinkInPassiveMode);
            EventManager.Instance.AddListener<POINodeActivatedEvent>(CheckNodeTypeAndSetTarget);
        }

        void OnDisable()
        {
            EventManager.Instance.RemoveListener<ExperienceModeChangedEvent>(HandleExperienceModeChanged);
            EventManager.Instance.RemoveListener<PassiveBlinkSettingChanged>(e => shouldBlink = e.BlinkInPassiveMode);
            EventManager.Instance.RemoveListener<POINodeActivatedEvent>(CheckNodeTypeAndSetTarget);
        }

        void Update()
        {
            currentTransform = transform;
            if (shouldRotate)
            {
                Rotate();
            }
        }
        #endregion

        private void SetIntialStateValues()
        {
            currentTransform = transform;
            targetTransform = transform;
            previousMode = SettingsManager.Instance.CurrentExperienceMode;
            currentMode = previousMode;
        }

        private void CheckNodeTypeAndSetTarget(POINodeActivatedEvent e)
        {
            if (e.Node is POIMapNode)
            {
                Debug.Log(e.Node.Data.SkyTransform);
                SetTargetTransform(e.Node.Data.SkyTransform);
            }
        }

        private void SetTargetTransform(Transform target)
        {
            if (!CanSetRotationTarget())
            {
                Debug.Log($"Can't set target, currently rotating.");
                return;
            }

            targetTransform = target;
            
            DetermineRotationBehaviour();
        }

        private void DetermineRotationBehaviour()
        {
            if (SettingsManager.Instance.BlinkInPassiveMode)
            {
                PrepareForBlink();
            }
            else
            {
                PrepareForRotation();
            }
        }

        #region Rotation
        private void PrepareForRotation()
        {
            shouldRotate = true;
            IsRotating = true;
        }

        private void Rotate()
        {
            if (shouldSlerp)
            {
                Slerp();
            }
            else
            {
                Lerp();
            }

            remainingAngle = Quaternion.Angle(transform.rotation, targetTransform.rotation);

            if (transform.rotation == targetTransform.rotation)
            {
                ResetState();
            }
        }

        private void Lerp()
        {
            if (isClamped)
            {
                transform.rotation = Quaternion.Lerp(
                    currentTransform.rotation,
                    targetTransform.rotation,
                    rotationSpeed * Time.deltaTime);
            }
            else
            {
                transform.rotation = Quaternion.LerpUnclamped(
                    currentTransform.rotation,
                    targetTransform.rotation,
                    rotationSpeed * Time.deltaTime);
            }
        }

        private void Slerp()
        {
            if (isClamped)
            {
                transform.rotation = Quaternion.Slerp(
                    currentTransform.rotation,
                    targetTransform.rotation,
                    rotationSpeed * Time.deltaTime);
            }
            else
            {
                transform.rotation = Quaternion.SlerpUnclamped(
                    currentTransform.rotation,
                    targetTransform.rotation,
                    rotationSpeed * Time.deltaTime);
            }
        }
        #endregion
        
        #region Blink
        private void PrepareForBlink()
        {
            CameraBlink.EyeClosed.AddListener(SetTransformDuringBlink);
            CameraBlink.Blink();
        }
        
        private void SetTransformDuringBlink()
        {
            transform.rotation = targetTransform.rotation;
            CameraBlink.EyeClosed.RemoveListener(SetTransformDuringBlink);
        }
        #endregion

        #region Reset
        private void ReturnToOrigin()
        {
            transform.rotation = OriginTransform.rotation;
            CameraBlink.EyeClosed.RemoveListener(ReturnToOrigin);

            previousMode = currentMode;
            ResetState();
        }
        
        private void ResetState()
        {
            targetTransform = transform;
            shouldRotate = false;
            IsRotating = false;
            remainingAngle = 0;
        }
        #endregion

        public void ResetStateForIntroSequence()
        {
            ReturnToOrigin();
            ResetState();
        }

        private void HandleExperienceModeChanged(ExperienceModeChangedEvent e)
        {
            currentMode = e.ExperienceMode;
            if (currentMode == previousMode) return;

            if (previousMode == ExperienceMode.Passive && currentMode == ExperienceMode.Exploration)
            {
                CameraBlink.EyeClosed.AddListener(ReturnToOrigin);
                return;
            }

            if (e.ExperienceMode == ExperienceMode.Exploration && remainingAngle > 0)
            {
                ResetState();
                SetTargetTransform(OriginTransform);
            }

            previousMode = currentMode;
        }

        private void SetAndCheckRefererences()
        {
            if (CameraBlink == null)
            {
                CameraBlink = Camera.main.GetComponentInChildren<CameraBlinkCanvas>();
            }
            Assert.IsNotNull(CameraBlink, $"<b>[{GetType().Name}]</b> Camera blink is not assigned and cannot be found in main camera children.");

            if (WarTablePanel == null)
            {
                WarTablePanel = FindObjectOfType<InfoPanelWarTable>();
            }
            Assert.IsNotNull(WarTablePanel, $"<b>[{GetType().Name}]</b> War table panel is not assigned and not found in scene.");
        }
    }
}