using GLEAMoscopeVR.Settings;

namespace GLEAMoscopeVR.Events
{
    public class ExperienceModeChangedEvent : GlobalEvent
    {
        public ExperienceMode ExperienceMode { get; }
        
        public ExperienceModeChangedEvent(ExperienceMode experienceMode, string eventMessage = "") : base(eventMessage)
        {
            ExperienceMode = experienceMode;
        }
    }
}