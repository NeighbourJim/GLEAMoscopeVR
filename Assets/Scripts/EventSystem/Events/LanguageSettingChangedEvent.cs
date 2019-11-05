using GLEAMoscopeVR.Settings;

namespace GLEAMoscopeVR.Events
{
    public class LanguageSettingChangedEvent : GlobalEvent
    {
        public LanguageSetting _LanguageSetting { get; }

        public LanguageSettingChangedEvent(LanguageSetting languageSetting, string eventMessage = "") : base(eventMessage)
        {
            _LanguageSetting = languageSetting;
        }
    }
}