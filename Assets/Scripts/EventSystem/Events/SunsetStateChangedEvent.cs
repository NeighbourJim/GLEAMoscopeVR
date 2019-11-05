namespace GLEAMoscopeVR.Events
{
    public class SunsetStateChangedEvent : GlobalEvent
    {
        public EventState State { get; }

        public SunsetStateChangedEvent(EventState state, string eventMessage = "") : base(eventMessage)
        {
            State = state;
        }
    }
}