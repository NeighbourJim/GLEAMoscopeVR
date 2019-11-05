using GLEAMoscopeVR.Settings;

namespace GLEAMoscopeVR.Events
{
    public class VoiceoverSettingChangedEvent : GlobalEvent
    {
        public VoiceoverSetting VoiceoverSetting { get; }

        public VoiceoverSettingChangedEvent(VoiceoverSetting voiceoverSetting, string eventMessage = "") : base(eventMessage)
        {
            VoiceoverSetting = voiceoverSetting;
        }
    }
}