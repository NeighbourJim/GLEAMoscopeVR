using System;
using System.Collections.Generic;
using GLEAMoscopeVR.Settings;
using GLEAMoscopeVR.Utility.Management;
using UnityEngine;
using UnityEngine.Assertions;

namespace GLEAMoscopeVR.POIs
{
    /// <summary>
    /// Oversees Point of Interest behaviour based on <see cref="ExperienceMode"/>,
    /// updates the user interface, deactivates the previously active node when a new one is activated.
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
        /// InfoPanel_Manager script reference used to update the data displayed for Point of Interest nodes in the sky.
        /// </summary>
        [Header("UI")]
        [Tooltip("GameObject with the InfoPanel_Manager script attached.")]
        public InfoPanel_Manager InfoPanelSky;
        
        /// <summary>
        /// InfoPanel_WarTable script reference used to update the data displayed for Point of Interest above the War Table.
        /// </summary>
        [Tooltip("GameObject with the InfoPanel_Manager script attached.")]
        public InfoPanel_WarTable InfoPanelWarTable;

        /// <summary>
        /// List of POIMapNodes retrieved from the MapNodesParent GameObject
        /// </summary>
        [Header("Nodes (Debugging)")]
        [SerializeField, Tooltip("Stores the POIMapNodes within the MapNodesParent GameObject.")]
        private List<POIMapNode> mapNodes;

        /// <summary>
        /// List of POISkyNodes retrieved from the MapSkyParent GameObject
        /// </summary>
        [SerializeField, Tooltip("Stores the POISkyNodes within the SkyNodesParent GameObject.")]
        private List<POISkyNode> skyNodes;

        /// <summary>
        /// The currently active (or most recently activated) POINode. For debugging purposes only.
        /// </summary>
        [SerializeField, Tooltip("The currently active (or most recently activated) POINode. For debugging purposes only.")]
        private POINode currentltyActiveNode = null;

        /// <summary>
        /// Reference to the current <see cref="ExperienceMode"/> set in the <see cref="ExperienceModeController"/>
        /// </summary>
        private ExperienceMode currentMode = ExperienceMode.Exploration;

        #region References
        ExperienceModeController _modeController;
        #endregion

        #region Unity Methods
        void Start()
        {
            GetComponentReferences();
            _modeController.OnExperienceModeChanged += HandleExperienceModeChanged;

            RetrievePOINodesInScene();
            ResetNodeEventSubscriptions();
        }
        #endregion

        #region Experience Mode
        /// <summary>
        /// Resets subscriptions to node events based on the new <see cref="ExperienceMode"/>,
        /// hides info panels and resets the <see cref="currentltyActiveNode"/>.
        /// </summary>
        private void HandleExperienceModeChanged()
        {
            if (_modeController.CurrentMode == currentMode) return;

            ResetNodeEventSubscriptions();
            ResetCurrentlyActiveNode();
        }
        #endregion

        #region Node Lists
        /// <summary>
        /// Adds the <see cref="POINode"/>'s in the <see cref="SkyNodesParent"/> and <see cref="MapNodesParent"/> GameObjects to the appropriate lists.
        /// </summary>
        private void RetrievePOINodesInScene()
        {
            AddMapNodesToList();
            AddSkyNodesToList();
        }
        /// <summary> Retrieves <see cref="POISkyNode"/>'s from the <see cref="SkyNodesParent"/> and adds them to the <see cref="skyNodes"/> list. </summary>
        private void AddSkyNodesToList()
        {
            skyNodes = new List<POISkyNode>();
            foreach (var skyNode in SkyNodesParent.GetComponentsInChildren<POISkyNode>())
            {
                skyNodes.Add(skyNode);
            }
        }
        /// <summary> Retrieves <see cref="POIMapNode"/>'s from the <see cref="MapNodesParent"/> and adds them to the <see cref="mapNodes"/> list. </summary>
        private void AddMapNodesToList()
        {
            mapNodes = new List<POIMapNode>();
            foreach (var mapNode in MapNodesParent.GetComponentsInChildren<POIMapNode>())
            {
                mapNodes.Add(mapNode);
            }
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
                case ExperienceMode.Exploration:
                    UnsubscribeFromMapNodesEvents();
                    SubscribeToSkyNodeEvents();
                    break;
                case ExperienceMode.Passive:
                    UnsubscribeFromSkyNodeEvents();
                    SubscribeToMapNodeEvents();
                    break;
                //case ExperienceMode.Mixed: Todo: uncomment if required.
                //    Debug.LogError($"[POIManager] Cannot handle {_modeController.CurrentMode} mode.");
                //    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(_modeController.CurrentMode), _modeController.CurrentMode,
                        null);
            }
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
        /// Updates the <see cref="currentltyActiveNode"/>, deactivates the previously activated node
        /// and updates the <see cref="InfoPanelSky"/> and <see cref="InfoPanelWarTable"/>s.
        /// </summary>
        /// <param name="activatedNode">POINode from which the OnPOINodeActivated event was invoked.</param>
        private void HandleNodeActivation(POINode activatedNode)
        {
            if (activatedNode == null || activatedNode == currentltyActiveNode) return;

            POIObject poi = new POIObject(activatedNode.Data);

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
        }

        /// <summary>
        /// Deactivates the <see cref="POINode"/> currently referenced by the <see cref="currentltyActiveNode"/> and sets it to null.
        /// </summary>
        private void ResetCurrentlyActiveNode()
        {
            if (currentltyActiveNode != null)
            {
                currentltyActiveNode.Deactivate();
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
            POIObject poi = new POIObject(activatedNode.Data);
            if (activatedNode is POISkyNode)
            {
                InfoPanelSky.CreateToolTip(poi, activatedNode.transform, 0, 0, 0);
            }
            InfoPanelWarTable.UpdateDisplay(poi);
        }
        #endregion
        
        private void GetComponentReferences()
        {
            Assert.IsNotNull(InfoPanelSky, $"[POIManager] InfoPanel_Manager has not been assigned.");
            Assert.IsNotNull(InfoPanelWarTable, $"[POIManager] InfoPanel_WarTable has not been assigned assigned.");
            Assert.IsNotNull(MapNodesParent, $"[POIManager] MapNodesParent has not been assigned.");
            Assert.IsNotNull(SkyNodesParent, $"[POIManager] SkyNodesParent has not been assigned.");

            _modeController = ExperienceModeController.Instance;
        }
    }
}