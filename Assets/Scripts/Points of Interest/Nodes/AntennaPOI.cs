using GLEAMoscopeVR.Events;
using GLEAMoscopeVR.Interaction;
using GLEAMoscopeVR.Settings;
using UnityEngine;
using UnityEngine.Assertions;

namespace GLEAMoscopeVR.POIs
{
    /// <summary>
    /// Behaviour for Antenna Point of Interest made available in <see cref="ExperienceMode.Introduction"/>.
    /// NOTE: No longer inherits from <see cref="POINode"/>.
    /// </summary>
    public class AntennaPOI : MonoBehaviour, IActivatable, IAnimatedPrompt
    {
        #region Constants
        [HideInInspector] public string IdleTrigger => "SetIdle";
        [HideInInspector] public string AnimateTrigger => "Bounce";
        #endregion

        [Header("UI Content")]
        public Renderer HaloRenderer;
        public Renderer ArrowRenderer;
        public Animator ArrowAnimator;

        [Space] public POIData Data;

        [Header("IActivatable")]
        [SerializeField] private float activationTime = 2f;
        [SerializeField] private bool isActivated = false;
        
        [Header("Debugging"), SerializeField]
        private bool activatable = false;

        #region References
        Collider _collider;
        Renderer _renderer;
        #endregion

        #region Public Accessors
        float IActivatable.ActivationTime => activationTime;
        public bool IsActivated => isActivated;
        Renderer IAnimatedPrompt.Renderer => ArrowRenderer;
        Animator IAnimatedPrompt.Animator => ArrowAnimator;
        #endregion

        #region Unity Methods
        void Awake()
        {
            SetAndCheckReferences();
            TogglePrompt(false);
        }

        void OnEnable()
        {
            EventManager.Instance.AddListener<ExperienceModeChangedEvent>(HandleExperienceModeChanged);
        }

        void OnDisable()
        {
            EventManager.Instance.RemoveListener<ExperienceModeChangedEvent>(HandleExperienceModeChanged);
        }
        #endregion

        #region IActivatable Implentation
        public bool CanActivate() => SettingsManager.Instance.CurrentExperienceMode == ExperienceMode.Introduction && activatable && !isActivated;
        
        public void Activate()
        {
            isActivated = true;
            TogglePrompt(false);
            
            EventManager.Instance.Raise(new AntennaNodeStateChangedEvent(AntennaNodeStateChangedEvent.AntennaNodeState.Activated, "Antenna node activated."));
        }

        public void Deactivate()
        {
            isActivated = false;
            TogglePrompt(false);

            EventManager.Instance.Raise(new AntennaNodeStateChangedEvent(AntennaNodeStateChangedEvent.AntennaNodeState.Deactivated, "Antenna node deactivated."));
        }
        #endregion

        public void TogglePrompt(bool shouldPrompt)
        {
            activatable = shouldPrompt;
            _collider.enabled = shouldPrompt;
            _renderer.enabled = shouldPrompt;
            HaloRenderer.enabled = shouldPrompt;
            ArrowRenderer.enabled = shouldPrompt;
            ArrowAnimator.enabled = shouldPrompt;
            if (shouldPrompt)
            {
                ArrowAnimator.SetTrigger(AnimateTrigger);
            }
        }

        private void HandleExperienceModeChanged(ExperienceModeChangedEvent e)
        {
            switch (e.ExperienceMode)
            {
                case ExperienceMode.Exploration:
                case ExperienceMode.Passive:
                    TogglePrompt(false);
                    break;
                case ExperienceMode.Introduction:
                default:
                    break;
            }
        }
        
        private void SetAndCheckReferences()
        {
            _collider = GetComponent<Collider>();
            Assert.IsNotNull(_collider, $"<b>[{GetType().Name}]</b> has no Collider component.");

            _renderer = GetComponentInChildren<Renderer>();
            Assert.IsNotNull(_renderer, $"<b>[{GetType().Name}]</b> has no renderer component in children.");

            Assert.IsNotNull(HaloRenderer, $"<b>[{GetType().Name}]</b> halo renderer has not been assigned.");
            Assert.IsNotNull(ArrowRenderer, $"<b>[{GetType().Name}]</b> arrow renderer has not been assigned.");
            Assert.IsNotNull(ArrowAnimator, $"<b>[{GetType().Name}]</b> arrow animator has not been assigned.");
        }
    }
}
