using System;
using UnityEngine;

namespace GLEAMoscopeVR.Settings
{
    [System.Serializable]
    public class UserSettings
    {
        #region Properties
        [SerializeField]
        private int _experienceMode = 0;
        public int ExperienceMode
        {
            get => _experienceMode;
            set
            {
                if (value >= 0 && value < Enum.GetValues(typeof(ExperienceMode)).Length)
                {
                    _experienceMode = value;
                }
            }
        }


        [SerializeField]
        private int _voiceoverSetting = 0;
        public int VoiceoverSetting
        {
            get => _voiceoverSetting;
            set
            {
                if (value >= 0 && value < Enum.GetValues(typeof(VoiceoverSetting)).Length)
                {
                    _voiceoverSetting = value;
                }
            }
        }

        [SerializeField]
        private int _languageSetting = 0;
        public int LanguageSetting
        {
            get => _languageSetting;
            set
            {
                if(value >= 0 && value < Enum.GetValues(typeof(LanguageSetting)).Length)
                {
                    _languageSetting = value;
                }
            }
        }

        
        [SerializeField]
        private bool _showSubtitles = true;
        public bool ShowSubtitles
        {
            get => _showSubtitles;
            set => _showSubtitles = value;            
        }

        [SerializeField]
        private bool _blinkInPassiveMode = false;
        public bool BlinkInPassiveMode
        {
            get => _blinkInPassiveMode;
            set => _blinkInPassiveMode = value;
        }
        #endregion

        #region Constructors
        public UserSettings(int experienceMode = 0, int voiceoverSetting = 0, int languageSetting = 0, bool showSubtitles = true, bool blinkInPassiveMode = false)
        {
            ExperienceMode = experienceMode;
            VoiceoverSetting = voiceoverSetting;
            LanguageSetting = languageSetting;
            ShowSubtitles = showSubtitles;
            BlinkInPassiveMode = blinkInPassiveMode;
        }
        #endregion
    }
}