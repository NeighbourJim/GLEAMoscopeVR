namespace GLEAMoscopeVR.Events
{
    public class VideoClipEndedEvent : GlobalEvent
    {
        public VideoClipEndedEvent(string eventMessage = "") : base(eventMessage) {}
    }
}