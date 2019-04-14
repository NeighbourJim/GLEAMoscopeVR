using System.ComponentModel;

namespace GLEAMoscopeVR.Settings
{
    /// <summary>
    /// Defines the interaction modes available to the user.
    /// When set, functionality specific to the interaction mode will be enabled
    /// and functionality specific to other modes will be disabled.
    /// Currently, mixed interaction is excluded.
    /// </summary>
    [System.Serializable]
    public enum ExperienceMode
    {
        /// <summary>
        /// The user can interact with the POI nodes in the SKY ONLY.
        /// POI nodes will be displayed on the map, but will not activate.
        /// </summary>
        [Description("Exploration Mode")]
        Exploration,

        /// <summary>
        /// The user can interact with the POI nodes on the map at the WAR TABLE ONLY.
        /// POI nodes will be displayed in they sky, but they will not activate.
        /// </summary>
        [Description("Passive Mode")]
        Passive,

        // Todo: to be decided.
        //[Description("Mixed Mode")]
        //Mixed 
    }
}