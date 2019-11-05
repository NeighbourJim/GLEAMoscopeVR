using UnityEngine;

namespace GLEAMoscopeVR.Events
{
    public class SubtitleSettingChangedEvent : GlobalEvent
    {
        public bool ShowSubtitles { get; }

        public SubtitleSettingChangedEvent(bool showSubtitles, string eventMessage = "") : base(eventMessage)
        {
            ShowSubtitles = showSubtitles;
        }
    }
}