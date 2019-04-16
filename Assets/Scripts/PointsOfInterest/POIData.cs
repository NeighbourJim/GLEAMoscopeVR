using UnityEngine;

namespace GLEAMoscopeVR.POIs
{
    /// <summary>
    /// Updated 12/04/19 - MM
    /// Stores Point of Interest data that can be entered in the Unity Editor
    /// and passed to the constructor of the <see cref="POIObject"/> class
    /// to instantiate an instance at runtime.
    /// NOTE: this class will change, but the way in which it is interacted with should not.
    /// </summary>
    [CreateAssetMenu(menuName = "GLEAMoscopeVR/POI", fileName = "New POIData")]
    public class POIData : ScriptableObject
    {
        /// <summary>
        /// Unique ID for the ScriptableObject .asset file. 
        /// </summary>
        public int ID;
        
        /// <summary>
        /// The Point of Interest's common name.
        /// </summary>
        [Tooltip("The Point of Interest's common name.")]
        public string Name;

        /// <summary>
        /// The Point of Interest's short description displayed in tooltip-style panels at the appropriate location in the sky.
        /// <see cref="longDescription"/> - not implemented but can be added on request.
        /// </summary>
        [Tooltip("2-3 sentence description of the Point of Interest that will be displayed " +
                 "in both the tooltip-style panel at the appropriate location in the sky" +
                 "and the panel above the war-table."), TextArea(3, 10)]
        public string ShortDescription;

        /// <summary>
        /// The Point of Interest's approximate distance from Earth (units to be confirmed).
        /// </summary>
        [Tooltip("The Point of Interest's approximate distance from Earth (units to be confirmed).")]
        public string Distance;

        /// <summary>
        /// Sprite used to display the Point of Interest in the user interface panel.
        /// </summary>
        [Tooltip("Sprite used to display the Point of Interest in the user interface panel.")]
        public Sprite Sprite;

        /// <summary>
        /// The Transform used to rotate the Point of Interest into the user's original, forward-facing viewport.
        /// </summary>
        [Tooltip("The Transform used to rotate the Point of Interest into the user's original, forward-facing viewport.")]
        public Transform SkyTransform;

        /// <summary>
        /// Voice over to be played on Point of Interest activation.
        /// </summary>
        [Tooltip("Voice over to be played when the Point of Interest is activated.")]
        public AudioClip Voiceover;

        #region Unity Methods
        void OnEnable()
        {
            ID = GetInstanceID();
        }
        #endregion
        
        #region Currently Excluded
        /// <summary>
        /// Todo: confirm required distance format(s) 
        /// </summary>
        private string distanceLightYears;
        private string distanceParsecs;

        /// <summary>
        /// Can be added if ICRAR want to display additional data at the war table.
        /// </summary>
        private string longDescription;

        /// <summary>
        /// Todo: Transform of the Point of Interest Node at app start-up.
        /// Coordinates to be provided by ICRAR and converted to spherical and/or cartesian for position tracking.
        /// </summary>
        private Transform DefaultTransform;

        #endregion
    }
}