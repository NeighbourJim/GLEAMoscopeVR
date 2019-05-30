using System;
using System.Collections.Generic;
using System.Linq;
using GLEAMoscopeVR.Audio;
using GLEAMoscopeVR.Settings;
using GLEAMoscopeVR.Spectrum;
using GLEAMoscopeVR.Utility.Management;
using UnityEngine;
using UnityEngine.Assertions;

namespace GLEAMoscopeVR.POIs
{
    /// <summary>
    /// Oversees Point of Interest behaviour based on <see cref="ExperienceMode"/>,
    /// updates the user interface, deactivates the previously active node when a new one is activated.
    /// Todo: make antenna available only in tutorial
    /// </summary>
    public class POIManager : GenericSingleton<POIManager>
    {
        /// <summary>
        /// Parent GameObject for all <see cref="POIMapNode"/>'s.
        /// </summary>
        [Header("Node Parent Objects")]
        [Tooltip("Parent GameObject for all POIMapNode's.")]
        public GameObject MapNodesParent;

        /// <summary>
        /// Parent GameObject for all <see cref="POISkyNode"/>'s.
        /// </summary>
        [Tooltip("Parent GameObject for all POISkyNode's.")]
        public GameObject SkyNodesParent;

        /// <summary>
        /// Parent GameObject for the SKA-LFAA antenna node.
        /// </summary>
        [Tooltip("Parent GameObject for all POISkyNode's.")]
        public GameObject AntennaNodeParent;

        /// <summary>
        /// InfoPanel_Tooltip script reference used to update the data displayed for Point of Interest nodes in the sky.
        /// </summary>
        [Header("UI")]
        [Tooltip("GameObject with the InfoPanel_Tooltip script attached.")]
        public InfoPanel_Tooltip InfoPanelSky;

        /// <summary>
        /// InfoPanel_WarTable script reference used to update the data displayed for Point of Interest above the War Table.
        /// </summary>
        [Tooltip("GameObject with the InfoPanel_Tooltip script attached.")]
        public InfoPanel InfoPanelWarTable;

        /// <summary>
        /// List of POIMapNodes retrieved from the MapNodesParent GameObject
        /// </summary>
        [Header("Nodes (Serialised for debugging)")]
        [SerializeField, Tooltip("Stores the POIMapNodes within the MapNodesParent GameObject.")]
        private List<POIMapNode> mapNodes;

        /// <summary>
        /// List of POISkyNodes retrieved from the MapSkyParent GameObject
        /// </summary>
        [SerializeField, Tooltip("Stores the POISkyNodes within the SkyNodesParent GameObject.")]
        private List<POISkyNode> skyNodes;

        [SerializeField]
        private POIAntennaNode antennaNode;

        /// <summary>
        /// The currently active (or most recently activated) POINode. For debugging purposes only.
        /// </summary>
        [SerializeField, Tooltip("The currently active (or most recently activated) POINode. For debugging purposes only.")]
        private POINode currentltyActiveNode = null;

        /// <summary>
        /// Reference to the current <see cref="ExperienceMode"/> set in the <see cref="ExperienceModeController"/>
        /// </summary>
        private ExperienceMode currentMode;

        public event Action OnAntennaPOIActivated;

        #region References
        ExperienceModeController _modeController;
        WavelengthStateController _wavelengthStateController;
        #endregion

        #region Unity Methods
        void Start()
        {
            SetAndCheckReferences();

            _wavelengthStateController.OnWavelengthChanged += HandleWavelengthChanged;
            _modeController.OnExperienceModeChanged += HandleExperienceModeChanged;

            RetrievePOINodesInScene();
            ResetNodeEventSubscriptions();
            SetMapNodeStates();
            SetSkyNodeStates();
        }

        private void SetAntennaNodeState()
        {
            if (_modeController.CurrentMode == ExperienceMode.Introduction)
            {
                antennaNode.SetActivatable();
            }
            else
            {
                antennaNode.SetInactive();
            }
        }

        /// <summary> Updates sky nodes state when the wavelength changes. </summary>
        private void HandleWavelengthChanged()
        {
            SetSkyNodeStates();
        }

        #endregion

        #region Experience Mode
        /// <summary>
        /// Resets subscriptions to node events based on the new <see cref="ExperienceMode"/>, hides info panels,
        /// resets the <see cref="currentltyActiveNode"/> and updates sky node state.
        /// </summary>
        private void HandleExperienceModeChanged()
        {
            if (_modeController.CurrentMode == currentMode) return;
            
            ResetNodeEventSubscriptions();
            ResetCurrentlyActiveNode();
            SetSkyNodeStates();
            SetMapNodeStates();
            SetAntennaNodeState();
        }

        private void SetMapNodeStates()
        {
            //var wavelength = WavelengthStateController.Instance.CurrentWavelength;
            var mode = _modeController.CurrentMode;
            var enable = mode == ExperienceMode.Exploration || mode == ExperienceMode.Passive;

            mapNodes
                .ForEach(n =>
                {
                    n.gameObject.SetActive(enable);
                    n.gameObject.GetComponentInChildren<MeshRenderer>().enabled = enable;
                });
        }

        #endregion

        #region Node Lists
        /// <summary> Adds the <see cref="POINode"/>'s in the <see cref="SkyNodesParent"/> and <see cref="MapNodesParent"/> GameObjects to the appropriate lists. </summary>
        private void RetrievePOINodesInScene()
        {
            AddMapNodesToList();
            AddSkyNodesToList();
            antennaNode = AntennaNodeParent.GetComponentInChildren<POIAntennaNode>();
        }

        /// <summary> Retrieves <see cref="POISkyNode"/>'s from the <see cref="SkyNodesParent"/> and adds them to the <see cref="skyNodes"/> list. </summary>
        private void AddSkyNodesToList()
        {
            skyNodes = new List<POISkyNode>();

            SkyNodesParent.GetComponentsInChildren<POISkyNode>()
                .ToList()
                .ForEach(n => skyNodes.Add(n));
        }

        /// <summary> Retrieves <see cref="POIMapNode"/>'s from the <see cref="MapNodesParent"/> and adds them to the <see cref="mapNodes"/> list. </summary>
        private void AddMapNodesToList()
        {
            mapNodes = new List<POIMapNode>();

            MapNodesParent.GetComponentsInChildren<POIMapNode>()
                .ToList()
                .ForEach(n => mapNodes.Add(n));
        }

        /// <summary>
        /// Iterates over <see cref="skyNodes"/> and enables / disables the mesh renderer and game object based on the current <see cref="Wavelengths"/> and <see cref="ExperienceMode"/>.
        /// Sky nodes should only be set active when in <see cref="ExperienceMode.Exploration"/> and wavelength is <see cref="Wavelengths.Visible"/> or <see cref="Wavelengths.Radio"/>.
        /// </summary>
        private void SetSkyNodeStates()
        {
            var wavelength = WavelengthStateController.Instance.CurrentWavelength;
            var mode = _modeController.CurrentMode;
            var enable = (wavelength == Wavelengths.Radio || wavelength == Wavelengths.Visible) && mode == ExperienceMode.Exploration;

            skyNodes
                .ForEach(n =>
                {
                    n.gameObject.SetActive(enable);
                    n.gameObject.GetComponentInChildren<MeshRenderer>().enabled = enable;
                });
        }

        #endregion

        #region Node Event Subscription
        /// <summary>
        /// Resets subscriptions to <see cref="POINode"/> events based on the <see cref="ExperienceMode"/> set in the <see cref="ExperienceModeController"/>.
        /// </summary>
        private void ResetNodeEventSubscriptions()
        {
            switch (_modeController.CurrentMode)
            {
                case ExperienceMode.Introduction:
                    UnsubscribeFromMapNodesEvents();
                    UnsubscribeFromSkyNodeEvents();
                    SubscribeToAntennaNodeEvents();
                    break;
                case ExperienceMode.Exploration:
                    UnsubscribeFromAntennaNodeEvents();
                    UnsubscribeFromMapNodesEvents();
                    SubscribeToSkyNodeEvents();
                    break;
                case ExperienceMode.Passive:
                    UnsubscribeFromAntennaNodeEvents();
                    UnsubscribeFromSkyNodeEvents();
                    SubscribeToMapNodeEvents();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(_modeController.CurrentMode), _modeController.CurrentMode, null);
            }
        }

        private void SubscribeToAntennaNodeEvents()
        {
            AntennaNodeParent.GetComponentInChildren<POIAntennaNode>().OnPOINodeActivated += HandleNodeActivation;
        }

        private void UnsubscribeFromAntennaNodeEvents()
        {
            AntennaNodeParent.GetComponentInChildren<POIAntennaNode>().OnPOINodeActivated -= HandleNodeActivation;
        }

        private void SubscribeToSkyNodeEvents()
        {
            foreach (var skyNode in skyNodes)
            {
                skyNode.OnPOINodeActivated += HandleNodeActivation;
            }
        }

        private void SubscribeToMapNodeEvents()
        {
            foreach (var mapNode in mapNodes)
            {
                mapNode.OnPOINodeActivated += HandleNodeActivation;
            }
        }

        private void UnsubscribeFromSkyNodeEvents()
        {
            foreach (var mapNode in mapNodes)
            {
                mapNode.OnPOINodeActivated -= HandleNodeActivation;
            }
        }

        private void UnsubscribeFromMapNodesEvents()
        {
            foreach (var mapNode in mapNodes)
            {
                mapNode.OnPOINodeActivated -= HandleNodeActivation;
            }
        }
        #endregion

        #region Node State
        /// <summary>
        /// Updates the <see cref="currentltyActiveNode"/>, deactivates the previously activated node and updates the <see cref="InfoPanelSky"/> and <see cref="InfoPanelWarTable"/>s.
        /// </summary>
        /// <param name="activatedNode">POINode from which the OnPOINodeActivated event was invoked.</param>
        private void HandleNodeActivation(POINode activatedNode)
        {
            if (activatedNode == null || activatedNode == currentltyActiveNode) return;

            if (currentltyActiveNode == null)
            {
                currentltyActiveNode = activatedNode;
            }
            else if (activatedNode != currentltyActiveNode)
            {
                currentltyActiveNode.Deactivate();
                currentltyActiveNode = activatedNode;
            }

            UpdateInfoPanels(activatedNode);
            TriggerVoiceOverClip(activatedNode);

            if (activatedNode == antennaNode)
            {
                OnAntennaPOIActivated?.Invoke();
                _modeController.SetIntroductionSequenceComplete();
            }
        }

        private void TriggerVoiceOverClip(POINode activatedNode)
        {
            VoiceOverController.Instance.RequestClipPlay(activatedNode.Data.VoiceoverFemale);//todo: determine when to use male vs female
        }

        /// <summary>
        /// Deactivates the <see cref="POINode"/> currently referenced by the <see cref="currentltyActiveNode"/> and sets it to null.
        /// </summary>
        private void ResetCurrentlyActiveNode()
        {
            if (currentltyActiveNode != null)
            {
                if (currentltyActiveNode != antennaNode)
                {
                    currentltyActiveNode.Deactivate();
                }
                currentltyActiveNode = null;
            }
        }
        #endregion

        #region UI
        /// <summary>
        /// Updates the <see cref="POIData"/> displayed in the <see cref="InfoPanelWarTable"/> and <see cref="InfoPanelSky"/>.
        /// </summary>
        /// <param name="activatedNode">The node containing the <see cref="POIData"/> to be displayed.</param>
        private void UpdateInfoPanels(POINode activatedNode)
        {
            print($"Update info panels for activated: {activatedNode.Data.Name}");
            var poi = new POIObject(activatedNode.Data);
            if (activatedNode is POISkyNode)
            {
                InfoPanelSky.CreateToolTip(poi, activatedNode.transform, poi.XOffset, poi.YOffset, poi.ZOffset);
            }
            InfoPanelWarTable.UpdateDisplay(poi);
        }
        #endregion

        private void SetAndCheckReferences()
        {
            _modeController = FindObjectOfType<ExperienceModeController>().Instance;
            _wavelengthStateController = WavelengthStateController.Instance;
            Assert.IsNotNull(_wavelengthStateController, $"[POIManager] cannot find SphereFadeControl in scene.");
            Assert.IsNotNull(_modeController, $"[POIManager] can not find reference to ExperienceModeController.");
            Assert.IsNotNull(InfoPanelSky, $"[POIManager] InfoPanel_Tooltip has not been assigned.");
            Assert.IsNotNull(InfoPanelWarTable, $"[POIManager] InfoPanel_WarTable has not been assigned assigned.");
            Assert.IsNotNull(MapNodesParent, $"[POIManager] MapNodesParent has not been assigned.");
            Assert.IsNotNull(SkyNodesParent, $"[POIManager] SkyNodesParent has not been assigned.");
            Assert.IsNotNull(AntennaNodeParent, $"[POIManager] AntennaNodeParent has not been assigned.");
            antennaNode = AntennaNodeParent.GetComponentInChildren<POIAntennaNode>();
            Assert.IsNotNull(antennaNode, $"[POIManager] antennaNode was not found as a child of the AntennaNodeParent.");
        }
    }
}