using System;
using UnityEngine;
namespace GLEAMoscopeVR.Settings
{
    /// <summary>
    /// NOTE: I would have just made the properties public but I want to be able to serialise them in the inspector while we test. Can be changed later.
    /// </summary>
    [System.Serializable]
    public class UserSettings
    {
        #region Properties
        [SerializeField]
        private int _mode = 0;
        public int Mode
        {
            get => _mode;
            set
            {
                if (value >= 0 && value < Enum.GetValues(typeof(ExperienceMode)).Length)
                {
                    _mode = value;
                }
            }
        }
        public ExperienceMode ExperienceMode => (ExperienceMode)_mode;

        [SerializeField]
        private bool _blinkInPassiveMode = false;
        public bool BlinkInPassiveMode
        {
            get => _blinkInPassiveMode;
            set => _blinkInPassiveMode = value;
        }
        
        [SerializeField]
        private int _voice = 0;
        public int Voice
        {
            get => _voice;
            set
            {
                if (value >= 0 && value < Enum.GetValues(typeof(VoiceoverSetting)).Length)
                {
                    _voice = value;
                }
            }
        }
        public VoiceoverSetting VoiceSetting => (VoiceoverSetting)_voice;

        [SerializeField]
        private bool _showSubtitles = true;
        public bool ShowSubtitles
        {
            get => _showSubtitles;
            set => _showSubtitles = value;            
        }

        #endregion

        #region Constructors
        public UserSettings(int mode = 0, bool blinkInPassiveMode = false, int voice = 0, bool showSubtitles = true)
        {
            Mode = mode;
            BlinkInPassiveMode = blinkInPassiveMode;
            Voice = voice;
            ShowSubtitles = showSubtitles;
        }

        public UserSettings(ExperienceMode mode, bool blinkInPassiveMode, VoiceoverSetting voice, bool showSubtitles)
        {
            Mode = (int)mode;
            BlinkInPassiveMode = blinkInPassiveMode;
            Voice = (int)voice;
            ShowSubtitles = showSubtitles;
        }
        #endregion

        public string GetJsonString()
        {
            return JsonUtility.ToJson(this);
        }
    }
}