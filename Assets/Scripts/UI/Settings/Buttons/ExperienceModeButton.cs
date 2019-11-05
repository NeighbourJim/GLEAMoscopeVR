using System.Linq;
using GLEAMoscopeVR.Events;
using GLEAMoscopeVR.Interaction;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace GLEAMoscopeVR.Settings
{
    [RequireComponent(typeof(Collider))]
    public class ExperienceModeButton : MonoBehaviour, IActivatable
    {
        [Header("Setting")]
        [SerializeField] private ExperienceMode experienceMode = ExperienceMode.Introduction;

        [Space]
        public Image SelectedImage;
        public ExperienceModeButton AlternateButton;
        //public ReplayIntroductionButton IntroductionButton;

        [Header("IActivatable")]
        [SerializeField] private float activationTime = 2f;
        [SerializeField] private bool isActivated = false;
        [SerializeField] private bool canActivate = true;

        public ExperienceMode ButtonMode => experienceMode;
        float IActivatable.ActivationTime => activationTime;
        bool IActivatable.IsActivated => isActivated;

        #region Unity Methods
        void Start()
        {
            SetAndCheckReferences();
            SetDisplayValues();
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

        private void SetDisplayValues()
        {
            if (SettingsManager.Instance.CurrentExperienceMode == experienceMode)
            {
                SetActiveState();
            }
            else
            {
                SetInactiveState();
            }
        }

        private void SetActiveState()
        {
            SelectedImage.enabled = true;
            canActivate = false;
        }

        private void SetInactiveState()
        {
            SelectedImage.enabled = false;
            canActivate = true;
        }

        bool IActivatable.CanActivate()
        {
            return !isActivated && experienceMode != SettingsManager.Instance.CurrentExperienceMode;
        }

        void IActivatable.Activate()
        {
            isActivated = true;
            SelectedImage.enabled = true;
            AlternateButton.Deactivate();
            SettingsManager.Instance.SetExperienceMode(experienceMode);
        }

        public void Deactivate()
        {
            isActivated = false;
            SelectedImage.enabled = false;
            canActivate = true;
        }

        private void HandleExperienceModeChanged(ExperienceModeChangedEvent e)
        {
            if (e.ExperienceMode == experienceMode)
            {
                SetActiveState();
            }
            else
            {
                SetInactiveState();
            }
        }
        
        private void SetAndCheckReferences()
        {
            Assert.IsNotNull(AlternateButton, $"<b>[{GetType().Name} - {gameObject.name}]</b> Alternate button not assigned.");
            
            FindObjectsOfType<ExperienceModeButton>().ToList().ForEach(b =>
            {
                if (b != this)
                {
                    Assert.IsFalse(b.ButtonMode == experienceMode,
                        $"<b>[{GetType().Name} - {gameObject.name}]</b> there is more than one ExperienceModeButton in the scene with experience mode {experienceMode}.");
                }
                Assert.IsFalse(b.ButtonMode == ExperienceMode.Introduction,
                    $"<b>[{GetType().Name} - {gameObject.name}]</b>  mode is set to Introduction. This should not be the case.");
            });
        }
    }
}