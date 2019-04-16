﻿using System;
using GLEAMoscopeVR.POIs;
using GLEAMoscopeVR.Utility.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;

namespace GLEAMoscopeVR.Settings
{
    /// <summary>
    /// 20190413 MM - Added to assist with enabling and disabling functionality until a decision is made as to whether there will be separate modes
    /// or mixed functionality.
    /// For the sake of simplicity, the first iteration 02 prototype will separate Exploration and Passive modes.
    /// They can be toggled (resetting functionality to the appropriate state), but they will exist as separate systems.
    /// <see cref="ExperienceMode"/> 
    /// </summary>
    public class ExperienceModeController : MonoBehaviour//GenericSingleton<ExperienceModeController>
    {
        public static ExperienceModeController Instance { get; private set; }

        private const ExperienceMode DEFAULT_MODE = ExperienceMode.Exploration;

        [Header("Settings Components")]
        public TextMeshProUGUI ModeText;

        [Header("Display Components")]
        public InfoPanel_WarTable WarTablePanel;
        public InfoPanel_Manager SkyPanel;

        [SerializeField]
        private ExperienceMode currentMode = ExperienceMode.Exploration;
        public ExperienceMode CurrentMode => currentMode;

        /// <summary>
        /// Notifies dependent scripts that the interaction mode has changed.
        /// </summary>
        public event Action OnExperienceModeChanged;

        void Awake()
        {
            LazySingleton();
            GetComponentReferences();
        }

        public void ToggleExperienceMode()
        {
            switch (currentMode)
            {
                case ExperienceMode.Exploration:
                    currentMode = ExperienceMode.Passive;
                    break;
                case ExperienceMode.Passive:
                    currentMode = ExperienceMode.Exploration;
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"[ExperienceModeController] is trying to switch to an unsupported mode.");
            }

            ModeText.text = currentMode.GetDescription();
            ResetInfoPanels();

            OnExperienceModeChanged?.Invoke();
        }

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

        private void GetComponentReferences()
        {
            Assert.IsNotNull(SkyPanel, $"[ExperienceModeController] does not have reference to the SkyInfoPanel");
            Assert.IsNotNull(WarTablePanel, $"[ExperienceModeController] does not have reference to the WarTablePanel");
            Assert.IsNotNull(ModeText, $"[ExperienceModeController] does not have reference to the Mode Text component.");
        }
    }
}