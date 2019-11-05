using System.ComponentModel;

namespace GLEAMoscopeVR.Settings
{
    public enum VoiceoverSetting
    {
        [Description("Female Voice for Selected Language")]
        Female,
        [Description("Male Voice for Selected Language")]
        Male,
        [Description("No Voice-over")]
        None
    }
}