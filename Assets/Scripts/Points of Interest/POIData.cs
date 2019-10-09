using UnityEngine;

namespace GLEAMoscopeVR.POIs
{
    /// <summary>
    /// Stores Point of Interest data that is passed to the constructor of the <see cref="POIObject"/> class to instantiate an instance at runtime.
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
        /// </summary>
        [Tooltip("2-3 sentence description of the Point of Interest that will be displayed " +
                 "in both the tooltip-style panel at the appropriate location in the sky" +
                 "and the panel above the war-table."), TextArea(3, 10)]
        public string Description;

        /// <summary>
        /// The Point of Interest's approximate distance from Earth.
        /// </summary>
        [Tooltip("The Point of Interest's approximate distance from Earth (units to be confirmed).")]
        public string Distance;
        
        /// <summary>
        /// The Point of Interest's sprites in each of the 6 wavelengths.
        /// </summary>
        public Sprite[] Sprites = {};

        /// <summary>
        /// The Transform used to rotate the Point of Interest into the user's original, forward-facing viewport.
        /// </summary>
        [Tooltip("The Transform used to rotate the Point of Interest into the user's original, forward-facing viewport.")]
        public Transform SkyTransform;

        /// <summary>
        /// The X-axis offset used to position the <see cref="InfoPanelBase"/> for a Point of Interest activated in the sky.
        /// </summary>
        [Header("Info-Panel (Sky) Offset Values")]
        [Tooltip("The X-axis offset used to position the InfoPanel for a Point of Interest activated in the sky.")]
        public float XOffset = 0;
        /// <summary>
        /// The Y-axis offset used to position the <see cref="InfoPanelBase"/> for a Point of Interest activated in the sky.
        /// </summary>
        [Tooltip("The Y-axis offset used to position the InfoPanel for a Point of Interest activated in the sky.")]
        public float YOffset = 0;
        /// <summary>
        /// The Z-axis offset used to position the <see cref="InfoPanelBase"/> for a Point of Interest activated in the sky.
        /// </summary>
        [Tooltip("The Z-axis offset used to position the InfoPanel for a Point of Interest activated in the sky.")]
        public float ZOffset = 0;

        /// <summary>
        /// Voice over to be played on Point of Interest activation (Male variant).
        /// </summary>
        [Tooltip("Voice over to be played when the Point of Interest is activated.")]
        public AudioClip VoiceoverMale;

        /// <summary>
        /// Voice over to be played on Point of Interest activation (Female variant).
        /// </summary>
        [Tooltip("Voice over to be played when the Point of Interest is activated.")]
        public AudioClip VoiceoverFemale;

        #region Unity Methods
        void OnEnable()
        {
            ID = GetInstanceID();
        }
        #endregion
    }
}