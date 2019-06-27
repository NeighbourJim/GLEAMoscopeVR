using System.IO;
using UnityEngine;

namespace GLEAMoscopeVR.Settings
{
    /// <summary>
    /// Handles user settings changes, serialisation and deserialising of json data.
    /// </summary>
    public class SettingsManager : MonoBehaviour
    {
        private string filePath;
        private string fileName = "/settingsData.json";

        [SerializeField]
        private UserSettings userSettings;
        public UserSettings UserSettings
        {
            get
            {
                if (userSettings == null)
                {
                    LoadSettings();
                }

                return userSettings;
            }
        }

        #region Unity Methods
        void Awake()
        {
            SetAndCheckReferences();

            filePath = Application.persistentDataPath + fileName;

            userSettings = new UserSettings();
            LoadSettings();
        }

        void OnApplicationQuit()
        {
            SaveSettings();
        }

        #endregion
        
        #region Placeholder Settings Methods
        public void SetExperienceMode(ExperienceMode mode)
        {
            UserSettings.Mode = (int) mode;
            SaveSettings();
        }

        public void SetBlinkInPassiveMode(bool blinkInPassiveMode)
        {
            UserSettings.BlinkInPassiveMode = blinkInPassiveMode;
            SaveSettings();
        }

        public void SetVoiceSetting(VoiceoverSetting voiceSetting)
        {
            UserSettings.Voice = (int) voiceSetting;
            SaveSettings();
        }

        public void SetSubtitleSetting(bool showSubtitles)
        {
            UserSettings.ShowSubtitles = showSubtitles;
            SaveSettings();
        }
        #endregion

        #region Serialise / Deserialise
        public void LoadSettings()
        {
            try
            {
                var jsonString = File.ReadAllText(filePath);
                userSettings = JsonUtility.FromJson<UserSettings>(jsonString);
                Debug.Log($"<b>[SettingsManager]</b> loaded user settings: {jsonString}");
            }
            catch
            {
                userSettings = new UserSettings();
                Debug.Log($"<b>[SettingsManager]</b> instantiated new user settings: {userSettings.GetJsonString()}");
            }

            Debug.Log($"<b>[SettingsManager]</b> Settings after loading: {userSettings.GetJsonString()}");
        }

        public void SaveSettings()
        {
            if (userSettings == null)
            {
                userSettings = new UserSettings();
            }

            var jsonString = userSettings.GetJsonString();

            if (!File.Exists(filePath))
            {
                File.Create(filePath);
                Debug.Log($"<b>[SettingsManager]</b> file created. Path {filePath}");
            }

            File.WriteAllText(filePath, jsonString);

            Debug.Log($"<b>[SettingsManager]</b> wrote settings to file: {jsonString}");
        }
        #endregion

        #region Debugging
        private void SetAndCheckReferences() { }
        #endregion
    }
}