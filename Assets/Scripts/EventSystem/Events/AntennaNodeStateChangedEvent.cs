namespace GLEAMoscopeVR.Events
{
    public class AntennaNodeStateChangedEvent : GlobalEvent
    {
        public enum AntennaNodeState { Unavailable, Activatable, Activated, Deactivated }

        public AntennaNodeState NodeState { get; }

        public AntennaNodeStateChangedEvent(AntennaNodeState nodeState, string eventMessage = "") : base(eventMessage)
        {
            NodeState = nodeState;
        }
    }
}