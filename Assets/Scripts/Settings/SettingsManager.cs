using System.IO;
using GLEAMoscopeVR.Events;
using GLEAMoscopeVR.Events.GLEAMoscopeVR.Events;
using GLEAMoscopeVR.Utility;
using UnityEngine;

namespace GLEAMoscopeVR.Settings
{
    /// <summary>
    /// Handles user settings changes, serialisation and deserialising of json data.
    /// </summary>
    public class SettingsManager : GenericSingleton<SettingsManager>
    {
        [Header("DEBUGGING")]
        [SerializeField] private bool disablePersistentSettings = false;

        [Header("Current Values (Read only)")]
        [SerializeField] private ExperienceMode currentMode;
        [SerializeField] private VoiceoverSetting voiceoverSetting;
        [SerializeField] private bool shouldShowSubtitles;
        [SerializeField] private bool passiveBlinkEnabled;

        private UserSettings userSettings;
        public UserSettings UserSettings => userSettings ?? (userSettings = new UserSettings());

        public ExperienceMode CurrentExperienceMode
        {
            get
            {
                currentMode = (ExperienceMode) UserSettings.ExperienceMode;
                return currentMode;
            }
        }
        public VoiceoverSetting CurrentVoiceoverSetting
        {
            get
            {
                voiceoverSetting = (VoiceoverSetting) UserSettings.VoiceoverSetting;
                return voiceoverSetting;
            }
        }
        public bool ShowSubtitles
        {
            get
            {
                shouldShowSubtitles = UserSettings.ShowSubtitles;
                return shouldShowSubtitles;
            }
        }
        public bool BlinkInPassiveMode
        {
            get
            {
                passiveBlinkEnabled = UserSettings.BlinkInPassiveMode;
                return passiveBlinkEnabled;
            }
        }
        
        #region Unity Methods
        protected override void Awake()
        {
            base.Awake();
        #if UNITY_EDITOR
            if (!disablePersistentSettings)
            {
                LoadSettings();
            }
            else
            {
                Debug.Log($"<b>[{GetType().Name}] NOT USING PERSISTENT SETTINGS.</b>");
                userSettings = new UserSettings();
            }
            #else
            LoadSettings();
        #endif
        }
        #endregion

        #region Settings Methods
        public void SetExperienceMode(ExperienceMode mode)
        {
            UserSettings.ExperienceMode = (int) mode;
            EventManager.Instance.Raise(new ExperienceModeChangedEvent(mode, $"Experience mode changed to {mode}"));
            currentMode = mode;
            if (!disablePersistentSettings)
            {
                SaveSettings();
            }
        }

        public void SetBlinkInPassiveMode(bool blinkInPassiveMode)
        {
            UserSettings.BlinkInPassiveMode = blinkInPassiveMode;
            EventManager.Instance.Raise(new PassiveBlinkSettingChanged(blinkInPassiveMode, $"Blink in passive mode setting changed to {blinkInPassiveMode}"));
            passiveBlinkEnabled = blinkInPassiveMode;
            if (!disablePersistentSettings)
            {
                SaveSettings();
            }
        }

        public void SetVoiceSetting(VoiceoverSetting voiceSetting)
        {
            UserSettings.VoiceoverSetting = (int) voiceSetting;
            EventManager.Instance.Raise(new VoiceoverSettingChangedEvent(voiceSetting, $"Voice setting changed to {voiceSetting}"));
            voiceoverSetting = voiceSetting;
            if (!disablePersistentSettings)
            {
                SaveSettings();
            }
        }

        public void SetSubtitleSetting(bool showSubtitles)
        {
            UserSettings.ShowSubtitles = showSubtitles;
            EventManager.Instance.Raise(new SubtitleSettingChangedEvent(showSubtitles, $"Show subtitle setting changed to {showSubtitles}"));
            shouldShowSubtitles = showSubtitles;
            if (!disablePersistentSettings)
            {
                SaveSettings();
            }
        }
        #endregion

        #region Serialise / Deserialise Settings (DC)
        public void SaveSettings()
        {
            Debug.Log($"SaveSettings() invoked. Settings: {userSettings}");

            string userSettingsPath = Path.Combine(Application.persistentDataPath, "userSettings.json");
            File.WriteAllText(userSettingsPath, JsonUtility.ToJson(userSettings, true));
        }

        public void LoadSettings()
        {
            string userSettingsPath = Path.Combine(Application.persistentDataPath, "userSettings.json");

            if (File.Exists(userSettingsPath))
            {
                try
                {
                    userSettings = JsonUtility.FromJson<UserSettings>(File.ReadAllText(userSettingsPath));
                    Debug.Log($"<b>[{GetType().Name}]</b> Settings loaded. " +
                              $"<b>Mode:</b> {userSettings.ExperienceMode}, " +
                              $"<b>Voice:</b> {userSettings.VoiceoverSetting}, " +
                              $"<b>Subtitles:</b> {userSettings.ShowSubtitles}, " +
                              $"<b>Passive blink:</b> {userSettings.BlinkInPassiveMode}.");
                }
                catch
                {
                    Debug.Log("User settings was found but may be corrupted. Creating new user settings.");
                    userSettings = new UserSettings();
                    SaveSettings();
                }
            }
            else
            {
                Debug.Log("No user settings found. Creating new user settings.");
                userSettings = new UserSettings();
                SaveSettings();
            }

            NotifySettingsChanged();
        }
        #endregion

        private void NotifySettingsChanged()
        {
            EventManager.Instance.Raise(new ExperienceModeChangedEvent(CurrentExperienceMode, $"Experience mode setting changed: {CurrentExperienceMode}"));
            currentMode = CurrentExperienceMode;

            EventManager.Instance.Raise(new VoiceoverSettingChangedEvent(CurrentVoiceoverSetting, $"Voiceover setting changed: {CurrentVoiceoverSetting}"));
            voiceoverSetting = CurrentVoiceoverSetting;

            EventManager.Instance.Raise(new SubtitleSettingChangedEvent(ShowSubtitles, $"Show subtitle setting changed: {ShowSubtitles}"));
            shouldShowSubtitles = ShowSubtitles;

            EventManager.Instance.Raise(new PassiveBlinkSettingChanged(BlinkInPassiveMode, $"Blink in passive mode setting changed: {BlinkInPassiveMode}"));
            passiveBlinkEnabled = BlinkInPassiveMode;
        }
    }
}