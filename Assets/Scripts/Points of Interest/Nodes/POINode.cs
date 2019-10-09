using GLEAMoscopeVR.Events;
using GLEAMoscopeVR.Interaction;
using GLEAMoscopeVR.Settings;
using UnityEngine;
using UnityEngine.Assertions;

namespace GLEAMoscopeVR.POIs
{
    /// <summary>
    /// Base class for Point of Interest nodes.
    /// <see cref="POIMapNode"/> <see cref="POISkyNode"/> <see cref="AntennaPOI"/>
    /// </summary>
    public abstract class POINode : MonoBehaviour, IActivatable
    {
        [Header("Data"), SerializeField]
        protected POIData data;

        [Header("IActivatable")]
        [SerializeField] protected float activationTime = 2f;
        [SerializeField] protected bool isActivated = false;

        protected Collider _collider;
        protected Renderer _renderer;
        
        #region Public Accessors
        public abstract POIData Data { get; }
        public abstract ExperienceMode ActivatableMode { get; }
        public abstract float ActivationTime { get; }
        public abstract bool IsActivated { get; }
        #endregion
        
        #region Unity Methods

        protected virtual void Awake()
        {
            SetAndCheckReferences();
        }

        protected virtual void OnEnable()
        {
            EventManager.Instance.AddListener<ExperienceModeChangedEvent>(HandleExperienceModeChanged);
            EventManager.Instance.AddListener<POINodeActivatedEvent>(HandleNodeActivated);
            EventManager.Instance.AddListener<SunsetStateChangedEvent>(HandleSunsetStateChanged);
        }

        protected virtual void OnDisable()
        {
            EventManager.Instance.RemoveListener<ExperienceModeChangedEvent>(HandleExperienceModeChanged);
            EventManager.Instance.RemoveListener<POINodeActivatedEvent>(HandleNodeActivated);
            EventManager.Instance.RemoveListener<SunsetStateChangedEvent>(HandleSunsetStateChanged);
        }
        #endregion

        protected abstract void SetVisibleState();
        
        #region IActivatable
        public abstract bool CanActivate();
        public abstract void Activate();
        public virtual void Deactivate()
        {
            if (!isActivated) return;
            isActivated = false;
        }
        #endregion

        #region Event Handlers
        protected abstract void HandleExperienceModeChanged(ExperienceModeChangedEvent e);
        protected virtual void HandleSunsetStateChanged(SunsetStateChangedEvent e)
        {
            if (e.State == EventState.Completed && SettingsManager.Instance.CurrentExperienceMode == ActivatableMode)
            {
                _renderer.enabled = true;
                _collider.enabled = true;
            }
        }
        protected virtual void HandleNodeActivated(POINodeActivatedEvent e)
        {
            if (e.Node != this)
            {
                Deactivate();
            }
        }
        #endregion

        protected virtual void SetAndCheckReferences()
        {
            _collider = GetComponent<Collider>();
            Assert.IsNotNull(_collider, $"<b>[{GetType().Name}]</b> {gameObject.name} has no collider component.");

            if (this is POIMapNode)
            {
                _renderer = GetComponent<Renderer>();
                Assert.IsNotNull(_renderer, $"<b>[{GetType().Name}]</b> {gameObject.name} has no renderer component.");
            }
            else
            {
                _renderer = GetComponentInChildren<Renderer>();
                Assert.IsNotNull(_renderer, $"<b>[{GetType().Name}]</b> {gameObject.name} has no renderer component in children.");
            }

            Assert.IsNotNull(data, $"<b>[{GetType().Name}]</b> {gameObject.name} POIData not assigned.");
        }
    }
}