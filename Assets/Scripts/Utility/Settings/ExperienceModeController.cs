using System;
using System.IO;
using GLEAMoscopeVR.Interaction;
using GLEAMoscopeVR.POIs;
using GLEAMoscopeVR.Utility.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace GLEAMoscopeVR.Settings
{
    /// <summary>
    /// todo: MM fix once interaction functionality is decided and comment
    /// Added to assist with enabling and disabling functionality until a decision is made as to whether there will be separate modes
    /// or mixed functionality.
    /// For the sake of simplicity, the first iteration 02 prototype will separate Exploration and Passive modes.
    /// They can be toggled (resetting functionality to the appropriate state), but they will exist as separate systems.
    /// <see cref="ExperienceMode"/> 
    /// </summary>
    public class ExperienceModeController : MonoBehaviour, IActivatable
    {
        public ExperienceModeController Instance { get; private set; }
        private const ExperienceMode DEFAULT_MODE = ExperienceMode.Introduction;

        public PassiveModeRotator Rotator;
        public SunsetController SunsetController;
        private CameraBlink cameraBlink;



        [Header("State")]
        [SerializeField]
        private ExperienceMode currentMode = ExperienceMode.Introduction;
        
        [SerializeField] private bool tutorialComplete = false;

        [Header("Settings UI")] 
        public TextMeshProUGUI ModeText;
        public Button ModeButton;

        [Header("POI UI")]
        public InfoPanel WarTablePanel;
        public InfoPanel_Tooltip SkyPanel;

        [Header("IActivatable")]
        [SerializeField]
        private float activationTime = 1f;

        public ExperienceMode CurrentMode => currentMode;
        bool IActivatable.IsActivated => false;
        float IActivatable.ActivationTime => activationTime;

        /// <summary>
        /// Notifies dependent scripts that the interaction mode has changed.
        /// </summary>
        public event Action OnExperienceModeChanged;

        void Awake()
        {
            LazySingleton();
            SetAndCheckReferences();
        }

        private void ToggleExperienceMode()
        {
            switch (currentMode)
            {
                case ExperienceMode.Exploration:
                    currentMode = ExperienceMode.Passive;
                    FindObjectOfType<SoundEffects>().Play("Switch");
                    break;
                case ExperienceMode.Passive:
                    currentMode = ExperienceMode.Exploration;
                    FindObjectOfType<SoundEffects>().Play("Switch");
                    break;
                case ExperienceMode.Introduction:
                    currentMode = ExperienceMode.Introduction;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(
                        $"[ExperienceModeController] is trying to switch to an unsupported mode.");
            }

            ModeText.text = currentMode.GetDescription();
            ResetInfoPanels();
            //SaveUserSettings();
            OnExperienceModeChanged?.Invoke();
        }

        public void SetIntroductionSequenceComplete()//placeholder until persist data setup
        {
            currentMode = ExperienceMode.Exploration;
            OnExperienceModeChanged?.Invoke();
        }


        #region IActivatable Implementation
        bool IActivatable.CanActivate()
        {
            return Rotator.CanSetRotationTarget();
        }

        void IActivatable.Activate()
        {
            cameraBlink.Blink();
            ToggleExperienceMode();
        }

        void IActivatable.Deactivate() { return; }
        #endregion

        private void ResetInfoPanels()
        {
            SkyPanel.SetCanvasGroupState(false);
            WarTablePanel.SetCanvasGroupState(false);
        }

        private void LazySingleton()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else if (Instance != this)
            {
                Destroy(gameObject);
            }
        }

        
        
        
        private void SetAndCheckReferences()
        {
            Assert.IsNotNull(Rotator, $"[ExperienceModeController] does not have reference to the PassiveSkyRotator");
            Assert.IsNotNull(SkyPanel, $"[ExperienceModeController] does not have reference to the SkyInfoPanel");
            Assert.IsNotNull(WarTablePanel, $"[ExperienceModeController] does not have reference to the WarTablePanel");
            Assert.IsNotNull(ModeText, $"[ExperienceModeController] does not have reference to the Mode Text component.");
            Assert.IsNotNull(ModeButton, $"[ExperienceModeController] does not have reference to the Mode Button component.");
            Assert.IsNotNull(SunsetController, $"[ExperienceModeController] does not have reference to the SunsetController");

            cameraBlink = Camera.main.GetComponentInChildren<CameraBlink>();
            Assert.IsNotNull(cameraBlink, $"{gameObject.name} cannot find CameraBlink component in main camera children.");

            POIManager.Instance.OnAntennaPOIActivated += HandleAntennaPOIActivated;
        }

        private void HandleAntennaPOIActivated()
        {
            SunsetController.StartSunset();
        }

        #region Persist

        //private const string filename = "/settings.json";
        //[SerializeField]private UserSettings userSettings;

        //private void ApplySavedSettings()
        //{
        //    LoadUserSettings();

        //    currentMode = (ExperienceMode) userSettings.ExperienceMode;
        //    tutorialComplete = userSettings.TutorialComplete;
        //    ToggleExperienceMode();
        //    //todo: if not tutorial -> straight into appropriate mode
        //}

        //public void SaveUserSettings()
        //{
        //    string jsonSettings = JsonUtility.ToJson(userSettings);
        //    print($"SaveUserSettings - {jsonSettings}");
        //    File.WriteAllText(Application.persistentDataPath + "/user_settings.json", jsonSettings);
        //    Debug.Log(jsonSettings);
        //}

        //private UserSettings LoadUserSettings()
        //{
        //    if (File.Exists(Application.persistentDataPath + filename))
        //    {
        //        userSettings = JsonUtility.FromJson<UserSettings>(Application.persistentDataPath + filename);
        //    }
        //    else
        //    {
        //        userSettings = new UserSettings();
        //    }
        //    print($"LoadUserSettings - {userSettings}");
        //    return userSettings;
        //}

        #endregion
    }
}