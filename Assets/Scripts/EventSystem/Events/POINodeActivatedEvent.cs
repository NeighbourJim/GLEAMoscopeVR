using GLEAMoscopeVR.POIs;

namespace GLEAMoscopeVR.Events
{
    public class POINodeActivatedEvent : GlobalEvent
    {
        public POINode Node { get; }
        
        public POINodeActivatedEvent(POINode node, string eventMessage = "") : base(eventMessage)
        {
            Node = node;
        }
    }
}