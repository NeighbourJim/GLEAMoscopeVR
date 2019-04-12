using System;

namespace MM.GLEAMoscopeVR.POIs
{
    /// <summary>
    /// Point of interest nodes in full-sky imagery. 
    /// </summary>
    public class POISkyNode : POINode
    {
        public override event Action<PointOfInterest> OnNodeActivated;
        
        public override void Activate() {}
    }
}