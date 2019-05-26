using UnityEngine;

namespace GLEAMoscopeVR.POIs
{
    /// <summary>
    /// MM: Updated 21/05/19 Added array that stores POI sprites for each wavelength, male and female voice-over audio clips.
    /// Converts a <see cref="POIData"/> ScriptableObject .asset file into an object.
    /// Allows for the data to be sent between scripts and components without the need for additional, explicit data entry.
    /// </summary>
    [System.Serializable]
    public class POIObject
    {
        #region Properties

        /// <summary>
        /// Point of Interest ScriptableObject .asset file used to extract the data entered in the Editor.
        /// </summary>
        private POIData _data;

        public POIData Data => _data;

        /// <summary>
        /// <see cref="POIData"/> .asset file's unique ID (obtained using the GetInstanceID() method).
        /// </summary>
        public int ID => Data.ID;

        /// <summary>
        /// The Point of Interest's common name.
        /// </summary>
        public string Name => Data.Name;

        /// <summary>
        /// The Point of Interest's short description.
        /// </summary>
        public string Description => Data.ShortDescription;

        /// <summary>
        /// The Point of Interest's approximate distance from Earth (units to be confirmed).
        /// Todo: determine whether this is required (descriptions and scripts provided by ICRAR contain this info (where applicable). May not be necessary to display in UI.
        /// </summary>
        public string Distance => Data.Distance;

        /// <summary>
        /// The Point of Interest's sprites in each of the 6 wavelengths.
        /// Todo: determine whether sprites or textures / materials are more appropriate.
        /// </summary>
        public Sprite[] Sprites => Data.Sprites;

        /// <summary>
        /// The Transform used to rotate the Point of Interest into the user's original, forward-facing viewport.
        /// </summary>
        public Transform SkyTransform => Data.SkyTransform;

        /// <summary>
        /// The X-axis offset used to position the <see cref="InfoPanel"/> for a Point of Interest activated in the sky.
        /// </summary>
        public float XOffset => Data.XOffset;

        /// <summary>
        /// The Y-axis offset used to position the <see cref="InfoPanel"/> for a Point of Interest activated in the sky.
        /// </summary>
        public float YOffset => Data.YOffset;

        /// <summary>
        /// The Z-axis offset used to position the <see cref="InfoPanel"/> for a Point of Interest activated in the sky.
        /// </summary>
        public float ZOffset => Data.ZOffset;

        /// <summary>
        /// Voice over to be played on Point of Interest activation (Male variant).
        /// </summary>
        public AudioClip VoiceoverMale => Data.VoiceoverMale;

        /// <summary>
        /// Voice over to be played on Point of Interest activation (Female variant).
        /// </summary>
        public AudioClip VoiceoverFemale => Data.VoiceoverFemale;

        #endregion

        #region Constructor

        public POIObject(POIData data)
        {
            _data = data;
        }

        #endregion

        #region Currently Excluded
        //public Sprite GammaSprite => Data.GammaSprite;
        //public Sprite XRaySprite => Data.XRaySprite;
        //public Sprite VisibleSprite => Data.VisibleSprite;
        //public Sprite FarInfraredSprite => Data.FarInfraredSprite;
        //public Sprite MicrowaveSprite => Data.MicrowaveSprite;
        //public Sprite RadioSprite => Data.RadioSprite;
        #endregion
    }
}