namespace GLEAMoscopeVR.Events
{
    public class TeleportToSceneEvent : GlobalEvent
    {
        public TeleportToSceneEvent(string eventMessage = "") : base(eventMessage) { }
    }
}