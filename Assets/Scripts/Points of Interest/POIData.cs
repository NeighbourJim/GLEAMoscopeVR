using UnityEngine;
using GLEAMoscopeVR.Settings;

namespace GLEAMoscopeVR.POIs
{
    /// <summary>
    /// Stores Point of Interest data that is passed to the constructor of the <see cref="POIObject"/> class to instantiate an instance at runtime.
    /// </summary>
    [CreateAssetMenu(menuName = "GLEAMoscopeVR/POI", fileName = "New POIData")]
    public class POIData : ScriptableObject
    {
        [Header("Non-Language Data")]
        /// <summary>
        /// Unique ID for the ScriptableObject .asset file. 
        /// </summary>
        public int ID;
        /// <summary>
        /// The Point of Interest's sprites in each of the 6 wavelengths.
        /// </summary>
        public Sprite[] Sprites = { };

        [Header("English Data")]
        [Tooltip("The Point of Interest's common name in English.")]
        public string EnglishName = "?ENG NAME?";
        [Tooltip("2-3 sentence English description of the Point of Interest that will be displayed " +
                 "in both the tooltip-style panel at the appropriate location in the sky" +
                 "and the panel above the war-table."), TextArea(3, 10)]
        public string EnglishDescription = "?ENG DESC?";
        [Tooltip("The Point of Interest's approximate distance from Earth in English.")]
        public string EnglishDistance = "?ENG DIST?";
        public AudioClip EnglishVOMale;
        public AudioClip EnglishVOFemale;

        [Header("Italian Data")]
        [Tooltip("The Point of Interest's common name in Italian.")]
        public string ItalianName = "?ITA NAME?";
        [Tooltip("2-3 sentence Italian description of the Point of Interest that will be displayed " +
                 "in both the tooltip-style panel at the appropriate location in the sky" +
                 "and the panel above the war-table."), TextArea(3, 10)]
        public string ItalianDescription = "?ITA DESC?";
        [Tooltip("The Point of Interest's approximate distance from Earth in Italian.")]
        public string ItalianDistance = "?ITA DIST?";
        public AudioClip ItalianVOMale;
        public AudioClip ItalianVOFemale;

        #region Offset Values
        /// <summary>
        /// The X-axis offset used to position the <see cref="InfoPanelBase"/> for a Point of Interest activated in the sky.
        /// </summary>
        [Header("Info-Panel (Sky) Offset Values")]
        /// <summary>
        /// The Transform used to rotate the Point of Interest into the user's original, forward-facing viewport.
        /// </summary>
        [Tooltip("The Transform used to rotate the Point of Interest into the user's original, forward-facing viewport.")]
        public Transform SkyTransform;

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
        #endregion

        #region Getters
        /// <summary>
        /// Returns the appropriate language's voice over to be played on Point of Interest activation (Male variant).
        /// </summary>
        public AudioClip VoiceoverMale
        {
            get
            {
                switch (SettingsManager.Instance.CurrentLanguageSetting)
                {
                    case LanguageSetting.English:
                        return EnglishVOMale;
                    case LanguageSetting.Italian:
                        return ItalianVOMale;
                    default:
                        return null;
                }
            }
        }

        /// <summary>
        /// Returns the appropriate language's voice over to be played on Point of Interest activation (Female variant).
        /// </summary>
        public AudioClip VoiceoverFemale
        {
            get
            {
                switch (SettingsManager.Instance.CurrentLanguageSetting)
                {
                    case LanguageSetting.English:
                        return EnglishVOFemale;
                    case LanguageSetting.Italian:
                        return ItalianVOFemale;
                    default:
                        return null;
                }
            }
        }

        /// <summary>
        /// Returns the appropriate language's Name field.
        /// </summary>
        public string Name
        {
            get
            {
                switch(SettingsManager.Instance.CurrentLanguageSetting)
                {
                    case LanguageSetting.English:
                        return EnglishName;
                    case LanguageSetting.Italian:
                        return ItalianName;
                    default:
                        return null;
                }
            }
        }

        /// <summary>
        /// Returns the appropriate language's Description field.
        /// </summary>
        
        public string Description
        {
            get
            {
                switch (SettingsManager.Instance.CurrentLanguageSetting)
                {
                    case LanguageSetting.English:
                        return EnglishDescription;
                    case LanguageSetting.Italian:
                        return ItalianDescription;
                    default:
                        return null;
                }
            }
        }

        /// <summary>
        /// Returns the appropriate language's Distance field.
        /// </summary>
        public string Distance
        {
            get
            {
                switch (SettingsManager.Instance.CurrentLanguageSetting)
                {
                    case LanguageSetting.English:
                        return EnglishDistance;
                    case LanguageSetting.Italian:
                        return ItalianDistance;
                    default:
                        return null;
                }
            }
        }
        #endregion

        #region Unity Methods
        void OnEnable()
        {
            ID = GetInstanceID();
        }
        #endregion
    }
}