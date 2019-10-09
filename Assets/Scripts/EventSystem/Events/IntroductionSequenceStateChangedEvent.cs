namespace GLEAMoscopeVR.Events
{
    public class IntroductionSequenceStateChangedEvent : GlobalEvent
    {
        public enum IntroState { Inactive, Playing, Skipped, Complete, }
        public IntroState State { get; }

        public IntroductionSequenceStateChangedEvent(IntroState state, string eventMessage = "") : base(eventMessage)
        {
            State = state;
        }
    }
}