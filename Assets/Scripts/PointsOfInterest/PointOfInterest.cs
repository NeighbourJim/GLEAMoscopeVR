using UnityEngine;

namespace GLEAMoscopeVR.POIs
{
    /// <summary>
    /// Updated 12/04/19 - MM
    /// Converts a <see cref="POIData"/> ScriptableObject .asset file into an object using the data entered in the editor.
    /// Allows for the data to be sent between scripts and components without the need for additional, explicit data entry.
    /// NOTE: this class will change, but the way in which it is interacted with should not.
    /// </summary>
    [System.Serializable]
    public class PointOfInterest
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
        /// </summary>
        public string Distance => Data.Distance;

        /// <summary>
        /// Sprite used to display the Point of Interest in the user interface panel.
        /// </summary>
        public Sprite Sprite => Data.Sprite;

        /// <summary>
        /// Voice over to be played on Point of Interest activation.
        /// </summary>
        public AudioClip Voiceover => Data.Voiceover;

        #endregion

        #region Constructors

        public PointOfInterest(POIData data)
        {
            _data = data;
        }

        #endregion
    }
}